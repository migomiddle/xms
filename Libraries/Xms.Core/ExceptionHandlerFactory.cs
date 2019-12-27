using Microsoft.AspNetCore.Http;
using System;
using Xms.Infrastructure;
using Xms.Infrastructure.Inject;
using Xms.Infrastructure.Utility;

namespace Xms.Core
{
    public interface IExceptionHandlerFactory
    {
        IExceptionHandler<TException> Get<TException>(TException e) where TException : Exception;
    }

    public class ExceptionHandlerFactory : IExceptionHandlerFactory
    {
        private readonly IServiceResolver _serviceResolver;

        public ExceptionHandlerFactory(IServiceResolver serviceResolver)
        {
            _serviceResolver = serviceResolver;
        }

        public IExceptionHandler<TException> Get<TException>(TException e) where TException : Exception
        {
            var handlerType = typeof(IExceptionHandler<>).MakeGenericType(e.GetType());
            var handler = _serviceResolver.Get(handlerType);
            return (IExceptionHandler<TException>)(handler ?? new DefaultExceptionHandler());
        }
    }

    public class DefaultExceptionHandler : IExceptionHandler<Exception>
    {
        public void Handle(HttpContext context, Exception exception)
        {
            if (context.Request.Path.Value.IsCaseInsensitiveEqual("error/index"))
            {
                return;
            }

            if (context.Features.Get<Exception>() == null)
            {
                context.Features.Set(exception);
            }
            context.Request.Path = new PathString("/error/index");
        }
    }
}