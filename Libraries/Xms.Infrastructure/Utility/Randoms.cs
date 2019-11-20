using System;
using System.Text;

namespace Xms.Infrastructure.Utility
{
    /// <summary>
    /// 随机实现类
    /// </summary>
    public class Randoms
    {
        private readonly Random _random;

        public Randoms()
        {
            _random = new Random();
        }

        /// <summary>
        /// 随机字符
        /// </summary>
        public char[] RandomLibrary { get; set; } = "123456789abcdefghjkmnpqrstuvwxyz".ToCharArray();

        /// <summary>
        /// 创建随机值
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="onlyNumber">是否只包含数字</param>
        /// <returns>随机值</returns>
        public string CreateRandomValue(int length, bool onlyNumber)
        {
            int index;
            StringBuilder randomValue = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                if (onlyNumber)
                {
                    index = _random.Next(0, 9);
                }
                else
                {
                    index = _random.Next(0, RandomLibrary.Length);
                }

                randomValue.Append(RandomLibrary[index]);
            }

            return randomValue.ToString();
        }

        /// <summary>
        /// 创建随机对
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="onlyNumber">是否只包含数字</param>
        /// <param name="randomKey">随机键</param>
        /// <param name="randomValue">随机值</param>
        public void CreateRandomPair(int length, bool onlyNumber, out string randomKey, out string randomValue)
        {
            StringBuilder randomKeySB = new StringBuilder();
            StringBuilder randomValueSB = new StringBuilder();

            int index1;
            int index2;
            for (int i = 0; i < length; i++)
            {
                if (onlyNumber)
                {
                    index1 = _random.Next(0, 10);
                    index2 = _random.Next(0, 10);
                }
                else
                {
                    index1 = _random.Next(0, RandomLibrary.Length);
                    index2 = _random.Next(0, RandomLibrary.Length);
                }

                randomKeySB.Append(RandomLibrary[index1]);
                randomValueSB.Append(RandomLibrary[index2]);
            }
            randomKey = randomKeySB.ToString();
            randomValue = randomValueSB.ToString();
        }
    }
}