using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleAPI.Helper
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _jwtTokenHandler;

        // 建構函式 - 注入 IConfiguration 來讀取 appsettings.json 的設定值
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jwtTokenHandler = new JwtSecurityTokenHandler(); // 負責建立與解析 JWT 的工具
        }

        /// <summary>
        /// 產生 JWT Token
        /// </summary>
        /// <param name="name">使用者名稱</param>
        /// <param name="role">使用者角色</param>
        /// <returns>JWT 字串</returns>
        public string GenerateJwtToken(string name, string role)
        {
            // 檢查輸入參數是否合法
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(role))
            {
                throw new InvalidOperationException("Name and Role is not specified.");
            }

            // 從設定檔取得 JWT 密鑰
            var jwtSecret = _configuration["jwt:Secret"];
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT Secret is not configured in appsettings.");
            }

            // 設定 JWT 的 Claims（聲明） - 包含使用者名稱與角色
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name), // 內建的 ClaimTypes.Name
                new Claim(ClaimTypes.Role, role)  // 內建的 ClaimTypes.Role
            };

            // 建立金鑰並使用 HmacSha256 作為簽章演算法
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                SecurityAlgorithms.HmacSha256);

            // 建立 JWT Token
            var token = new JwtSecurityToken(
                issuer: "ExampleServer",         // 發行者，可替換為你的網域
                audience: "ExampleClients",       // 接收者，通常為前端應用程式
                claims: claims,                   // 包含的聲明資料
                expires: DateTime.Now.AddSeconds(1800), // Token 有效期限：30 分鐘
                signingCredentials: credentials   // 用來加密簽名的金鑰
            );

            // 回傳序列化後的 JWT Token 字串
            return _jwtTokenHandler.WriteToken(token);
        }
    }
}
