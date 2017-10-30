using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DemoWebApiJwtAuth.Models;
using DemoWebApiJwtAuth.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoWebApiJwtAuth.Controllers
{
    [Route("api/[controller]")]
    public class JwtController : Controller
    {
        /// <summary>
        /// JWT 設定項
        /// </summary>
        private readonly JwtIssuerOptions _jwtIssuerOptions;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtOptions"></param>
        public JwtController(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtIssuerOptions = jwtOptions.Value;
        }

        // POST: api/jwt
        /// <summary>
        /// 登入取得並取得 JWT
        /// </summary>
        /// <param name="logingUser">使用者帳號資訊</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody]LoginModel logingUser)
        {
            // 驗證並取得使用者資訊
            var identity = await GetClaimsIdentity(logingUser.Username, logingUser.Password);
            if (identity == null)
            {
                return BadRequest("Invalid credentials");
            }

            // 產生 JWT 並進行編碼
            var jwt = new JwtSecurityToken(
                issuer: _jwtIssuerOptions.Issuer,
                audience: _jwtIssuerOptions.Audience,
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, logingUser.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, await _jwtIssuerOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtIssuerOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                    identity.FindFirst("Role")
                },
                notBefore: _jwtIssuerOptions.NotBefore,
                expires: _jwtIssuerOptions.Expiration,
                signingCredentials: _jwtIssuerOptions.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                accessToken = encodedJwt,
                expiresIn = (int)_jwtIssuerOptions.ValidFor.TotalSeconds
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
