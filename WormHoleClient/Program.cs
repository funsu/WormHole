using System;
using WormHoleCore;
using System.Threading;

namespace WormHoleClient
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!\n");
			WormHoleLocalServer simpServer = new WormHoleLocalServer ();
			simpServer.startServer ();
		}
	}
}
