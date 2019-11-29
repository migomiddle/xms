using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Xms.Security.Verify
{
    /// <summary>
    /// 随机图片生成器
    /// </summary>
    public class RandomImageGenernator
    {
        private readonly Random _random;

        public RandomImageGenernator()
        {
            _random = new Random();
        }

        /// <summary>
        /// 创建随机图片
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="imageWidth">图片宽度</param>
        /// <param name="imageHeight">图片高度</param>
        /// <param name="imageBGColor">图片背景颜色</param>
        /// <param name="imageTextColor1">图片文字颜色</param>
        /// <param name="imageTextColor2">图片文字颜色</param>
        /// <returns>随机图片</returns>
        public RandomImage CreateRandomImage(string value, int imageWidth, int imageHeight, Color imageBGColor, Color imageTextColor1, Color imageTextColor2)
        {
            Bitmap image = new Bitmap(imageWidth, imageHeight);
            Graphics g = Graphics.FromImage(image);
            //保存图片数据
            MemoryStream stream = new MemoryStream();
            try
            {
                //生成随机生成器
                //Random random = new Random();

                //清空图片背景色
                g.Clear(imageBGColor);

                //画图片的背景噪音线
                for (int i = 0; i < 5; i++)
                {
                    int x1 = _random.Next(image.Width);
                    int x2 = _random.Next(image.Width);
                    int y1 = _random.Next(image.Height);
                    int y2 = _random.Next(image.Height);

                    g.DrawLine(new Pen(Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255))), x1, y1, x2, y2);
                }

                Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                                                                    imageTextColor1,
                                                                    imageTextColor2,
                                                                    1.2f,
                                                                    true);
                g.DrawString(value, font, brush, 2, 2);

                //画图片的前景噪音点
                for (int i = 0; i < 80; i++)
                {
                    int x = _random.Next(image.Width);
                    int y = _random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(_random.Next()));
                }

                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                RandomImage verifyImage = new RandomImage();
                image.Save(stream, ImageFormat.Jpeg);
                verifyImage.Image = stream.ToArray();
                verifyImage.ContentType = "image/jpeg";

                return verifyImage;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }

                if (g != null)
                {
                    g.Dispose();
                }

                if (image != null)
                {
                    image.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// 随机图片
    /// </summary>
    public class RandomImage
    {
        /// <summary>
        /// 图片输出类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public byte[] Image { get; set; }
    }
}