using System;

namespace Xms.Security.Verify
{
    public interface IVerifyProvider
    {
        VerifyValue Get(Action<VerifyImageOptions> setupAction = null);

        bool IsValid(string key);
    }

    public class VerifyValue
    {
        public byte[] Value { get; set; }
        public string MediaType { get; set; }
    }
}