using System;
using System.Drawing;
using Xms.Infrastructure.Utility;
using Xms.Session;

namespace Xms.Security.Verify
{
    /// <summary>
    /// 图片验证码提供者
    /// </summary>
    public class ImageTextVerifyProvider : IVerifyProvider
    {
        private readonly ISessionService _sessionService;
        private readonly Randoms _randoms;
        private readonly RandomImageGenernator _randomImageGenernator;
        private const string NAME = "verifycode";

        public ImageTextVerifyProvider(ISessionService sessionService)
        {
            _sessionService = sessionService;
            _randoms = new Randoms();
            _randomImageGenernator = new RandomImageGenernator();
        }

        public VerifyValue Get(Action<VerifyImageOptions> setupAction = null)
        {
            VerifyImageOptions options = new VerifyImageOptions();
            if (setupAction != null)
            {
                setupAction.Invoke(options);
            }
            //生成随机字符
            string verifyValue = _randoms.CreateRandomValue(options.CodeCount, options.OnlyNumeric).ToLower();
            //生成图片
            RandomImage verifyImage = _randomImageGenernator.CreateRandomImage(verifyValue, options.ImageWidth, options.ImageHeight, Color.White, Color.Blue, Color.DarkRed);
            //保存到session中
            _sessionService.Set(NAME, verifyValue, 5);
            return new VerifyValue { Value = verifyImage.Image, MediaType = verifyImage.ContentType };
        }

        public bool IsValid(string key)
        {
            var result = key.IsCaseInsensitiveEqual(_sessionService.GetValue(NAME));
            //清除缓存值
            _sessionService.Remove(NAME);
            return result;
        }
    }

    public class VerifyImageOptions
    {
        public int CodeCount { get; set; } = 4;
        public bool OnlyNumeric { get; set; } = false;
        public int ImageWidth { get; set; } = 56;
        public int ImageHeight { get; set; } = 20;
    }
}