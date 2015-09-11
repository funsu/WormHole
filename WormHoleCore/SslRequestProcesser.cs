using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;

namespace WormHoleCore
{
	public class SslRequestProcesser
	{
		readonly TcpClient client;
		readonly int port;
		readonly string host;
		readonly Dictionary<string, string> headers;
		Stream clientStream;

		public SslRequestProcesser (TcpClient client)
		{
			this.client = client;
			clientStream = client.GetStream ();
			//clientStream.CopyTo (Console.OpenStandardOutput ());
			string host_temp;
			try {
				getConnectRequest (clientStream, out host_temp, out headers);

				if (host_temp.Contains (':')) {
					var hosts = host_temp.Split (':');
					host_temp = hosts [0];
					int.TryParse (hosts [1], out port);
				} else
					port = 443;
				host = host_temp;
			} catch (Exception e) {
				Console.WriteLine (e);
				client.Close ();
			}

		}

		public void processRequest ()
		{
			if (host == null) {
				client.Close ();
				return;
			}
			if (UrlUtility.LocationChecker.hostInChina (host))
				localRelay ();
			else if (Auth.LoggedIn)
				remoteRelay ();
			else
				localRelay ();
				
		}

		void localRelay ()
		{
			var relayTcp = new TcpClient ();
			try {
				relayTcp.Connect (host, port);
				var relayStream = relayTcp.GetStream ();
				var welcome = new ASCIIEncoding ().GetBytes ("HTTP/1.0 200 Connection established\r\n\r\n");
				foreach (var b in welcome)
					clientStream.WriteByte (b);
				clientStream.Flush ();
				//clientStream.CopyTo (Console.OpenStandardOutput ());
				clientStream.CopyToAsync (relayStream);
				relayStream.CopyToAsync (clientStream);
			} catch (Exception e) {
				Console.WriteLine (e);
				relayTcp.Close ();
				client.Close ();
			}
		}

		void remoteRelay ()
		{
			//clientStream.read
		}

		void getConnectRequest (Stream clientStream, 
		                        out string host, out Dictionary<string, string> headers)
		{
			var sr = new StreamReader (clientStream);
			var requestLine = sr.ReadLine ();
			host = requestLine.Split () [1];
			var headerLines = new List<string> ();
			var line = sr.ReadLine ();
			while (!string.IsNullOrEmpty (line)) {
				headerLines.Add (line);
				line = sr.ReadLine ();
			}
			headers = headerLines
				.Select (l => l.Split (new char[] { ':' }, 2))
				.ToDictionary (a => a [0], a => a [1] [0] == ' ' ? a [1].Substring (1) : a [1]);
		}
	}
}

