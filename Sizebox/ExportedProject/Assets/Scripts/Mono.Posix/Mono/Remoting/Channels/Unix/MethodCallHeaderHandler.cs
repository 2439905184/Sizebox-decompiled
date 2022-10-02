using System.Runtime.Remoting.Messaging;

namespace Mono.Remoting.Channels.Unix
{
	internal class MethodCallHeaderHandler
	{
		private string _uri;

		public MethodCallHeaderHandler(string uri)
		{
			_uri = uri;
		}

		public object HandleHeaders(Header[] headers)
		{
			return _uri;
		}
	}
}
