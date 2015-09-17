using System;
using System.Security.Cryptography;

namespace WormHoleCore
{
	public static class Security
	{
		public static bool LoggedIn = false;
		public static string serverPubKey;
		public static string clientPrivateKey;
		static Security ()
		{
		}

	}
}

