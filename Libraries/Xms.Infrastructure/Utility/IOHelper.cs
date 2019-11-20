using System.IO;
using System.IO.Compression;

namespace Xms.Infrastructure.Utility
{
    public class IOHelper
    {
        #region zip

        /// <summary>
        /// 创建压缩文件
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destinationFile"></param>
        public static void CreateZip(string sourceDir, string destinationFile)
        {
            ZipFile.CreateFromDirectory(sourceDir, destinationFile, CompressionLevel.Fastest, false);
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipFilePath"></param>
        public static void UnZip(string zipFilePath)
        {
            var dir = Path.GetDirectoryName(zipFilePath);
            ZipFile.ExtractToDirectory(zipFilePath, dir);
        }

        #endregion zip
    }
}