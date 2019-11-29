using Xms.Logging.Abstractions;

namespace Xms.Configuration.Domain
{
    public class PlatformSetting
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 应用地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 应用移动端地址
        /// </summary>
        public string MobileUrl { get; set; }

        /// <summary>
        /// 应用状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 关闭提示
        /// </summary>
        public string ClosedReason { get; set; }

        /// <summary>
        /// 图片存储地址
        /// </summary>
        public string ImageCDN { get; set; }

        /// <summary>
        /// 样式存储地址
        /// </summary>
        public string CSSCDN { get; set; }

        /// <summary>
        /// 脚本存储地址
        /// </summary>
        public string ScriptCDN { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// 按用户权限显示菜单
        /// </summary>
        public bool ShowMenuInUserPrivileges { get; set; }

        /// <summary>
        /// 应用版本号
        /// </summary>
        public string VersionNumber { get; set; }

        /// <summary>
        /// 启用访问日志
        /// </summary>
        public bool LogEnabled { get; set; }

        /// <summary>
        /// 启用数据日志
        /// </summary>
        public bool DataLogEnabled { get; set; }

        /// <summary>
        /// 启用登录验证码
        /// </summary>
        public bool VerifyCodeEnabled { get; set; }

        /// <summary>
        /// 最大提取记录数
        /// </summary>
        public int MaxFetchRecords { get; set; }

        /// <summary>
        /// 启用缓存
        /// </summary>
        public bool CacheEnabled { get; set; }
    }
}