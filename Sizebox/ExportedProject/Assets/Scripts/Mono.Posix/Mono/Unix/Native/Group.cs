using System;
using System.Text;

namespace Mono.Unix.Native
{
	public sealed class Group : IEquatable<Group>
	{
		public string gr_name;

		public string gr_passwd;

		[CLSCompliant(false)]
		public uint gr_gid;

		public string[] gr_mem;

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < gr_mem.Length; i++)
			{
				num ^= gr_mem[i].GetHashCode();
			}
			return gr_name.GetHashCode() ^ gr_passwd.GetHashCode() ^ gr_gid.GetHashCode() ^ num;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Group value = (Group)obj;
			return Equals(value);
		}

		public bool Equals(Group value)
		{
			if (value == null)
			{
				return false;
			}
			if (value.gr_gid != gr_gid)
			{
				return false;
			}
			if (value.gr_gid == gr_gid && value.gr_name == gr_name && value.gr_passwd == gr_passwd)
			{
				if (value.gr_mem == gr_mem)
				{
					return true;
				}
				if (value.gr_mem == null || gr_mem == null)
				{
					return false;
				}
				if (value.gr_mem.Length != gr_mem.Length)
				{
					return false;
				}
				for (int i = 0; i < gr_mem.Length; i++)
				{
					if (gr_mem[i] != value.gr_mem[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(gr_name).Append(":").Append(gr_passwd)
				.Append(":");
			stringBuilder.Append(gr_gid).Append(":");
			GetMembers(stringBuilder, gr_mem);
			return stringBuilder.ToString();
		}

		private static void GetMembers(StringBuilder sb, string[] members)
		{
			if (members.Length != 0)
			{
				sb.Append(members[0]);
			}
			for (int i = 1; i < members.Length; i++)
			{
				sb.Append(",");
				sb.Append(members[i]);
			}
		}

		public static bool operator ==(Group lhs, Group rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(Group lhs, Group rhs)
		{
			return !object.Equals(lhs, rhs);
		}
	}
}
