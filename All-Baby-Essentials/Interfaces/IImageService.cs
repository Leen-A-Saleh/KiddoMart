using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace All_Baby_Essentials.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImage(IFormFile image);
        bool DeleteImage(string fileName);
    }
}
