using System;

namespace Mono.Unix.Native
{
	[Map("struct timeval")]
	public struct Timeval : IEquatable<Timeval>
	{
		[time_t]
		public long tv_sec;

		[suseconds_t]
		public long tv_usec;

		public override int GetHashCode()
		{
			return tv_sec.GetHashCode() ^ tv_usec.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			Timeval timeval = (Timeval)obj;
			if (timeval.tv_sec == tv_sec)
			{
				return timeval.tv_usec == tv_usec;
			}
			return false;
		}

		public bool Equals(Timeval value)
		{
			if (value.tv_sec == tv_sec)
			{
				return value.tv_usec == tv_usec;
			}
			return false;
		}

		public static bool operator ==(Timeval lhs, Timeval rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Timeval lhs, Timeval rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
