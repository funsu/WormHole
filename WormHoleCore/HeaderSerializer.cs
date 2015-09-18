using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.Runtime.Remoting.Messaging;
using System.IO;


//需要重写
namespace WormHoleCore
{
	public static class HeaderSerializer
	{
		static HashSet<int> relayIdSet = new HashSet<int> ();
		static Random randomInt = new Random ();

		static int getRid ()
		{
			int id;
			while (true) {
				id = randomInt.Next ();
				if (!relayIdSet.Contains (id))
					break;
			}
			relayIdSet.Add (id);
			return id;
		}

		public static string serializeRequest (HttpListenerRequest request)
		{
			var headerRequest = new HeaderRequest ();
//			httpRequest.rid = getRid ();
			headerRequest.method = request.HttpMethod;
			headerRequest.url = request.RawUrl;
			var headers = new Dictionary<string, string> ();
			foreach (var h in request.Headers.AllKeys) {
				if (HeaderUtil.isEndToEndHeader (h))
					headers.Add (h, request.Headers [h]);
			}
			headerRequest.headers = headers;
			if (request.HasEntityBody) {
				var body = new MemoryStream ();
				request.InputStream.CopyTo (body);
				headerRequest.body = body.ToArray ();
			}
			return JsonConvert.SerializeObject (headerRequest);
		}

		public static void deserializeResponse (string response, HttpListenerResponse originalResponse)
		{
			var headerResponse = JsonConvert.DeserializeObject<HeaderRespons> (response);
			if (relayIdSet.Contains (headerResponse.rid))
				relayIdSet.Remove (headerResponse.rid);
			originalResponse.StatusCode = headerResponse.statusCode;
			foreach (var h in headerResponse.headers) {
				if (HeaderUtil.isEndToEndHeader (h.Key))
					originalResponse.AddHeader (h.Key, h.Value);
			}
			//originalResponse.OutputStream.Write (headerResponse.body, 0, headerResponse.body.Length);
		}

		class HeaderRequest
		{
//			public int rid;
			public string method;
			public string url;
			public Dictionary<string, string> headers;
//			public byte[] body;
		}

		class HeaderRespons
		{
			public int statusCode = 0;
			public Dictionary<string, string> headers = null;
		}
	}
}

