using DemoWebApiJwtAuth.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DemoWebApiJwtAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // 從 appsettings.json 中取得 JWT 配置
            var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions));

            builder.Services.AddControllers();

            // 註冊 Options Pattern 服務，將配置內容註冊到容器裡，來獲取對應的服務 Provider 對象
            builder.Services.AddOptions();
            // 設定 JwtIssuerOptions
            builder.Services.Configure<JwtOptions>(options =>
            {
                options.Issuer = jwtOptions[nameof(JwtOptions.Issuer)] ?? string.Empty;
                options.Audience = jwtOptions[nameof(JwtOptions.Audience)] ?? string.Empty;
                options.ValidFor = TimeSpan.FromMinutes(10);
                options.SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions[nameof(JwtOptions.SecretKey)] ?? string.Empty)), SecurityAlgorithms.HmacSha256);
            });
            // 設定用哪種方式驗證 HTTP Request 是否合法
            builder.Services
                // 檢查 HTTP Header 的 Authorization 是否有 JWT Bearer Token
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                // 設定 JWT Bearer Token 的檢查選項
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions[nameof(JwtOptions.Issuer)],

                        ValidateAudience = true,
                        ValidAudience = jwtOptions[nameof(JwtOptions.Audience)],

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions[nameof(JwtOptions.SecretKey)] ?? string.Empty)),

                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero
                    };
                });
            // 設定驗證的 Policy
            builder.Services.AddAuthorization(options =>
            {
                // 設定驗證原則
                // 這裡僅驗證是否有指定角色
                options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
                options.AddPolicy("Member", policy => policy.RequireClaim("Role", "Member"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            // 使用驗證權限的 Middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
