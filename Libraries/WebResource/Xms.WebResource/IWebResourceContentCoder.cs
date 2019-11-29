namespace Xms.WebResource
{
    public interface IWebResourceContentCoder
    {
        string CodeDecode(string content);

        string CodeEncode(string content);

        string CodeEncode(byte[] bytes);

        byte[] DecodeToByte(string content);
    }
}