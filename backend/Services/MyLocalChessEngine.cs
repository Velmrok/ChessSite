using System.Net.Http.Json;
using backend.DTO.Games;
using backend.Services.Interfaces;
using ErrorOr;

namespace backend.Services;

public class MyLocalChessEngine : IChessEngine
{
    private readonly HttpClient _httpClient;

    public MyLocalChessEngine(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<string>> GetMoveAsync(string fen, int thinkTimeMs)
    {
        
        var request= new AIMoveRequest 
        (
            Fen : fen,
            ThinkTimeMs : thinkTimeMs
        );

        var response = await _httpClient.PostAsJsonAsync("/move", request);

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure("Engine.Error", $"Engine returned : {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<AIMoveResponse>();

        if (result == null || string.IsNullOrWhiteSpace(result.San))
        {
            return Error.Validation("Engine.InvalidResponse", "Empty move received from engine");
        }

        return result.San;
        
    }
    

}