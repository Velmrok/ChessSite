using System.Net.Http.Json;
using backend.DTO.Games;
using backend.Services.Interfaces;
using ErrorOr;

namespace backend.Services;

public class MyLocalChessEngine : IChessEngine
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyLocalChessEngine> _logger;

    public MyLocalChessEngine(HttpClient httpClient, ILogger<MyLocalChessEngine> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ErrorOr<string>> GetMoveAsync(string fen, int thinkTimeMs)
    {
        
        var request= new AIMoveRequest 
        (
            Fen : fen
        );

        var response = await _httpClient.PostAsJsonAsync("/move", request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Engine returned non-success status code: {StatusCode}", response.StatusCode);
            return Error.Failure("Engine.Error", $"Engine returned : {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<AIMoveResponse>();

        if (result == null || string.IsNullOrWhiteSpace(result.San))
        {
            _logger.LogWarning("Engine returned invalid response");
            return Error.Validation("Engine.InvalidResponse", "Empty move received from engine");
        }

        return result.San;
        
    }
    

}