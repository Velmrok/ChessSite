using backend.Services.Interfaces;
using ErrorOr;

namespace backend.Services.Interfaces
{
    public interface IChessEngine
    {
        public Task<ErrorOr<string>> GetMoveAsync(string fen, int thinkTimeMs);
    }
}
