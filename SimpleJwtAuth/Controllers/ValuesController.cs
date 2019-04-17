using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleJwtAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values/anonymous
        /// <summary>
        /// 使用匿名登入，無視於身分驗證
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("anonymous")]
        public IActionResult Anonymous()
        {
            return new ContentResult() { Content = $@"For all anonymous." };
        }

        // GET api/values/authorize
        /// <summary>
        /// 使用身分驗證，HTTP 的 Authorization Header 必須設定合法的 JWT Bearer Token 才能使用
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet, Route("authorize")]
        public IActionResult All()
        {
            return new ContentResult() { Content = $@"For all client who authorize." };
        }
    }
}
