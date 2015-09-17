using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WormHoleCore
{
	public static class Security
	{
		static string aesKey = "ABCDEFGHABCDEFGH";
		static string initVector = "ABCDEFGHABCDEFGH";

		static Security ()
		{

		}

		public static Stream encryptStream(Stream originalStream){
			var aes = new RijndaelManaged();
			aes.BlockSize = 128;
			aes.KeySize = 128;
			aes.Key = Encoding.ASCII.GetBytes (aesKey);
			aes.IV = Encoding.ASCII.GetBytes (initVector);

			var encrypter = aes.CreateEncryptor ();
			var encryptStream = new CryptoStream (originalStream, encrypter, CryptoStreamMode.Read);
			return encryptStream;
		}

		public static Stream decryptStream(Stream encryptedStream){
			var aes = new RijndaelManaged();
			aes.BlockSize = 128;
			aes.KeySize = 128;
			aes.Key = Encoding.ASCII.GetBytes (aesKey);
			aes.IV = Encoding.ASCII.GetBytes (initVector);

			var decrypter = aes.CreateDecryptor ();
			var decryptStream = new CryptoStream (encryptedStream, decrypter, CryptoStreamMode.Read);
			return decryptStream;
		}
	}
}

