using StatisticsAPI.Models;

namespace StatisticsAPI.Repositories
{
    public interface IStatisticsUnitRepository
    {
        Task<IEnumerable<StatisticsUnit>> GetStatisticsUnits();
        Task<IEnumerable<StatisticsUnit>> GetStatisticsUnitsAtTime(long from, long to, int? gameId);
        Task<StatisticsUnit> GetStatisticsUnit(long timestamp, int gameId);
        Task<StatisticsUnit> AddStatisticsUnit(StatisticsUnit statisticsUnit);
        Task<StatisticsUnit?> UpdateStatisticsUnit(StatisticsUnit statisticsUnit);
    }
}