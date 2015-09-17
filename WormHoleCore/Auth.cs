using System;
using System.Security.Cryptography;

namespace WormHoleCore
{
	public static class Auth
	{
		public static bool LoggedIn = false;
		public static string serverPubKey;
		public static string clientPrivateKey;
		static Auth ()
		{
		}

	}
}

