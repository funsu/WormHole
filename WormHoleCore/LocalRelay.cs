using System;
using System.Net;
using System.IO;
using System.Linq;

namespace WormHoleCore
{
	public class LocalRelay
	{
		readonly HttpListenerContext originalContext;
		readonly HttpWebRequest relayRequest;

		public LocalRelay (HttpListenerContext context)
		{
			this.originalContext = context;
			relayRequest = WebRequest.CreateHttp (context.Request.RawUrl);
			foreach (var key in context.Request.Headers.AllKeys) {
				if (HeaderUtil.validRelayHeader (key)) {
					//headers.Set (key, context.Request.Headers[key]);
					relayRequest.Headers.Set (key, context.Request.Headers.Get (key));
				}
			}
			var originalRequest = context.Request;
			relayRequest.UserAgent = originalRequest.UserAgent;
			if (originalRequest.Headers.AllKeys.Any (h => h.ToLower () == "range")) {
				relayRequest.AddRange (Int32.Parse (originalRequest.Headers [originalRequest.Headers.AllKeys.First (k => k.ToLower () == "range")]));
			}

			if (originalRequest.Headers.AllKeys.Any (h => h.ToLower () == "referer")) {
//				relayRequest.AddRange (Int32.Parse (originalRequest.Headers [originalRequest.Headers.AllKeys.Where (k => k.ToLower () == "range").ToList ().First ()]));
				relayRequest.Referer = originalRequest.Headers [originalRequest.Headers.AllKeys.First (k => k.ToLower () == "referer")];
			}
			//relayRequest.Accept = originalRequest.AcceptTypes;
			relayRequest.ContentLength = originalRequest.ContentLength64;
			//relayRequest.Headers.Add (headers);
			//relayRequest.KeepAlive = false;
			relayRequest.Method = context.Request.HttpMethod;
			if (context.Request.HasEntityBody) {
				var requestStream = relayRequest.GetRequestStream ();
				context.Request.InputStream.CopyTo (requestStream);
				requestStream.Flush ();
			}
			//微软你炸
			WebResponse relayResponse;
			try {
				relayResponse = relayRequest.GetResponse ();
				originalContext.Response.StatusCode = (int)((HttpWebResponse)relayResponse).StatusCode;
			} catch (WebException e) {
				//originalContext.Response.StatusCode = (int)((HttpWebResponse)e.Response).StatusCode;
				if (e.Response != null) {
					relayResponse = e.Response;
					originalContext.Response.StatusCode = (int)((HttpWebResponse)e.Response).StatusCode;
				} else {
					context.Response.Abort ();
					return;
				}
			}
				
			foreach (var h in relayResponse.Headers.AllKeys) {
				if (HeaderUtil.isEndToEndHeader (h))
					context.Response.AddHeader (h, relayResponse.Headers.Get (h));
			}
			//context.Response.ContentType = relayResponse.ContentType;
			//context.Response.ContentLength64 = relayResponse.ContentLength;
			try {
				relayResponse.GetResponseStream ().CopyTo (context.Response.OutputStream);
				context.Response.OutputStream.Flush ();
				context.Response.OutputStream.Close ();
				context.Response.Close ();
			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
			}

			//relayRequest.
		}
	}
}

