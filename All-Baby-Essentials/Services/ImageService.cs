using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using All_Baby_Essentials.Interfaces;

namespace All_Baby_Essentials.Services
{

    public class ImageService : IImageService
    {
        private readonly string _imagePath;

        public ImageService() {
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "upload");
        }

        public async Task<string> UploadImage(IFormFile Image)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
            var filePath = Path.Combine(_imagePath, fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await Image.CopyToAsync(stream);
            }

            return fileName;
        } 

        public bool DeleteImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            var fullPath = Path.Combine(_imagePath, fileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

    }
}
