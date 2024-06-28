using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IExtensionService
    {
        string[] UploadPicture(IFormFile Picture);
        string Base64String(IHostingEnvironment environmentstring, string fileName);
        string ConvertImageToBase64(IFormFile image);
        void DeleteFile(string Path);
        string GetLogId();
        string ResizeImageRatio(IFormFile file, int maxWidth, int maxHeight);
        string DecryptPassKiwi(string cipherString);
        void UploadImage(string base64String, string pathTarget);
    }
}
