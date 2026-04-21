namespace backend.Services.Interfaces;
public interface ICookieService
{
    public void SetJwtCookie(HttpResponse response, string token);
    public void SetRefreshTokenCookie(HttpResponse response, string token);
    public void DeleteRefreshTokenCookie(HttpResponse response);
    public void DeleteJwtCookie(HttpResponse response);
}