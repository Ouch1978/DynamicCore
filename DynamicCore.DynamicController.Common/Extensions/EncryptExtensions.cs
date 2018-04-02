using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DynamicCore.DynamicController.Common.Models;

namespace DynamicCore.DynamicController.Common.Extensions
{
    public static class EncryptExtensions
    {
        public static string Encrypt( this string input , string key = "" , string iv = "" , EncryptionAlgorithms algorithm = EncryptionAlgorithms.AES )
        {
            switch( algorithm )
            {
                case EncryptionAlgorithms.AES:
                    return input.EncryptAes( key , iv );
                case EncryptionAlgorithms.DES:
                    return input.EncryptDes( key , iv );
            }

            throw new Exception( "Algorithm error." );
        }

        public static string Decrypt( this string input , string key = "" , string iv = "" , EncryptionAlgorithms algorithm = EncryptionAlgorithms.AES )
        {
            switch( algorithm )
            {
                case EncryptionAlgorithms.AES:
                    return input.DecryptAes( key , iv );
                case EncryptionAlgorithms.DES:
                    return input.DecryptDes( key , iv );
            }

            throw new Exception( "Algorithm error." );
        }

        private static string EncryptAes( this string input , string key = "" , string iv = "" )
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();

            rijndaelManaged.Mode = CipherMode.CBC;
            rijndaelManaged.Padding = PaddingMode.PKCS7;
            rijndaelManaged.KeySize = 128;
            rijndaelManaged.BlockSize = 128;

            byte[] keyBytes = Encoding.UTF8.GetBytes( key );
            byte[] defaultKeyBytes = new byte[ 16 ];

            int keyLength = keyBytes.Length;

            if( keyLength > defaultKeyBytes.Length )
            {
                keyLength = defaultKeyBytes.Length;
            }

            Array.Copy( keyBytes , defaultKeyBytes , keyLength );

            rijndaelManaged.Key = defaultKeyBytes;

            byte[] ivBytes = Encoding.UTF8.GetBytes( iv );
            byte[] defaultIvBytes = new byte[ 16 ];

            int ivLength = ivBytes.Length;

            if( ivLength > defaultIvBytes.Length )
            {
                ivLength = defaultIvBytes.Length;
            }

            Array.Copy( ivBytes , defaultIvBytes , ivLength );

            rijndaelManaged.IV = ivBytes;

            ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();

            byte[] plainText = Encoding.UTF8.GetBytes( input );
            byte[] cipherBytes = cryptoTransform.TransformFinalBlock( plainText , 0 , plainText.Length );

            return Convert.ToBase64String( cipherBytes );
        }

        private static string DecryptAes( this string input , string key = "" , string iv = "" )
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();

            rijndaelManaged.Mode = CipherMode.CBC;
            rijndaelManaged.Padding = PaddingMode.PKCS7;

            rijndaelManaged.KeySize = 128;
            rijndaelManaged.BlockSize = 128;

            byte[] encryptedBytes = Convert.FromBase64String( input );

            byte[] keyBytes = Encoding.UTF8.GetBytes( key );
            byte[] defaultKeyBytes = new byte[ 16 ];

            int keyLength = keyBytes.Length;

            if( keyLength > defaultKeyBytes.Length )
            {
                keyLength = defaultKeyBytes.Length;
            }

            Array.Copy( keyBytes , defaultKeyBytes , keyLength );

            rijndaelManaged.Key = defaultKeyBytes;

            byte[] ivBytes = Encoding.UTF8.GetBytes( iv );
            byte[] defaultIvBytes = new byte[ 16 ];

            int ivLength = ivBytes.Length;

            if( ivLength > defaultIvBytes.Length )
            {
                ivLength = defaultIvBytes.Length;
            }

            Array.Copy( ivBytes , defaultIvBytes , ivLength );

            rijndaelManaged.IV = defaultIvBytes;

            ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();

            byte[] plainText = cryptoTransform.TransformFinalBlock( encryptedBytes , 0 , encryptedBytes.Length );
            return Encoding.UTF8.GetString( plainText );
        }

        private static string EncryptDes( this string input , string key = "" , string iv = "" )
        {
            DESCryptoServiceProvider desCryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes( input );
            desCryptoServiceProvider.Key = Encoding.ASCII.GetBytes( key );
            desCryptoServiceProvider.IV = Encoding.ASCII.GetBytes( iv );
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream( memoryStream , desCryptoServiceProvider.CreateEncryptor() , CryptoStreamMode.Write );

            cryptoStream.Write( inputByteArray , 0 , inputByteArray.Length );

            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String( memoryStream.ToArray() );
        }

        private static string DecryptDes( this string input , string key = "" , string iv = "" )
        {
            DESCryptoServiceProvider desCryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] inputByteArray = Convert.FromBase64String( input );
            desCryptoServiceProvider.Key = Encoding.ASCII.GetBytes( key );
            desCryptoServiceProvider.IV = Encoding.ASCII.GetBytes( iv );
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream( memoryStream , desCryptoServiceProvider.CreateDecryptor() , CryptoStreamMode.Write );

            cryptoStream.Write( inputByteArray , 0 , inputByteArray.Length );
            cryptoStream.FlushFinalBlock();

            return Encoding.Default.GetString( memoryStream.ToArray() );
        }

        public static string CalculateMd5Hash( this string input )
        {
            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes( input );
            byte[] hash = md5.ComputeHash( inputBytes );

            StringBuilder stringBuilder = new StringBuilder();

            for( int i = 0 ; i < hash.Length ; i++ )
            {
                stringBuilder.Append( hash[ i ].ToString( "x2" ) );
            }

            return stringBuilder.ToString();
        }

    }
}
