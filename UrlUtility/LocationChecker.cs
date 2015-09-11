using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace UrlUtility
{
	public static class LocationChecker
	{
		static Dictionary<string, bool> locationCache = new Dictionary<string, bool> ();
		static List<IPAddress[]> cnIpRange = new List<IPAddress[]> ();

		static LocationChecker ()
		{
			var cnIpFile = File.ReadAllLines ("/Users/fangsu/Downloads/cn.csv");
			foreach (var line in cnIpFile) {
				var ip = line.Split (',');
				if (ip.Length < 2)
					continue;
				cnIpRange.Add (new IPAddress[2]{ IPAddress.Parse (ip [0]), IPAddress.Parse (ip [1]) });
			}
		}

		static void setLocationCache (string host, bool inChina)
		{
			locationCache.Add (host, inChina);
		}

		public static bool urlInChina (string url)
		{
			var uri = new Uri (url);
			var host = uri.Host;
			return hostInChina (host);
		}

		public static bool hostInChina (string host)
		{
			lock (locationCache) {
				if (locationCache.ContainsKey (host))
					return locationCache [host];
			
				IPAddress address;
				var addresses = new List<IPAddress> ();
				if (IPAddress.TryParse (host, out address)) {
					addresses.Add (address);
				} else {
					try {
						var ips = Dns.GetHostAddresses (host);
						foreach (var ip in ips)
							addresses.Add (ip);
					} catch (Exception e) {
						setLocationCache (host, false);
						return false;
					}
				}

				if (addresses.Count < 1) {
					setLocationCache (host, false);
					return false;
				}

				foreach (var addr in addresses) {
					if (!ipInChina (addr)) {
						setLocationCache (host, false);
						return false;
					}
				}
				setLocationCache (host, true);
				return true;
			}
		}

		static bool ipInChina (IPAddress ipAddress)
		{
			//only support IPv4
			if (ipAddress.AddressFamily == AddressFamily.InterNetwork) {
				foreach (var range in cnIpRange) {
					if (ipInRange (ipAddress, range))
						return true;
				}
			}
			return false;
		}

		private static bool ipInRange (IPAddress ip, IPAddress[] range)
		{
			//Console.Write (ip.ToString ()+" "+range[0].ToString ()+" "+range[1].ToString ());
			var ipBytes = ip.GetAddressBytes ();
			var lowBytes = range [0].GetAddressBytes ();
			var hithBytes = range [1].GetAddressBytes ();
			return ipInRangeIter (0, ipBytes, lowBytes, hithBytes);
		}

		private static bool ipInRangeIter (int i, byte[] ip, byte[] low, byte[] high)
		{
			if (i > 3)
				return false;
			if (ip [i] < low [i] || ip [i] > high [i])
				return false;
			else if (ip [i] > low [i] && ip [i] < high [i])
				return true;
			else
				return ipInRangeIter (i + 1, ip, low, high);
		}
	}
}

