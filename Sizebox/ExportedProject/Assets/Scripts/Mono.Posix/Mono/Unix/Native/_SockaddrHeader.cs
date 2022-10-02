namespace Mono.Unix.Native
{
	[Map]
	internal struct _SockaddrHeader
	{
		internal SockaddrType type;

		internal UnixAddressFamily sa_family;
	}
}
