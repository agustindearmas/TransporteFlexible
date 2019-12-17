using System;
using System.Security.Cryptography;
using System.Text;

namespace Negocio.Managers.Seguridad
{
    public class EncriptacionManager
    {
        private const string mysecurityKey = "agustin";

        public static string EncriptarMD5(string cadena)
        {
            try
            {
                MD5 md5 = MD5.Create();
                ASCIIEncoding encoding = new ASCIIEncoding();
                StringBuilder sb = new StringBuilder();
                byte[] stream = md5.ComputeHash(encoding.GetBytes(cadena));
                for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
                return sb.ToString();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static string EncriptarAES(string TextToEncrypt)
        {
            try
            {
                byte[] MyEncryptedArray = UTF8Encoding.UTF8.GetBytes(TextToEncrypt);
                MD5CryptoServiceProvider MyMD5CryptoService = new MD5CryptoServiceProvider();
                byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(mysecurityKey));
                MyMD5CryptoService.Clear();

                var MyTripleDESCryptoService = new TripleDESCryptoServiceProvider
                {
                    Key = MysecurityKeyArray,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                var MyCrytpoTransform = MyTripleDESCryptoService.CreateEncryptor();
                byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock(MyEncryptedArray, 0, MyEncryptedArray.Length);
                MyTripleDESCryptoService.Clear();
                return Convert.ToBase64String(MyresultArray, 0, MyresultArray.Length);
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public static string DesencriptarAES(string TextToDecrypt)
        {
            try
            {
                byte[] MyDecryptArray = Convert.FromBase64String(TextToDecrypt);
                MD5CryptoServiceProvider MyMD5CryptoService = new MD5CryptoServiceProvider();
                byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(mysecurityKey));
                MyMD5CryptoService.Clear();

                var MyTripleDESCryptoService = new TripleDESCryptoServiceProvider
                {
                    Key = MysecurityKeyArray,

                    Mode = CipherMode.ECB,

                    Padding = PaddingMode.PKCS7
                };

                var MyCrytpoTransform = MyTripleDESCryptoService.CreateDecryptor();
                byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock(MyDecryptArray, 0, MyDecryptArray.Length);
                MyTripleDESCryptoService.Clear();
                return UTF8Encoding.UTF8.GetString(MyresultArray);
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}
