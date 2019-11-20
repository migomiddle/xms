using Microsoft.AspNetCore.Http;
using System;

namespace Xms.Infrastructure
{
    public interface IExceptionHandler<out T> where T : Exception
    {
        void Handle(HttpContext context, Exception exception);
    }
}