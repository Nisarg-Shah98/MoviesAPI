using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class InAppStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _host;
        private readonly IHttpContextAccessor _http;

        public InAppStorageService(IWebHostEnvironment host, IHttpContextAccessor http)
        {
            _host = host;
            _http = http;
        }
        public Task DeleteFile(string fileRoute, string containerName)
        {
            if (string.IsNullOrEmpty(fileRoute))
            {
                return Task.CompletedTask;
            }
            var fileName = Path.GetFileName(fileRoute);
            var fileDirectory = Path.Combine(_host.WebRootPath, containerName, fileName);
            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
            return Task.CompletedTask;
        }

        public async Task<string> EditFile(string containerName, IFormFile file, string fileRoute)
        {
            await DeleteFile(fileRoute, containerName);
            return await SaveFile(containerName, file);
        }

        public async Task<string> SaveFile(string containerName, IFormFile file)
        {
            try
            {
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                string folder = Path.Combine(_host.WebRootPath, containerName);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string route = Path.Combine(folder, fileName);
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    var content = ms.ToArray();
                    await File.WriteAllBytesAsync(route, content);
                }
                var url = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host}";
                var routeDB = Path.Combine(url, containerName, fileName).Replace("\\", "/");
                return routeDB;

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
