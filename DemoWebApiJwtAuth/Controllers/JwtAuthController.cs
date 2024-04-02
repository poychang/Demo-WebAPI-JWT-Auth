using DemoWebApiJwtAuth.Models;
using DemoWebApiJwtAuth.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Principal;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoWebApiJwtAuth.Controllers
{
    [Route("api/[controller]")]
    public class JwtAuthController : Controller
    {
        /// <summary>
        /// JWT 設定項
        /// </summary>
        private readonly JwtOptions _jwtOptions;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtOptions"></param>
        public JwtAuthController(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        // POST: api/JwtAuth
        /// <summary>
        /// 登入取得並取得 JWT
        /// </summary>
        /// <param name="loginUser">使用者帳號資訊</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] LoginModel loginUser)
        {
            // 驗證並取得使用者資訊
            var identity = await GetClaimsIdentity(loginUser.Username, loginUser.Password);
            if (identity == null)
            {
                return BadRequest("Invalid credentials");
            }

            // 產生 JWT 並進行編碼
            var jwt = new SecurityTokenDescriptor {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, loginUser.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                    identity.FindFirst("Role")
                ]),
                NotBefore = _jwtOptions.NotBefore,
                Expires = _jwtOptions.Expiration,
                SigningCredentials = _jwtOptions.SigningCredentials
            };
            var encodedJwt = new JsonWebTokenHandler().CreateToken(jwt);

            var response = new
            {
                accessToken = encodedJwt,
                expiresIn = (int)_jwtOptions.ValidFor.TotalSeconds
            };
            return new JsonResult(response);
        }

        /// <summary>
        /// 計算距離 UNIX 紀元時間的秒數
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>秒數</returns>
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        /// <summary>
        /// 取得使用者識別資訊
        /// </summary>
        /// <remarks>DEMO 用，這邊請改成符合您環境的驗證方式</remarks>
        /// <param name="username">帳號</param>
        /// <param name="password">密碼</param>
        /// <returns></returns>
        private static Task<ClaimsIdentity> GetClaimsIdentity(string username, string password)
        {
            if (username == "admin" &&
                password == "password")
            {
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(username, "Token"),
                    new[]
                    {
                        new Claim("Role", "Admin")
                    }));
            }

            if (username == "member" &&
                password == "password")
            {
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(username, "Token"),
                    new[]
                    {
                        new Claim("Role", "Member")
                    }));
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }
    }
}
