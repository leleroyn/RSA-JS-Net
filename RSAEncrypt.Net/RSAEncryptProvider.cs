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

        private RSACryptoServiceProvider rsa = null;
        /// <summary>
        /// 实例名称
        /// </summary>
        public string InstanceName
        {
            get;
            private set;
        }

        #region 构造函数
        private RSAEncryptProvider() { }
        public RSAEncryptProvider(string instanceName)
            : this(instanceName, HttpContext.Current.Session.SessionID)
        {
        }
        public RSAEncryptProvider(string instanceName, string sessionId)
            : this(instanceName, HttpContext.Current.Session.SessionID, 5)
        {
        }
        public RSAEncryptProvider(string instanceName, string sessionId, int expireMinutes)
        {
            InstanceName = string.Join("#", sessionId, instanceName);
            if (HttpRuntime.Cache.Get(InstanceName) != null)
            {
                rsa = HttpRuntime.Cache.Get(InstanceName) as RSACryptoServiceProvider;
            }
            else
            {
                lock (lockobj)
                {
                    rsa = new RSACryptoServiceProvider();
                    HttpRuntime.Cache.Insert(InstanceName, rsa, null, DateTime.Now.AddMinutes(expireMinutes), System.Web.Caching.Cache.NoSlidingExpiration);
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
        public string Decrypt(string secretStr, bool complete = true)
        {
            byte[] result = rsa.Decrypt(HexStringToBytes(secretStr), false);
            System.Text.ASCIIEncoding enc = new ASCIIEncoding();
            if (complete)
            {
                this.Complete();
            }
            return enc.GetString(result);
        }

        public void Complete()
        {
            lock (lockobj)
            {
                HttpRuntime.Cache.Remove(InstanceName);
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
