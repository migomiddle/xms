namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 变量替换器接口
    /// </summary>
    public interface IVariableReplacer
    {
        string Replace(string text);
    }
}