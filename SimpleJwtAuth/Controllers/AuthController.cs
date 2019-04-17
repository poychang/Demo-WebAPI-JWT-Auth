using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleJwtAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration configuration)
        {
            _config = configuration;
        }

        // GET api/auth/login
        /// <summary>
        /// 取得 JWT Token
        /// </summary>
        /// <param name="name">呼叫者的名稱</param>
        /// <returns></returns>
        [HttpGet, Route("login")]
        public IActionResult Login(string name)
        {
            // STEP0: 在產生 JWT Token 之前，可以依需求做身分驗證

            // STEP1: 建立使用者的 Claims 聲明，這會是 JWT Payload 的一部分
            var userClaims = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.NameId, name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Custom", "Anything You Like")
            });
            // STEP2: 取得對稱式加密 JWT Signature 的金鑰
            // 這部分是選用，但此範例在 Startup.cs 中有設定 ValidateIssuerSigningKey = true 所以這裡必填
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            // STEP3: 建立 JWT TokenHandler 以及用於描述 JWT 的 TokenDescriptor
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Issuer"],
                Subject = userClaims,
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };
            // 產出所需要的 JWT Token 物件
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            // 產出序列化的 JWT Token 字串
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return new ContentResult() { Content = serializeToken };
        }
    }
}