using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace RSAEncrypt.Net
{
    public class RSAEncryptProvider
    {
        private static object lockobj = new object();

        private static RSACryptoServiceProvider rsa ;    

        #region 构造函数
        public RSAEncryptProvider() {
            lock (lockobj)
            {
                if (rsa == null)
                {
                    rsa = new RSACryptoServiceProvider();
                }
            }
        } 

        #endregion

        /// <summary>
        /// 当解密最后一个值后，complate应设为true
        /// </summary>
        /// <param name="secretStr"></param>
        /// <param name="complate"></param>
        /// <returns></returns>
        public bool TryDecrypt(string secretStr, out string realValue, bool complete = true)
        {
            try
            {
                byte[] result = rsa.Decrypt(HexStringToBytes(secretStr), false);
                System.Text.ASCIIEncoding enc = new ASCIIEncoding();
                realValue = enc.GetString(result);               
                return true;
            }
            catch (Exception)
            {
                realValue = null;
                return false;
            }
           
        }     

        public string RSAExponent
        {
            get
            {
                RSAParameters parameter = rsa.ExportParameters(true);
                return BytesToHexString(parameter.Exponent);
            }
        }

        public string RSAModulus
        {
            get
            {
                RSAParameters parameter = rsa.ExportParameters(true);
                return BytesToHexString(parameter.Modulus);
            }
        }

        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey
        {
            get { return rsa.ToXmlString(false); }
        }

        private string BytesToHexString(byte[] input)
        {
            StringBuilder hexString = new StringBuilder(64);
            for (int i = 0; i < input.Length; i++)
            {
                hexString.Append(String.Format("{0:X2}", input[i]));
            }
            return hexString.ToString();
        }

        private byte[] HexStringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[] { 0 };
            }
            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length / 2; i++)
            {
                result[i] = byte.Parse(hex.Substring(2 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return result;
        }
    }
}
