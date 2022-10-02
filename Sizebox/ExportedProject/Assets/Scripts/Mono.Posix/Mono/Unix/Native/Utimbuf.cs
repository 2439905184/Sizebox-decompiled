using System;

namespace Mono.Unix.Native
{
	[Map("struct utimbuf")]
	public struct Utimbuf : IEquatable<Utimbuf>
	{
		[time_t]
		public long actime;

		[time_t]
		public long modtime;

		public override int GetHashCode()
		{
			return actime.GetHashCode() ^ modtime.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			Utimbuf utimbuf = (Utimbuf)obj;
			if (utimbuf.actime == actime)
			{
				return utimbuf.modtime == modtime;
			}
			return false;
		}

		public bool Equals(Utimbuf value)
		{
			if (value.actime == actime)
			{
				return value.modtime == modtime;
			}
			return false;
		}

		public static bool operator ==(Utimbuf lhs, Utimbuf rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Utimbuf lhs, Utimbuf rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
