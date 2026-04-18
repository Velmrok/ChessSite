namespace backend.Services.Helpers.Auth
{
    public static class CookieService
    {
      
        
        public static void SetJwtCookie(HttpResponse response, string token)
        {
            CookieOptions jwtCookieOptions = new()
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };
            response.Cookies.Append("accessToken", token, jwtCookieOptions);
        }
        public static void SetRefreshTokenCookie(HttpResponse response, string token)
        {
            if (string.IsNullOrEmpty(token))
                return;
            CookieOptions refreshTokenCookieOptions = new()
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            response.Cookies.Append("refreshToken", token, refreshTokenCookieOptions);
 
        }
         public static void DeleteRefreshTokenCookie(HttpResponse response)
        {
            response.Cookies.Delete("refreshToken");
        }
        public static void DeleteJwtCookie(HttpResponse response)
        {
    
            response.Cookies.Delete("accessToken");
        }
       
    }
}