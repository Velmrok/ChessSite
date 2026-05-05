namespace backend.Services.Interfaces
{
    public interface IGameTimerService
    {
        void ScheduleTimeout(string gameId, string playerId, int remainingTimeMs);
        void CancelTimeout(string gameId);
        void RemoveGame(string gameId);
    }
}