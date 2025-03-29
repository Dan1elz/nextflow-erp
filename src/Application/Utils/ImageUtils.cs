using Microsoft.AspNetCore.Mvc;

namespace dotnet_api_erp.src.Application.Utils
{
    public class ImageUtils
    {
        public async Task<string> SaveImg(IFormFile img, string path, CancellationToken ct)
        {
            if (img == null || img.Length == 0) throw new Exception("A imagem é obrigatória");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(img.FileName).ToLower();
            if (!allowedExtensions.Contains(extension)) throw new Exception("A extensão do arquivo não é permitida");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(path, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await img.CopyToAsync(stream, ct);
            }
            return fileName;
        }
        public void RemoveImg(string path, string fileName)
        {
            var filePath = Path.Combine(path, fileName);
            if (File.Exists(filePath)) File.Delete(filePath);
        }
        public async Task<FileContentResult?> GetImg(string path, string fileName, CancellationToken ct)
        {
            var filePath = Path.Combine(path, fileName);
            if (!File.Exists(filePath)) return null;

            byte[] bytes = await File.ReadAllBytesAsync(filePath, ct);
            return new FileContentResult(bytes, "image/jpeg");
        }
    }
}