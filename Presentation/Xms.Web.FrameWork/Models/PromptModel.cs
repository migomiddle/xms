namespace Xms.Web.Framework.Models
{
    public class PromptModel
    {
        public PromptModel(string returnUrl, string message)
        {
            ReturnUrl = returnUrl;
            Message = message;
        }

        public string ReturnUrl { get; set; }
        public string Message { get; set; }
    }
}