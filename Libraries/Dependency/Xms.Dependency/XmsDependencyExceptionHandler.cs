using Microsoft.AspNetCore.Http;
using System;
using Xms.Dependency.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;

namespace Xms.Dependency
{
    public class XmsDependencyExceptionHandler : IExceptionHandler<XmsDependencyException>
    {
        public void Handle(HttpContext context, Exception exception)
        {
            context.Request.Body = (exception as XmsDependencyException).Dependents.SerializeToJson().ToStream();
            context.Request.Path = new PathString("/error/dependentexception");
        }
    }
}