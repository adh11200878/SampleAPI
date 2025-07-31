using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;

namespace SampleAPI.Helper
{
    public class FileUploadHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        //中文、英文大小寫、數字、底線、句點、破折號
        private readonly Regex _fileNameRegex = new(@"^[\u4e00-\u9fa5a-zA-Z0-9_.\-]+$");

        public FileUploadHelper(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task<(bool isSuccess, string message)> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (true, ""); // 當作允許的情境處理，不是錯誤

            // 直接從 IConfiguration 讀取設定
            var maxFileSize = _configuration.GetValue<long>("FileUploadOptions:MaxFileSize");
            var allowedExtensions = _configuration.GetSection("FileUploadOptions:AllowedExtensions").Get<string[]>();
            var savePathConfig = _configuration.GetValue<string>("FileUploadOptions:SavePath") ?? "uploads";

            if (file.Length > maxFileSize)
                return (false, $"檔案超過最大限制：{maxFileSize / (1024 * 1024)}MB");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (allowedExtensions == null || !allowedExtensions.Contains(extension))
                return (false, $"不支援的副檔名：{extension}");

            var fileName = Path.GetFileName(file.FileName);
            if (!_fileNameRegex.IsMatch(fileName))
                return (false, "檔案名稱包含不合法的字元");

            var savePath = Path.Combine(_env.ContentRootPath, savePathConfig);
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var newFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(savePath, newFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, newFileName);
        }
    }
}
