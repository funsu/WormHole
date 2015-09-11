using System;
using System.Linq;

namespace WormHoleCore
{
	public static class HeaderUtil
	{
		static readonly string[] hopByHopHeaders = {
			"Connection", "Keep-Alive", 
			"Public", "Proxy-Authenticate", 
			"Transfer-Encoding", "Upgrade"
		};
		static readonly string[] restricteedHeaders = {
			"Accept", "Connection", "Content-Length", 
			"Content-Type", "Date", 
			"Except", "Host", "If-Modified-Since", 
			"Range", "Referer", 
			"Transfer-Encoding", "User-Agent", 
			"Proxy-Connection"
		};

		public static bool validRelayHeader (string header)
		{
			return restricteedHeaders.All (h => h.ToLower () != header.ToLower ()) &&
			hopByHopHeaders.All (h => h.ToLower () != header.ToLower ());
		}

		public static bool isEndToEndHeader (string header)
		{
			return hopByHopHeaders.All (h => h.ToLower () != header.ToLower ());
		}
	}
}

