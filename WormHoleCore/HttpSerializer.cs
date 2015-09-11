﻿using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.Runtime.Remoting.Messaging;
using System.IO;

namespace WormHoleCore
{
	public static class HttpSerializer
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
			var httpRequest = new HttpRequest ();
			httpRequest.rid = getRid ();
			httpRequest.method = request.HttpMethod;
			httpRequest.url = request.RawUrl;
			var headers = new Dictionary<string, string> ();
			foreach (var h in request.Headers.AllKeys) {
				if (HeaderUtil.isEndToEndHeader (h))
					headers.Add (h, request.Headers [h]);
			}
			httpRequest.headers = headers;
			if (request.HasEntityBody) {
				var body = new MemoryStream ();
				request.InputStream.CopyTo (body);
				httpRequest.body = body.ToArray ();
			}
			return JsonConvert.SerializeObject (httpRequest);
		}

		public static void deserializeResponse (string response, HttpListenerResponse originalResponse)
		{
			var httpResponse = JsonConvert.DeserializeObject<HttpRespons> (response);
			if (relayIdSet.Contains (httpResponse.rid))
				relayIdSet.Remove (httpResponse.rid);
			originalResponse.StatusCode = httpResponse.statusCode;
			foreach (var h in httpResponse.headers) {
				if (HeaderUtil.isEndToEndHeader (h.Key))
					originalResponse.AddHeader (h.Key, h.Value);
			}
			originalResponse.OutputStream.Write (httpResponse.body, 0, httpResponse.body.Length);
		}

		class HttpRequest
		{
			public int rid;
			public string method;
			public string url;
			public Dictionary<string, string> headers;
			public byte[] body;
		}

		class HttpRespons
		{
			public int rid = 0;
			public int statusCode = 0;
			public Dictionary<string, string> headers = null;
			public byte[] body = null;
		}
	}
}

