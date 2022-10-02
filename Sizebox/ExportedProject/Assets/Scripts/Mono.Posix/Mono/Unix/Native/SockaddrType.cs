namespace Mono.Unix.Native
{
	[Map]
	internal enum SockaddrType
	{
		Invalid = 0,
		SockaddrStorage = 1,
		SockaddrUn = 2,
		Sockaddr = 3,
		SockaddrIn = 4,
		SockaddrIn6 = 5,
		MustBeWrapped = 32768
	}
}
