using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Infrastructure.Data;
using dotnet_api_erp.src.Application.DTOs;

namespace dotnet_api_erp.src.Application.Utils
{
    public class FileProcessorUtils<TEntity>(ApplicationDbContext context) where TEntity : class
    {
        private readonly ApplicationDbContext _context = context;
        public async Task<List<object>> GetFilteredDataAsync<TResult>(
            Expression<Func<TEntity, bool>> whereExpression,
            Expression<Func<TEntity, TResult>>? selectExpression,
            CancellationToken ct,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeExpression = null
        )
        {
            var data = _context.Set<TEntity>().Where(whereExpression);

            if (includeExpression != null)
                data = includeExpression(data);

            if (selectExpression != null)
                return [.. (await data.Select(selectExpression).ToListAsync(ct)).Cast<object>()];

            return [.. (await data.ToListAsync(ct)).Cast<object>()];
        }
        public byte[] GenerateExcelFromData<TData>(List<TData> data)
        {
            if (data == null || data.Count == 0)
                throw new NotFoundException("Não há dados disponíveis para exportação.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(typeof(TData).Name);

            var firstItem = data.First();
            var properties = firstItem?.GetType().GetProperties();

            var customBooleanMappings = new Dictionary<string, Func<bool, string>>
            {
                { "Active", value => value ? "Ativo" : "Inativo" }
            };

            for (int i = 0; i < properties?.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
            }

            for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
            {
                var entity = data[rowIndex];
                for (int colIndex = 0; colIndex < properties?.Length; colIndex++)
                {
                    var propertyValue = properties[colIndex].GetValue(entity);
                    var propertyName = properties[colIndex].Name;

                    worksheet.Cell(rowIndex + 2, colIndex + 1).Value = propertyValue switch
                    {
                        null => string.Empty,
                        bool boolValue when customBooleanMappings.ContainsKey(propertyName) =>
                            customBooleanMappings[propertyName](boolValue),
                        bool boolValue => boolValue ? "Verdadeiro" : "Falso",
                        DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
                        _ => propertyValue?.ToString() ?? string.Empty
                    };
                }
            }

            worksheet.Columns().AdjustToContents();

            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }
        public async Task<byte[]> ExportFileAsync<TResult>(
            Expression<Func<TEntity, bool>> whereExpression,
            Expression<Func<TEntity, TResult>>? selectExpression,
            CancellationToken ct,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeExpression = null
        )
        {
            var data = await GetFilteredDataAsync(whereExpression, selectExpression, ct, includeExpression);
            return GenerateExcelFromData(data);
        }
        public async Task<List<TEntity>> ImportEntityAsync(FileDto base64file, List<Type>? relatedEntityTypes, CancellationToken ct)
        {
            return await ReadAndValidateExcelAsync(base64file, relatedEntityTypes, ct);;
        }

        public async Task ImportFileAsync(FileDto base64file, List<Type>? relatedEntityTypes, CancellationToken ct)
        {
            List<TEntity> entities = await ReadAndValidateExcelAsync(base64file, relatedEntityTypes, ct);
            
            foreach (var entity in entities)
            {
                await _context.Set<TEntity>().AddAsync(entity, ct);
            }
            
            await _context.SaveChangesAsync(ct);
        }

        public async  Task<List<TEntity>> ReadAndValidateExcelAsync(FileDto base64file, List<Type>? relatedEntityTypes, CancellationToken ct)
        {
            List<TEntity> entities = new List<TEntity>();
            byte[] file = Convert.FromBase64String(base64file.Base64);

            string[] allowedExtensions = ["xls", "xlsx"];
            if (!allowedExtensions.Contains(base64file.FileType.ToLowerInvariant()))
            {
                throw new BadRequestException($"Extenção de arquivo invalida: {base64file.FileType}; aceitamos apenas .xls e .xlsx");
            }

            string tempFilePath = Path.ChangeExtension(Path.GetTempFileName(), base64file.FileType);
            await File.WriteAllBytesAsync(tempFilePath, file, ct);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                using (var stream = new MemoryStream(file))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = static (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    var tables = result.Tables
                        .Cast<DataTable>()
                        .Select(static t => new
                        {
                            t.TableName,
                            Columns = t.Columns
                                .Cast<DataColumn>().Select(x => x.ColumnName).ToList()
                        })
                        .ToList();

                    do
                    {
                        while (reader.Read())
                        {
                            if (reader.Depth == 0) continue;

                            var values = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                values.Add(reader.GetValue(i)?.ToString() ?? string.Empty);
                            }
                            var entity = await MapRowToTEntity(tables[0].Columns, values, relatedEntityTypes);

                            if (await ValidateEntityAsync(entity, ct))
                            {
                                entities.Add(entity);
                            }
                        }
                    } while (reader.NextResult());
                }
               return entities;
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Erro ao processar o arquivo: {ex.Message}");
            }
            finally
            {
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }
        private async Task<TEntity> MapRowToTEntity(List<string> copyColumns, List<string> values, List<Type>? relatedEntityTypes)
        {
            var columns = new List<string>(copyColumns);
            TEntity entity = Activator.CreateInstance<TEntity>();
            var properties = typeof(TEntity).GetProperties();

            for (int i = 0; i < columns.Count; i++)
            {
                var property = properties.FirstOrDefault(p =>
                    string.Equals(p.Name, columns[i], StringComparison.OrdinalIgnoreCase));

                if (property != null && property.CanWrite)
                {
                    var value = values[i];
                    try
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(entity, value);
                        }
                        else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                        {
                            if (int.TryParse(value, out var intValue))
                            {
                                property.SetValue(entity, intValue);
                            }
                            else
                            {
                                throw new BadRequestException($"Erro ao converter '{value}' para numero inteiro na propriedade '{property.Name}'.");
                            }
                        }
                        else if ((property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?)) && decimal.TryParse(value, out var decimalValue))
                        {
                            property.SetValue(entity, decimalValue);
                        }
                        else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                        {
                            var trueValues = new[] { "true", "t", "verdadeiro", "v", "sim", "s", "1", "ativo" };
                            var falseValues = new[] { "false", "f", "falso", "não", "nao", "n", "0", "inativo" };

                            var normalizedValue = value.Trim().ToLowerInvariant();
                            if (trueValues.Contains(normalizedValue))
                                property.SetValue(entity, true);
                            else if (falseValues.Contains(normalizedValue))
                                property.SetValue(entity, false);
                            else
                                throw new BadRequestException($"Erro ao converter '{value}' para booleano na propriedade '{property.Name}'.");
                        }
                        else if (property.PropertyType == typeof(DateTime) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
                        {
                            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
                            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

                            string[] formatosPermitidos =
                            [
                                "dd/MM/yyyy", "dd-MM-yyyy", "dd/MM/yy", "dd-MM-yy",
                                "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy HH:mm", "dd-MM-yyyy HH:mm",
                                "dd/MM/yy HH:mm", "dd-MM-yy HH:mm", "yyyy-MM-dd HH:mm", "yyyy/MM/dd HH:mm",
                                "dd/MM/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss", "dd/MM/yy HH:mm:ss",
                                "dd-MM-yy HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss",
                                "MM/dd/yyyy", "MM-dd-yyyy", "MM/dd/yyyy HH:mm:ss", "MM-dd-yyyy HH:mm:ss"
                            ];


                            if (DateTime.TryParseExact(value, formatosPermitidos,
                                                        new CultureInfo("pt-BR"),
                                                        DateTimeStyles.None, out var dateValue))
                            {
                                property.SetValue(entity, dateValue);
                            }
                            else
                            {
                                throw new BadRequestException($"Erro ao converter '{value}' para data na propriedade '{property.Name}'.");
                            }
                        }
                        else if (property.PropertyType == typeof(Guid) && Guid.TryParse(value, out var guidValue))
                        {
                            property.SetValue(entity, guidValue);
                        }
                        else if (property.PropertyType.IsEnum)
                        {
                            if (Enum.TryParse(property.PropertyType, value, true, out var enumValue))
                            {
                                property.SetValue(entity, enumValue);
                            }
                            else
                            {
                                throw new BadRequestException($"Erro ao converter '{value}' para o enum na propriedade '{property.Name}'.");
                            }
                        }
                        else if (relatedEntityTypes != null && relatedEntityTypes.Contains(property.PropertyType))
                        {
                            if (Guid.TryParse(value, out var relatedEntityId))
                            {
                                var dbSet = _context.GetType()
                                    .GetMethod("Set", Type.EmptyTypes)?
                                    .MakeGenericMethod(property.PropertyType)
                                    .Invoke(_context, null);

                                if (dbSet is IQueryable queryable)
                                {
                                    var query = ((IQueryable<object>)queryable)
                                        .Cast<object>()
                                        .Where(e => EF.Property<Guid>(e, "Id") == relatedEntityId);

                                    var relatedEntity = await query.FirstOrDefaultAsync() ?? throw new Exception($"Entidade relacionada não encontrada para '{property.Name}' com o ID '{value}'.");

                                    property.SetValue(entity, relatedEntity);
                                }
                            }
                            else if (string.IsNullOrEmpty(value))
                            {
                                property.SetValue(entity, null);
                            }
                            else
                            {
                                var dbSet = _context.GetType()
                                    .GetMethod("Set", Type.EmptyTypes)?
                                    .MakeGenericMethod(property.PropertyType)
                                    .Invoke(_context, null);

                                if (dbSet is IQueryable queryable)
                                {
                                    var entityType = _context.Model.FindEntityType(property.PropertyType) ?? throw new Exception($"Tipo de entidade não encontrado para '{property.PropertyType}'.");

                                    var uniqueProperty = entityType.GetProperties()
                                        .FirstOrDefault(p => p.GetContainingIndexes().Any(i => i.IsUnique)) ?? throw new Exception($"Nenhum campo único encontrado para a entidade '{entityType.Name}'.");

                                    var relatedEntity = await ((IQueryable<object>)queryable)
                                        .Cast<object>()
                                        .FirstOrDefaultAsync(e => EF.Property<string>(e, uniqueProperty.Name) == value.ToString()) ?? throw new Exception($"Entidade relacionada não encontrada para '{property.Name}' com o valor único '{value}'.");

                                    property.SetValue(entity, relatedEntity);
                                }
                            }
                        }
                        else
                        {
                            throw new BadRequestException($"Tipo de propriedade não suportado: {property.PropertyType}");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new BadRequestException($"Erro ao mapear valor para a propriedade '{property.Name}': {ex.Message}");
                    }
                }
            }
            return entity;
        }
        protected virtual Task<bool> ValidateEntityAsync(TEntity entity, CancellationToken ct) => Task.FromResult(true);
    }
}
