using System.Linq;
using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Data.Services
{
    public interface ILogService
    {
        IQueryable<Log> GetAllLogs();
        IQueryable<Log> GetAllLogsByLevel(LogLevel[] logLevels);
        Log GetLogById(int logId);
        void SaveLog(Log log);
        void SaveLog<TType>(LogProviderBase<TType> logProvider);
        void DeleteLog(Log log);
    }
}