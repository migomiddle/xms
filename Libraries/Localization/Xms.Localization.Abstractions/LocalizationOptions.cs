using Microsoft.Extensions.Options;

namespace Xms.Localization.Abstractions
{
    /// <summary>
    /// 本地化服务参数信息
    /// </summary>
    public class LocalizationOptions : IOptions<LocalizationOptions>
    {
        public LocalizationOptions Value => this;

        /// <summary>
        /// 资源类型，xml/db
        /// </summary>
        public LocalizationSourceType Source { get; set; }

        /// <summary>
        /// 资源文件夹路径, 如果资源类型是xml，需提供此属性值
        /// </summary>
        public string FilePath { get; set; }
    }

    public enum LocalizationSourceType
    {
        Xml
            , Db
    }
}