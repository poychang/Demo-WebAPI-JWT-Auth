using DemoWebApiJwtAuth.Models;
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
        public IActionResult Post([FromBody]LoginModel logingUser)
        {
            return new EmptyResult();
        }
    }
}
