namespace Xms.Web.Framework.Models
{
    public class JsonResultObject
    {
        public JsonResultObject()
        {
        }

        public JsonResultObject(bool isSuccess, object data = null, int statusCode = 200, string errorMessage = null)
        {
            this.IsSuccess = isSuccess;
            this.Content = data;
            this.StatusCode = statusCode;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 状态,success/error
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// 内容
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 其它数据
        /// </summary>
        public object Extra { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; } = null;

        public static JsonResultObject Failure(string message, int statusCode = 500, object data = null)
        {
            return new JsonResultObject(false, data, statusCode, message);
        }

        public static JsonResultObject Success(object data = null, int statusCode = 200)
        {
            return new JsonResultObject(true, data, statusCode);
        }
    }
}