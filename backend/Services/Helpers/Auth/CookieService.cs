namespace backend.Services.Helpers.Auth
{
    public  class CookieService : ICookieService
    {
        private readonly IWebHostEnvironment _env;
        public CookieService(IWebHostEnvironment env)
        {
            _env = env;
        }
        
        public  void SetJwtCookie(HttpResponse response, string token)
        {
            CookieOptions jwtCookieOptions = new()
            {
                HttpOnly = true,
                Secure = _env.IsProduction(),
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };
            response.Cookies.Append("accessToken", token, jwtCookieOptions);
        }
        public  void SetRefreshTokenCookie(HttpResponse response, string token)
        {
            if (string.IsNullOrEmpty(token))
                return;
            CookieOptions refreshTokenCookieOptions = new()
            {
                HttpOnly = true,
                Secure = _env.IsProduction(),
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            response.Cookies.Append("refreshToken", token, refreshTokenCookieOptions);
 
        }
         public  void DeleteRefreshTokenCookie(HttpResponse response)
        {
            response.Cookies.Delete("refreshToken");
        }
        public  void DeleteJwtCookie(HttpResponse response)
        {
    
            response.Cookies.Delete("accessToken");
        }
       
    }
}