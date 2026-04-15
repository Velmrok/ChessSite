namespace backend.Services.Helpers.Auth
{
    public static class CookieService
    {
        public static void SetJwtCookie(HttpResponse response, string token)
        {
            response.Cookies.Append("accessToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });
        }
        public static void SetRefreshTokenCookie(HttpResponse response, string token)
        {
            if (string.IsNullOrEmpty(token))
                return;
            response.Cookies.Append("refreshToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
         public static void DeleteRefreshTokenCookie(HttpResponse response)
        {
            response.Cookies.Delete("refreshToken");
        }
    }
}