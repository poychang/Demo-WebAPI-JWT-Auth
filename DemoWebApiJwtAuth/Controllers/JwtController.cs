using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DemoWebApiJwtAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoWebApiJwtAuth.Controllers
{
    [Route("api/[controller]")]
    public class JwtController : Controller
    {
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
            return new EmptyResult();
        }

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
