using System;
using Xms.Core.Context;
using Xms.Logging.Abstractions;

namespace Xms.Logging.AppLog
{
    public interface ILogService
    {
        bool Create(Domain.VisitedLog entity);

        bool Create(LogLevel logLevel, string title, string description = "");

        bool Error(string message, Exception exception = null);

        bool Error(Exception exception);

        Domain.VisitedLog FindById(Guid id);

        bool Information(string message, Exception exception = null);

        bool Information(Exception exception);

        PagedList<Domain.VisitedLog> Query(Func<QueryDescriptor<Domain.VisitedLog>, QueryDescriptor<Domain.VisitedLog>> container);

        bool Warning(string message, Exception exception = null);

        bool Warning(Exception exception);
    }
}