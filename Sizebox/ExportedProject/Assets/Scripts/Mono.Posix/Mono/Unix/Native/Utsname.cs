using System;

namespace Mono.Unix.Native
{
	public sealed class Utsname : IEquatable<Utsname>
	{
		public string sysname;

		public string nodename;

		public string release;

		public string version;

		public string machine;

		public string domainname;

		public override int GetHashCode()
		{
			return sysname.GetHashCode() ^ nodename.GetHashCode() ^ release.GetHashCode() ^ version.GetHashCode() ^ machine.GetHashCode() ^ domainname.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Utsname value = (Utsname)obj;
			return Equals(value);
		}

		public bool Equals(Utsname value)
		{
			if (value.sysname == sysname && value.nodename == nodename && value.release == release && value.version == version && value.machine == machine)
			{
				return value.domainname == domainname;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3} {4}", sysname, nodename, release, version, machine);
		}

		public static bool operator ==(Utsname lhs, Utsname rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(Utsname lhs, Utsname rhs)
		{
			return !object.Equals(lhs, rhs);
		}
	}
}
