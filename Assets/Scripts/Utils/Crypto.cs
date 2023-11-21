using System;
using System.Security.Cryptography;
using System.Text;

namespace Utils
{
    public class Crypto
    {
        public static string EncryptionSHA256(string str)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            var source = Encoding.Default.GetBytes(str);
            var crypto = sha256.ComputeHash(source);
            var result = Convert.ToBase64String(crypto);
        
            return result;
        }
    
        public static string EncryptionAes(string key, string str)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toEncryptArray = Encoding.UTF8.GetBytes(str);
        
            var rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            var cTransform = rDel.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    
        public static string DecryptionAes(string key, string str)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toEncryptArray = Convert.FromBase64String(str);
        
            var rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            var cTransform = rDel.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}