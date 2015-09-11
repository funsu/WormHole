using System;

namespace WormHoleCore
{
	public static class Auth
	{
		static bool loggedIn = false;

		public static bool LoggedIn {
			get { return loggedIn; } 
		}
	}
}

