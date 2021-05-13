using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace broadcast
{
    class Des
    {
        public static byte[] Encrypt(byte[] srcEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.UTF8.GetBytes(sKey);
            des.IV = ASCIIEncoding.UTF8.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(srcEncrypt, 0, srcEncrypt.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }


        public static byte[] Decrypt(byte[] strDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            des.Key = ASCIIEncoding.UTF8.GetBytes(sKey);
            des.IV = ASCIIEncoding.UTF8.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(strDecrypt, 0, strDecrypt.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }
    }
}
