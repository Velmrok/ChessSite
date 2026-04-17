namespace backend.Services.Helpers.Auth
{
    public static class CookieService
    {
        static CookieOptions refreshTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        static CookieOptions jwtCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        
        public static void SetJwtCookie(HttpResponse response, string token)
        {
            response.Cookies.Append("accessToken", token, jwtCookieOptions);
        }
        public static void SetRefreshTokenCookie(HttpResponse response, string token)
        {
            if (string.IsNullOrEmpty(token))
                return;
            response.Cookies.Append("refreshToken", token, refreshTokenCookieOptions);
 
        }
         public static void DeleteRefreshTokenCookie(HttpResponse response)
        {
            response.Cookies.Delete("refreshToken", refreshTokenCookieOptions);
        }
        public static void DeleteJwtCookie(HttpResponse response)
        {
    
            response.Cookies.Delete("accessToken", jwtCookieOptions);
        }
       
    }
}