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

//�s�u�r��
builder.Services.AddScoped<IDbConnection>(sp =>
 new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

//���UServiceCollectionExtensions �ѼƬO�Nappsettings.json�ǤJ
builder.Services.AddApplicationServices(builder.Configuration);

//���]�w
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", p =>
    {
        p.WithOrigins(
            "http://localhost:5110",        // �����}�o�e�ݡ]Vue�BReact�^
            "http://127.0.0.1:5110",        // ���� IP �}�o
            "https://your-frontend.com",    // ��������
            "http://192.168.1.100:8080"     // ����������w IP
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // �p�G�n��a cookie �� JWT token ��ĳ�[�W(�ϥ�cookie �~�[)
    });
});

//�[�J���v
builder.Services.AddAuthorization(options =>
{
    // �s�W�@�ӱ��v�F���A�W�٬� JwtBearerDefaults.AuthenticationScheme�]���N�O "Bearer"�^
    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
    {
        // ���w�o�ӱ��v�F���ϥ� JWT ���ҡ]Bearer Token�^
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        // �n�D�ϥΪ֦̾� Name�PRole �� Claim �~��q�L���v
        policy.RequireClaim(ClaimTypes.Name);
        policy.RequireClaim(ClaimTypes.Role);
    });
});

// ���U JWT Bearer ���Ҿ���
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // �ϥιw�]�� "Bearer" ���Ҥ��
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret �]�w���`");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ���ҬO�_�ŦX�]�w�� Audience�]�����^
            // �]�� false ��ܤ����� Audience�]�}�o�i�o�˳]�A������ĳ�אּ true�^
            ValidateAudience = false,
            //  ���ҬO�_�ŦX�]�w�� Issuer�]�o��̡^
            // �]�� false ��ܤ����� Issuer�]�}�o�i�o�˳]�A������ĳ�אּ true�^
            ValidateIssuer = false,
            // ���� Actor�]�q�`�Ω�h�������A�D���n�i�] false�^
            ValidateActor = false,
            // ���� Token �O�_�L��
            ValidateLifetime = true,
            // �]�w��٦����_�A�����P���� Token �ɨϥΪ��K�_�ۦP
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            )
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection(); // 1. �j���ର HTTPS
app.UseCors("cors");       // 2. CORS �����b Auth ���e�A�~��^���w�˽ШD�]OPTIONS�^
app.UseAuthentication();   // 3. ���ҡG�ѪR Token�A�إ� HttpContext.User
app.UseAuthorization();    // 4. ���v�G�ھ� User �P Policy �M�w��_�s���귽
app.MapControllers();
app.Run();