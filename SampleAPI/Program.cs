using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using SampleAPI.Extension;
using System.Data;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//連線字串
builder.Services.AddScoped<IDbConnection>(sp =>
 new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

//註冊ServiceCollectionExtensions 參數是將appsettings.json傳入
builder.Services.AddApplicationServices(builder.Configuration);

//跨域設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", p =>
    {
        p.WithOrigins(
            "http://localhost:5110",        // 本機開發前端（Vue、React）
            "http://127.0.0.1:5110",        // 本機 IP 開發
            "https://your-frontend.com",    // 正式網域
            "http://192.168.1.100:8080"     // 局域網內指定 IP
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // 如果要攜帶 cookie 或 JWT token 建議加上(使用cookie 才加)
    });
});

//加入授權
builder.Services.AddAuthorization(options =>
{
    // 新增一個授權政策，名稱為 JwtBearerDefaults.AuthenticationScheme（其實就是 "Bearer"）
    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
    {
        // 指定這個授權政策使用 JWT 驗證（Bearer Token）
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        // 要求使用者擁有 Name與Role 的 Claim 才能通過授權
        policy.RequireClaim(ClaimTypes.Name);
        policy.RequireClaim(ClaimTypes.Role);
    });
});

// 註冊 JWT Bearer 驗證機制
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // 使用預設的 "Bearer" 驗證方案
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret 設定異常");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // 驗證是否符合設定的 Audience（受眾）
            // 設為 false 表示不驗證 Audience（開發可這樣設，正式建議改為 true）
            ValidateAudience = false,
            //  驗證是否符合設定的 Issuer（發行者）
            // 設為 false 表示不驗證 Issuer（開發可這樣設，正式建議改為 true）
            ValidateIssuer = false,
            // 驗證 Actor（通常用於多重身份，非必要可設 false）
            ValidateActor = false,
            // 驗證 Token 是否過期
            ValidateLifetime = true,
            // 設定對稱式金鑰，必須與產生 Token 時使用的密鑰相同
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            )
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection(); // 1. 強制轉為 HTTPS
app.UseCors("cors");       // 2. CORS 必須在 Auth 之前，才能回應預檢請求（OPTIONS）
app.UseAuthentication();   // 3. 驗證：解析 Token，建立 HttpContext.User
app.UseAuthorization();    // 4. 授權：根據 User 與 Policy 決定能否存取資源
app.MapControllers();
app.Run();