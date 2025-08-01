using System.Text.RegularExpressions;

namespace SampleAPI.Helper
{
    public class FileHelper
    {
        private readonly IConfiguration _configuration;
        //_env.WebRootPath	取得 wwwroot 資料夾的實體路徑（通常用來儲存靜態檔案）
        //_env.ContentRootPath 取得應用程式的根目錄（通常是 Program.cs 所在的資料夾）
        //_env.EnvironmentName 取得目前的執行環境名稱（例如：Development、Staging、Production）
        //_env.IsDevelopment() 判斷是否為開發環境
        private readonly IWebHostEnvironment _env; 
        private readonly Regex _fileNameRegex = new(@"^[\u4e00-\u9fa5a-zA-Z0-9_.\-]+$");//中文、英文大小寫、數字、底線、句點、破折號
        private readonly long _maxFileSize; //容量限制
        private readonly string[] _allowedExtensions;//副檔名
        private readonly string[] _allowedMimeTypes;//MIME TYPE限制
        private readonly string _savePathConfig;//上傳路徑

        public FileHelper(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _maxFileSize = _configuration.GetValue<long>("FileUploadOptions:MaxFileSize");
            _allowedExtensions = _configuration.GetSection("FileUploadOptions:AllowedExtensions").Get<string[]>() ?? Array.Empty<string>();
            _allowedMimeTypes = _configuration.GetSection("FileUploadOptions:AllowedMimeTypes").Get<string[]>() ?? Array.Empty<string>();
            _savePathConfig = _configuration.GetValue<string>("FileUploadOptions:SavePath") ?? "uploads";
        }

        /// <summary>
        /// 上傳檔案共用函數
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="newFileName">新檔案名稱</param>
        /// <returns></returns>
        public async Task<(bool isSuccess, string message)> UploadFileAsync(IFormFile file,string newFileName)
        {
            if (file == null || file.Length == 0)
                return (true, ""); // 當作允許的情境處理，不是錯誤

            if (file.Length > _maxFileSize)
                return (false, $"檔案超過最大限制：{_maxFileSize / (1024 * 1024)}MB");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (_allowedExtensions == null || !_allowedExtensions.Contains(extension))
                return (false, $"不支援的副檔名：{extension}");

            var fileName = Path.GetFileName(file.FileName);
            if (!_fileNameRegex.IsMatch(fileName))
                return (false, "檔案名稱包含不合法的字元");

            var mimeType = file.ContentType;
            if (_allowedMimeTypes == null || !_allowedMimeTypes.Contains(mimeType))
            {
                return (false, $"不支援的檔案類型：{mimeType}");
            }

            var savePath = Path.Combine(_env.ContentRootPath, _savePathConfig);
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            //沒有傳入新檔案名稱就直接使用GUID編碼
            if (string.IsNullOrEmpty(newFileName))
                newFileName = $"{Guid.NewGuid()}{extension}";
            else
                newFileName = $"{newFileName}{extension}";

            var fullPath = Path.Combine(savePath, newFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, newFileName);
        }

    }
}
