using System;

namespace Mono.Unix.Native
{
	[Map("struct timespec")]
	public struct Timespec : IEquatable<Timespec>
	{
		[time_t]
		public long tv_sec;

		public long tv_nsec;

		public override int GetHashCode()
		{
			return tv_sec.GetHashCode() ^ tv_nsec.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			Timespec timespec = (Timespec)obj;
			if (timespec.tv_sec == tv_sec)
			{
				return timespec.tv_nsec == tv_nsec;
			}
			return false;
		}

		public bool Equals(Timespec value)
		{
			if (value.tv_sec == tv_sec)
			{
				return value.tv_nsec == tv_nsec;
			}
			return false;
		}

		public static bool operator ==(Timespec lhs, Timespec rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Timespec lhs, Timespec rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
