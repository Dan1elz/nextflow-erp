using System.Text;
using dotnet_api_erp.src.API.Middlewares;
using dotnet_api_erp.src.Application.Services.ProductContext;
using dotnet_api_erp.src.Application.Services.UserContext;
using dotnet_api_erp.src.Application.Utils;
using dotnet_api_erp.src.Domain.Interfaces.ProductContext;
using dotnet_api_erp.src.Domain.Interfaces.SalesContext;
using dotnet_api_erp.src.Domain.Interfaces.UserContext;
using dotnet_api_erp.src.Infrastructure.Data;
using dotnet_api_erp.src.Infrastructure.Repositories.ProductContext;
using dotnet_api_erp.src.Infrastructure.Repositories.SalesContext;
using dotnet_api_erp.src.Infrastructure.Repositories.UserContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace dotnet_api_erp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // *** CONFIGURAÇÃO DO DATABASE CONTEXT ***
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // *** CONFIGURAÇÃO DE CORS ***
            builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
            {
                policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            }));

            // *** CONFIGURAÇÃO DO SWAGGER ***
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
                });
            });

            // *** CONFIGURAÇÃO DE AUTENTICAÇÃO JWT ***
            var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            builder.Services.Configure<JwtUtils.JwtSettingsUseCase>(
                builder.Configuration.GetSection("JwtSettings"));

            // *** ADICIONANDO CONTROLLERS COM SUPORTE A NEWTONSOFT.JSON ***
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            builder.Services.AddEndpointsApiExplorer();

            // *** REGISTRO DE DEPENDÊNCIAS ***
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<AddressService>();
            builder.Services.AddScoped<ContactService>();
            builder.Services.AddScoped<RefreshTokenService>();
            
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<SupplierService>();
            builder.Services.AddScoped<StockMovementService>();
            
            builder.Services.AddHttpContextAccessor();

            // CONTEXTO DE USER
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();

            // CONTEXTO DE SALES
            builder.Services.AddScoped<ISaleRepository, SaleRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();

            // CONTEXTO DE PRODUCT
            builder.Services.AddScoped<ISuppliersRepository, SuppliersRepository>();
            builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryProductRepository, CategoryProductRepository>();


            builder.Services.AddScoped<JwtUtils>();
            builder.Services.AddScoped<ImageUtils>();

            // *** CONFIGURAÇÃO DO APP ***
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Executa Financeiro API V2");
                    c.RoutePrefix = string.Empty;
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors();

            app.UseRouting();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            // *** MAPEAMENTO DE CONTROLLERS ***
            app.MapControllers();

            // *** EXECUÇÃO DA APLICAÇÃO ***
            app.Run();
        }
    }
}