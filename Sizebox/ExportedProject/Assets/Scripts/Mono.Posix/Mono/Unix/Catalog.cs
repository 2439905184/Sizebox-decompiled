using System;
using System.Runtime.InteropServices;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public class Catalog
	{
		private Catalog()
		{
		}

		[DllImport("intl", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr bindtextdomain(IntPtr domainname, IntPtr dirname);

		[DllImport("intl", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr bind_textdomain_codeset(IntPtr domainname, IntPtr codeset);

		[DllImport("intl", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr textdomain(IntPtr domainname);

		public static void Init(string package, string localedir)
		{
			IntPtr p;
			IntPtr p2;
			IntPtr p3;
			MarshalStrings(package, out p, localedir, out p2, "UTF-8", out p3);
			try
			{
				if (bindtextdomain(p, p2) == IntPtr.Zero)
				{
					throw new UnixIOException(Errno.ENOMEM);
				}
				if (bind_textdomain_codeset(p, p3) == IntPtr.Zero)
				{
					throw new UnixIOException(Errno.ENOMEM);
				}
				if (textdomain(p) == IntPtr.Zero)
				{
					throw new UnixIOException(Errno.ENOMEM);
				}
			}
			finally
			{
				UnixMarshal.FreeHeap(p);
				UnixMarshal.FreeHeap(p2);
				UnixMarshal.FreeHeap(p3);
			}
		}

		private static void MarshalStrings(string s1, out IntPtr p1, string s2, out IntPtr p2, string s3, out IntPtr p3)
		{
			p1 = (p2 = (p3 = IntPtr.Zero));
			bool flag = true;
			try
			{
				p1 = UnixMarshal.StringToHeap(s1);
				p2 = UnixMarshal.StringToHeap(s2);
				if (s3 != null)
				{
					p3 = UnixMarshal.StringToHeap(s3);
				}
				flag = false;
			}
			finally
			{
				if (flag)
				{
					UnixMarshal.FreeHeap(p1);
					UnixMarshal.FreeHeap(p2);
					UnixMarshal.FreeHeap(p3);
				}
			}
		}

		[DllImport("intl", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr gettext(IntPtr instring);

		public static string GetString(string s)
		{
			IntPtr intPtr = UnixMarshal.StringToHeap(s);
			try
			{
				IntPtr intPtr2 = gettext(intPtr);
				if (intPtr2 != intPtr)
				{
					return UnixMarshal.PtrToStringUnix(intPtr2);
				}
				return s;
			}
			finally
			{
				UnixMarshal.FreeHeap(intPtr);
			}
		}

		[DllImport("intl", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr ngettext(IntPtr singular, IntPtr plural, int n);

		public static string GetPluralString(string s, string p, int n)
		{
			IntPtr p2;
			IntPtr p3;
			IntPtr p4;
			MarshalStrings(s, out p2, p, out p3, null, out p4);
			try
			{
				IntPtr intPtr = ngettext(p2, p3, n);
				if (intPtr == p2)
				{
					return s;
				}
				if (intPtr == p3)
				{
					return p;
				}
				return UnixMarshal.PtrToStringUnix(intPtr);
			}
			finally
			{
				UnixMarshal.FreeHeap(p2);
				UnixMarshal.FreeHeap(p3);
			}
		}
	}
}
