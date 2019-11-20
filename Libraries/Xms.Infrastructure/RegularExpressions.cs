using System.Text.RegularExpressions;

namespace Xms.Infrastructure
{
    public static class RegularExpressions
    {
        internal static readonly string ValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
        internal static readonly string ValidIntegerPattern = "^([-]|[0-9])[0-9]*$";

        internal static readonly Regex HasTwoDot = new Regex("[0-9]*[.][0-9]*[.][0-9]*", RegexOptions.Compiled);
        internal static readonly Regex HasTwoMinus = new Regex("[0-9]*[-][0-9]*[-][0-9]*", RegexOptions.Compiled);

        public static readonly Regex IsAlpha = new Regex("[^a-zA-Z]", RegexOptions.Compiled);
        public static readonly Regex IsAlphaNumeric = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);
        public static readonly Regex IsNotNumber = new Regex("[^0-9.-]", RegexOptions.Compiled);
        public static readonly Regex IsPositiveInteger = new Regex(@"\d{1,10}", RegexOptions.Compiled);
        public static readonly Regex IsInteger = new Regex(ValidIntegerPattern, RegexOptions.Compiled);
        public static readonly Regex IsNumeric = new Regex("(" + ValidRealPattern + ")|(" + ValidIntegerPattern + ")", RegexOptions.Compiled);

        public static readonly Regex IsWebUrl = new Regex(@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", RegexOptions.Singleline | RegexOptions.Compiled);

        public static readonly Regex IsEmail = new Regex("^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex RemoveHTML = new Regex(@"<[/]{0,1}\s*(?<tag>\w*)\s*(?<attr>.*?=['""].*?[""'])*?\s*[/]{0,1}>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public static readonly Regex IsGuid = new Regex(@"\{?[a-fA-F0-9]{8}(?:-(?:[a-fA-F0-9]){4}){3}-[a-fA-F0-9]{12}\}?", RegexOptions.Compiled);
        public static readonly Regex IsBase64Guid = new Regex(@"[a-zA-Z0-9+/=]{22,24}", RegexOptions.Compiled);

        public static readonly Regex IsCultureCode = new Regex(@"^([a-z]{2})|([a-z]{2}-[A-Z]{2})$", RegexOptions.Singleline | RegexOptions.Compiled);
        public static readonly Regex IsDateTime = new Regex(@"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$", RegexOptions.Singleline | RegexOptions.Compiled);

        public static readonly Regex IsYearRange = new Regex(@"^(\d{4})-(\d{4})$", RegexOptions.Compiled);

        public static readonly Regex IsIban = new Regex(@"[a-zA-Z]{2}[0-9]{2}[a-zA-Z0-9]{4}[0-9]{7}([a-zA-Z0-9]?){0,16}", RegexOptions.Singleline | RegexOptions.Compiled);
        public static readonly Regex IsBic = new Regex(@"([a-zA-Z]{4}[a-zA-Z]{2}[a-zA-Z0-9]{2}([a-zA-Z0-9]{3})?)", RegexOptions.Singleline | RegexOptions.Compiled);

        public static readonly Regex IsZipCode = new Regex(@"\d{1,6}", RegexOptions.Compiled);
        public static readonly Regex IsPhoneNumber = new Regex(@"^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$", RegexOptions.Singleline | RegexOptions.Compiled);
        public static readonly Regex IsMobileNumber = new Regex(@"^((\(\d{3}\))|(\d{3}\-))?13[456789]\d{8}|15[89]\d{8}", RegexOptions.Singleline | RegexOptions.Compiled);
        public static readonly Regex IsIdCardNumber = new Regex(@"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$", RegexOptions.Singleline | RegexOptions.Compiled);
    }
}