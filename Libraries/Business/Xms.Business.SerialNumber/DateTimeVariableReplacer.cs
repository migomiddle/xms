using System;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 日期时间变量替换器
    /// </summary>
    public class DateTimeVariableReplacer : IVariableReplacer
    {
        public DateTimeVariableReplacer()
        {
        }

        public string Replace(string text)
        {
            text = text.Replace("{yy}", DateTime.Now.ToString("yy"));//replace year var
            text = text.Replace("{yyyy}", DateTime.Now.ToString("yyyy"));//replace year var
            text = text.Replace("{mm}", DateTime.Now.ToString("mm"));//replace month var
            text = text.Replace("{dd}", DateTime.Now.ToString("dd"));//replace day var
            return text;
        }
    }
}