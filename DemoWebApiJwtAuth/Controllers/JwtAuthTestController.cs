using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoWebApiJwtAuth.Controllers
{
    [Route("api/[controller]")]
    public class JwtAuthTestController : Controller
    {
        // GET: api/JwtAuthTest/Admin
        [HttpGet("admin")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetAdmin()
        {
            return new JsonResult("I Am Admin");
        }

        // GET: api/JwtAuthTest/Member
        [HttpGet("member")]
        [Authorize(Policy = "Member")]
        public IActionResult GetMember()
        {
            return new JsonResult("I Am Member");
        }
    }
}
