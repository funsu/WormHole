using System;
using System.Net;

namespace WormHoleCore
{
	public class RequestProcesser
	{
		readonly HttpListenerContext originalContext;

		public RequestProcesser (HttpListenerContext context)
		{
			this.originalContext = context;
//			Console.WriteLine (
//				string.Format ("{0} {1}", 
//					context.Request.HttpMethod, 
//					context.Request.Headers));
			
		}

		public void processRequest ()
		{
			if (UrlUtility.LocationChecker.urlInChina (originalContext.Request.RawUrl)) {
				new LocalRelay (originalContext);
			} else if (Auth.LoggedIn) {
				new RemoteRelay (originalContext);
			} else {
				new LocalRelay (originalContext);
			}
		}
	}
}

