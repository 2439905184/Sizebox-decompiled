using MoonSharp.Interpreter;

namespace Lua
{
	[MoonSharpUserData]
	public class IKEffector
	{
		private IKBone effector;

		public Vector3 position
		{
			get
			{
				return new Vector3(effector.virtualPosition);
			}
			set
			{
				effector.virtualPosition = value.virtualPosition;
			}
		}

		public float positionWeight
		{
			get
			{
				return effector.positionWeight;
			}
			set
			{
				effector.positionWeight = value;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return new Quaternion(effector.rotation);
			}
			set
			{
				effector.rotation = value.quaternion;
			}
		}

		public float rotationWeight
		{
			get
			{
				return effector.rotationWeight;
			}
			set
			{
				effector.rotationWeight = value;
			}
		}

		[MoonSharpHidden]
		public IKEffector(IKBone ikBone)
		{
			effector = ikBone;
		}
	}
}
