namespace DemoWebApiJwtAuth.Models
{
    /// <summary>
    /// 登入帳號模型
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; }
    }
}
