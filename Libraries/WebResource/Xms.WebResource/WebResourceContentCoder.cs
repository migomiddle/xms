using System;
using System.Text;
using Xms.Infrastructure.Utility;

namespace Xms.WebResource
{
    /// <summary>
    /// Web资源内容编码/解码器
    /// </summary>
    public class WebResourceContentCoder : IWebResourceContentCoder
    {
        public string CodeDecode(string content)
        {
            if (content.IsEmpty())
            {
                return content;
            }
            byte[] outputb = Convert.FromBase64String(content);
            return Encoding.UTF8.GetString(outputb);
        }

        public string CodeEncode(string content)
        {
            if (content.IsEmpty())
            {
                return content;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return this.CodeEncode(bytes);
        }

        public string CodeEncode(byte[] bytes)
        {
            if (bytes != null && bytes.Length > 0)
            {
                return Convert.ToBase64String(bytes);
            }
            return null;
        }

        public byte[] DecodeToByte(string content)
        {
            if (content.IsNotEmpty())
            {
                return Convert.FromBase64String(content);
            }
            return null;
        }
    }
}