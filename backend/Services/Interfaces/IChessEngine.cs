using backend.Services.Interfaces;
using ErrorOr;

namespace backend.Services.Interfaces
{
    public interface IChessEngine
    {
        public Task<ErrorOr<string>> GetMoveAsync(string fen, int thinkTimeMs);
    }
}


public class TestChessEngine : IChessEngine
{
    public async Task<ErrorOr<string>> GetMoveAsync(string fen, int thinkTimeMs)
    {
        await Task.Delay(thinkTimeMs);
        return "e5";
    }
}