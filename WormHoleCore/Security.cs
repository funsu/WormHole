using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WormHoleCore
{
	public static class Security
	{
		static string aesKey = "28FB258BFCB28A557A3976928841D";
		static string initVector = "27B96249D567C";

		static Security ()
		{
			
		}

		public static Stream encryptStream(Stream originalStream){
			var aes = new RijndaelManaged();
			aes.BlockSize = 128;
			aes.KeySize = 256;
			aes.Key = Encoding.ASCII.GetBytes (aesKey);
			aes.IV = Encoding.ASCII.GetBytes (initVector);

			var encryptedStream = new MemoryStream ();
			var encrypter = aes.CreateEncryptor ();
			var encryptStream = new CryptoStream (encryptedStream, encrypter, CryptoStreamMode.Write);
			originalStream.CopyTo (encryptStream);
			return encryptedStream;
		}
		public static Stream decryptStream(Stream encryptedStream){
			var aes = new RijndaelManaged();
			aes.BlockSize = 128;
			aes.KeySize = 256;
			aes.Key = Encoding.ASCII.GetBytes (aesKey);
			aes.IV = Encoding.ASCII.GetBytes (initVector);

			var decrypedStream = new MemoryStream ();
			var decrypter = aes.CreateDecryptor ();
			var decryptStream = new CryptoStream (decrypedStream, decrypter, CryptoStreamMode.Write);
			encryptedStream.CopyTo (decryptStream);
			return decrypedStream;
		}

	}
}

