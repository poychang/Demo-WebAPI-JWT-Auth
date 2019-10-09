using DemoWebApiJwtAuth.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace DemoWebApiJwtAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 註冊 Options Pattern 服務，將配置內容註冊到容器裡，來獲取對應的服務 Provider 對象
            services.AddOptions();

            // 設定使用驗證 MVC 路由
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            // 從 appsettings.json 中取得 JWT 配置
            var jwtOptions = Configuration.GetSection(nameof(JwtOptions));

            // 設定 JwtIssuerOptions
            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = jwtOptions[nameof(JwtOptions.Issuer)];
                options.Audience = jwtOptions[nameof(JwtOptions.Audience)];
                options.ValidFor = TimeSpan.FromMinutes(10);
                options.SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions[nameof(JwtOptions.SecretKey)])), SecurityAlgorithms.HmacSha256);
            });

            // 設定驗證的 Policy
            services.AddAuthorization(options =>
                {
                    // 設定驗證原則
                    // 這裡僅驗證是否有指定角色
                    options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
                    options.AddPolicy("Member", policy => policy.RequireClaim("Role", "Member"));
                });
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions[nameof(JwtOptions.Issuer)],

                        ValidateAudience = true,
                        ValidAudience = jwtOptions[nameof(JwtOptions.Audience)],

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions[nameof(JwtOptions.SecretKey)])),

                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 使用驗證權限的 Middleware
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
