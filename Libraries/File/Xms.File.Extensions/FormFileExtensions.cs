using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xms.Configuration;
using Xms.Configuration.Domain;
using Xms.Infrastructure.Utility;

namespace Xms.File.Extensions
{
    /// <summary>
    /// IFormFile扩展方法
    /// </summary>
    public static class FormFileExtensions
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<SaveFileResult> SaveAs(this IFormFile file, string path, ISettingFinder settingFinder, IWebHelper webHelper)
        {
            SaveFileResult result = new SaveFileResult();
            if (file == null)
            {
                result.IsSuccess = false;
                result.Status = -1;
                result.Error = "file is null";
            }
            else
            {
                string fileName = file.FileName;
                string fileExt = Path.GetExtension(fileName);
                var config = settingFinder.Get<UploadSetting>();
                if (!config.FileExts.IsNotEmpty() && !config.FileExts.Split(';').Contains(fileExt, StringComparer.InvariantCultureIgnoreCase))
                {
                    result.IsSuccess = false;
                    result.Status = -2;
                    result.Error = "file extension is forbidden";
                }
                else
                {
                    long fileSize = file.Length;
                    if (fileSize > config.MaxSize * 1024)
                    {
                        result.IsSuccess = false;
                        result.Status = -3;
                        result.Error = $"file size is greater than {config.MaxSize}kb";
                    }
                }
                if (path.IsEmpty())
                {
                    string dirPath = webHelper.MapPath("/upload/attachment/");
                    string name = DateTime.Now.ToString(config.FormatName);//"yyMMddHHmmssfffffff"
                    string newFileName = name + fileExt;

                    path = dirPath + newFileName;
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream).ConfigureAwait(false);
                }
                result.IsSuccess = true;
                result.FilePath = path;
            }
            return result;
        }
    }

    public sealed class SaveFileResult
    {
        public bool IsSuccess { get; set; }
        public string FilePath { get; set; }
        public string Error { get; set; }
        public int Status { get; set; }
    }
}