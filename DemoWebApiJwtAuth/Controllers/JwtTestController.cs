using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace DemoWebApiJwtAuth.Controllers
{
    [Route("api/[controller]")]
    public class JwtTestController : Controller
    {
        // GET api/JwtTest/Anonymous
        /// <summary>
        /// 使用匿名登入，無視於身分驗證
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("anonymous")]
        [HttpGet]
        public IActionResult Anonymous()
        {
            var name = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
            return new ContentResult() { Content = $@"For all anonymous. {name}" };
        }

        /// <summary>
        /// 使用身分驗證，HTTP 的 Authorization Header 必須設定合法的 JWT Bearer Token 才能使用
        /// </summary>
        /// <returns></returns>
        // GET api/JwtTest/Authorize
        [Authorize]
        [Route("authorize")]
        [HttpGet]
        public IActionResult All()
        {
            var name = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
            return new ContentResult() { Content = $@"For all client who authorize. {name}" };
        }

        // GET api/JwtTest/roles
        /// <summary>
        /// 使用角色驗證，所提供的 JWT Bearer Token 必須擁有指定角色才能使用
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,administrator")]
        [Route("roles")]
        public IActionResult Admin()
        {
            var name = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
            return new ContentResult() { Content = $@"For all client who's role is admin or administrator. {name}" };
        }

        // GET: api/JwtTest/Admin
        /// <summary>
        /// 使用原則驗證，所提供的 JWT Bearer Token 必須通過 Admin 驗證原則才能使用
        /// </summary>
        /// <returns></returns>
        [HttpGet("admin")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetAdmin()
        {
            return new ContentResult() { Content = "I Am Admin" };
        }

        // GET: api/JwtTest/Member
        /// <summary>
        /// 使用原則驗證，所提供的 JWT Bearer Token 必須通過 Member 驗證原則才能使用
        /// </summary>
        /// <returns></returns>
        [HttpGet("member")]
        [Authorize(Policy = "Member")]
        public IActionResult GetMember()
        {
            return new ContentResult() { Content = "I Am Member" };
        }
    }
}
