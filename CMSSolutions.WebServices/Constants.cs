using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace CMSSolutions.WebServices
{
    public static class ReflectionExtensions
    {
        public static string GetAttributeDisplayName(PropertyInfo property)
        {
            var atts = property.GetCustomAttributes(
                typeof(DisplayNameAttribute), true);
            if (atts.Length == 0)
                return null;
            var displayNameAttribute = atts[0] as DisplayNameAttribute;
            if (displayNameAttribute != null)
                return displayNameAttribute.DisplayName;
            return string.Empty;
        }

        public static string Encrypt(string key, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return string.Empty;
                }

                byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);
                var hashmd5 = new MD5CryptoServiceProvider();
                byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                var tdes = new TripleDESCryptoServiceProvider
                {
                    Key = keyArray,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string Decrypt(string key, string toDecrypt)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return string.Empty;
                }

                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
                var hashmd5 = new MD5CryptoServiceProvider();
                byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                var tdes = new TripleDESCryptoServiceProvider
                {
                    Key = keyArray,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    public class Constants
    {
        public const string NotMapped = "NotMapped";

        public const string ConnectionString = "DefaultDataContext";

        public const string NamespaceSite = "http://tempuri.org/";

        public const string UrlCheckCopyright = @"CheckCopyright?domain={domain}";
        public const string UrlGetTokenKey = @"UrlGetTokenKey?domain={domain}";
    }
}