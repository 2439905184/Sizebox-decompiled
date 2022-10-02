using System;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public struct UnixPipes : IEquatable<UnixPipes>
	{
		public UnixStream Reading;

		public UnixStream Writing;

		public UnixPipes(UnixStream reading, UnixStream writing)
		{
			Reading = reading;
			Writing = writing;
		}

		public static UnixPipes CreatePipes()
		{
			int reading;
			int writing;
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.pipe(out reading, out writing));
			return new UnixPipes(new UnixStream(reading), new UnixStream(writing));
		}

		public override bool Equals(object value)
		{
			if (value == null || value.GetType() != GetType())
			{
				return false;
			}
			UnixPipes unixPipes = (UnixPipes)value;
			if (Reading.Handle == unixPipes.Reading.Handle)
			{
				return Writing.Handle == unixPipes.Writing.Handle;
			}
			return false;
		}

		public bool Equals(UnixPipes value)
		{
			if (Reading.Handle == value.Reading.Handle)
			{
				return Writing.Handle == value.Writing.Handle;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Reading.Handle.GetHashCode() ^ Writing.Handle.GetHashCode();
		}

		public static bool operator ==(UnixPipes lhs, UnixPipes rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(UnixPipes lhs, UnixPipes rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
