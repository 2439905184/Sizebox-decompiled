using System;

namespace Mono.Unix.Native
{
	[Map("struct pollfd")]
	public struct Pollfd : IEquatable<Pollfd>
	{
		public int fd;

		[CLSCompliant(false)]
		public PollEvents events;

		[CLSCompliant(false)]
		public PollEvents revents;

		public override int GetHashCode()
		{
			return events.GetHashCode() ^ revents.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			Pollfd pollfd = (Pollfd)obj;
			if (pollfd.events == events)
			{
				return pollfd.revents == revents;
			}
			return false;
		}

		public bool Equals(Pollfd value)
		{
			if (value.events == events)
			{
				return value.revents == revents;
			}
			return false;
		}

		public static bool operator ==(Pollfd lhs, Pollfd rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Pollfd lhs, Pollfd rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
