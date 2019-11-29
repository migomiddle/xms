using System;

namespace Xms.Web.Framework.Models
{
    /// <summary>
    /// 执行结果
    /// </summary>
    public class ExecuteResult
    {
        public ExecuteResult()
        {
        }

        public ExecuteResult(bool isSuccess, object data = null, int statusCode = 200, string errorMessage = null)
        {
            this.IsSuccess = isSuccess;
            this.Data = data;
            this.StatusCode = statusCode;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; } = null;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; } = null;

        /// <summary>
        /// 状态码
        /// </summary>
        public int StatusCode { get; set; }

        public static ExecuteResult Failure(string message, int statusCode = 400)
        {
            return new ExecuteResult(false, null, statusCode, message);
        }

        public static ExecuteResult Success(object data = null, int statusCode = 200)
        {
            return new ExecuteResult(true, data, statusCode);
        }

        internal string SerializeToJson()
        {
            throw new NotImplementedException();
        }
    }
}