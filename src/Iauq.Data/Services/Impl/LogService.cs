using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Data.Services.Impl
{
    public class LogService : ILogService
    {
        private readonly IDbSet<Log> _logs;
        private readonly IUnitOfWork _unitOfWork;

        public LogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logs = _unitOfWork.Set<Log>();
        }

        #region ILogService Members

        public IQueryable<Log> GetAllLogs()
        {
            return _logs;
        }

        public IQueryable<Log> GetAllLogsByLevel(LogLevel[] logLevels)
        {
            List<short> ll = logLevels.Cast<short>().ToList();

            return _logs.Where(l => ll.Contains(l.LevelShort));
        }

        public Log GetLogById(int logId)
        {
            return _logs.Find(logId);
        }

        public void SaveLog(Log log)
        {
            _logs.Add(log);
            _unitOfWork.SaveChanges();
        }

        public void SaveLog<TType>(LogProviderBase<TType> logProvider)
        {
            SaveLog(logProvider.Provide());
        }

        public void DeleteLog(Log log)
        {
            _logs.Remove(log);
        }

        #endregion
    }
}