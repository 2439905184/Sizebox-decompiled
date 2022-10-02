using MoonSharp.Interpreter;

namespace Lua
{
	[MoonSharpUserData]
	public class IK
	{
		public bool enabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public IKEffector leftFoot { get; private set; }

		public IKEffector rightFoot { get; private set; }

		public IKEffector leftHand { get; private set; }

		public IKEffector rightHand { get; private set; }

		public IKEffector body { get; private set; }

		[MoonSharpHidden]
		public IK(GiantessIK ik)
		{
		}
	}
}
