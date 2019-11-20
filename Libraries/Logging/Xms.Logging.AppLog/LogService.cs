using System;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Logging.Abstractions;
using Xms.Logging.AppLog.Data;

namespace Xms.Logging.AppLog
{
    /// <summary>
    /// 系统日志服务
    /// </summary>
    public class LogService : ILogService
    {
        private readonly IAppLogRepository _AppLogRepository;
        private readonly IAppContext _appContext;

        public LogService(IAppContext appContext
            , IAppLogRepository AppLogRepository)
        {
            _appContext = appContext;
            _AppLogRepository = AppLogRepository;
        }

        public bool Create(Domain.VisitedLog entity)
        {
            if (_AppLogRepository == null)
            {
                return false;
            }

            return _AppLogRepository.Create(entity);
        }

        public bool Create(LogLevel logLevel, string title, string description = "")
        {
            var entity = new Domain.VisitedLog();
            entity.LogLevel = logLevel;
            entity.Title = title;
            entity.Url = _appContext.HttpContext.GetThisPageUrl();
            entity.UrlReferrer = _appContext.HttpContext.GetUrlReferrer();
            entity.SystemUserId = _appContext.GetFeature<Identity.ICurrentUser>().SystemUserId;
            entity.StatusCode = _appContext.HttpContext.Response.StatusCode;
            entity.Description = description;
            entity.ClientIP = _appContext.HttpContext.GetClientIpAddress();
            entity.CreatedOn = DateTime.Now;
            entity.OrganizationId = _appContext.GetFeature<Identity.ICurrentUser>().OrganizationId;
            return Create(entity);
        }

        public Domain.VisitedLog FindById(Guid id)
        {
            if (_AppLogRepository == null)
            {
                return null;
            }

            return _AppLogRepository.FindById(id);
        }

        public PagedList<Domain.VisitedLog> Query(Func<QueryDescriptor<Domain.VisitedLog>, QueryDescriptor<Domain.VisitedLog>> container)
        {
            if (_AppLogRepository == null)
            {
                return null;
            }

            QueryDescriptor<Domain.VisitedLog> q = container(QueryDescriptorBuilder.Build<Domain.VisitedLog>());

            return _AppLogRepository.QueryPaged(q);
        }

        #region sync

        public bool Information(string message, Exception exception = null)
        {
            Guard.NotNull(message, nameof(message));
            return Create(LogLevel.Information, message, exception?.ToString() ?? string.Empty);
        }

        public bool Warning(string message, Exception exception = null)
        {
            Guard.NotNull(message, nameof(message));
            return Create(LogLevel.Warning, message, exception?.ToString() ?? string.Empty);
        }

        public bool Error(string message, Exception exception = null)
        {
            Guard.NotNull(message, nameof(message));
            return Create(LogLevel.Error, message, exception?.ToString() ?? string.Empty);
        }

        public bool Information(Exception exception)
        {
            Guard.NotNull(exception, nameof(exception));
            return Create(LogLevel.Information, exception.Message, exception.ToString());
        }

        public bool Warning(Exception exception)
        {
            Guard.NotNull(exception, nameof(exception));
            return Create(LogLevel.Warning, exception.Message, exception.ToString());
        }

        public bool Error(Exception exception)
        {
            Guard.NotNull(exception, nameof(exception));
            return Create(LogLevel.Error, exception.Message, exception.ToString());
        }

        public void Clear()
        {
            _AppLogRepository.Clear();
        }

        #endregion sync
    }
}