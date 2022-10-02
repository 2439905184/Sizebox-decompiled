using System;

namespace Mono.Unix.Native
{
	[Map("struct linger")]
	[CLSCompliant(false)]
	public struct Linger
	{
		public int l_onoff;

		public int l_linger;

		public override string ToString()
		{
			return string.Format("{0}, {1}", l_onoff, l_linger);
		}
	}
}
