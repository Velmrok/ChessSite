using backend.Data;
using backend.DTO.Games;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class GamesService : IGamesService
    {
        private readonly AppDbContext _dbContext;

        public GamesService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<GamesResponse>> GetAllGamesAsync(GetGamesQuery query)
        {
            var games = await _dbContext.Games
                .Where(g => g.WhitePlayer.Nickname.Contains(query.Query ?? "") || g.BlackPlayer.Nickname.Contains(query.Query ?? ""))
                .Include(g => g.WhitePlayer)
                .Include(g => g.BlackPlayer)
                .Include(g => g.Winner)
                .ToListAsync();


            var totalCount = games.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.Limit);

            var response = new GamesResponse
            (
                Games: [.. games.Select(g => g.MapToGamesResponse(null))],
                TotalPages: totalPages
            );

            return response;

        }
    }
}