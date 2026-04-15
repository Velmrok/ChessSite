namespace backend.Helpers.Auth
{
    public static class CookieService
    {
        public static void SetJwtCookie(HttpResponse response, string token)
        {
            response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });
        }
    }
}