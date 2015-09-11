using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace WormHoleCore
{
	public class WormHoleLocalServer
	{
		int port;
		int sslPort;
		HttpListener httpListener;
		TcpListener tcpListener;
		Thread getContextThread;
		Thread acceptSslThread;

		public WormHoleLocalServer ()
		{
			this.port = 8080;
			this.sslPort = 8443;
		}

		public int Port {
			get { return this.port; }
			set {
				if (value < 0 || value > 65535) {
					throw new Exception ("Invalid port number!");
				}
				this.port = value;
			}
		}

		public int SslPort {
			get { return this.sslPort; }
			set {
				if (value < 0 || value > 65535) {
					throw new Exception ("Invalid port number!");
				}
				this.sslPort = value;
			}
		}


		public void startServer ()
		{
			//start http proxy
			httpListener = new HttpListener ();
			httpListener.Prefixes.Add (string.Format ("http://*:{0}/", port));
			httpListener.Start ();
			getContextThread = new Thread (new ThreadStart (processLoop));
			getContextThread.Start ();
			//start ssl proxy
			tcpListener = new TcpListener (IPAddress.Parse ("0.0.0.0"), sslPort);
			tcpListener.Start ();
			acceptSslThread = new Thread (new ThreadStart (processSslLoop));
			acceptSslThread.Start ();

		}

		public void stopServer ()
		{
			httpListener.Stop ();
			tcpListener.Stop ();
			getContextThread.Abort ();
			acceptSslThread.Abort ();
		}

		void processLoop ()
		{
			while (true) {
				var context = httpListener.GetContext ();
				new Thread (new ThreadStart (new RequestProcesser (context).processRequest)).Start ();
			}
		}
		//万一客户端或者服务端关闭连接怎么办

		void processSslLoop ()
		{
			while (true) {
				var client = tcpListener.AcceptTcpClient ();
				new Thread (new ThreadStart (new SslRequestProcesser (client).processRequest)).Start ();
			}
		}
	}
		
}

