using System;

namespace Xms.Infrastructure
{
    /// <summary>
    /// 平台异常
    /// </summary>
    [Serializable]
    public class XmsException : Exception
    {
        public int StatusCode { get; set; } = 500;

        public XmsException(string message)
            : base(message)
        {
        }

        public XmsException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public XmsException(Exception e) : base(e.Message, e.InnerException)
        {
        }

        public XmsException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// 无权限异常
    /// </summary>
    public class XmsUnauthorizedException : XmsException
    {
        public XmsUnauthorizedException(string message) : base(403, message)
        {
        }
    }

    public class XmsNotFoundException : XmsException
    {
        public XmsNotFoundException(string message) : base(404, message)
        {
        }
    }
}