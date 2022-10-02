using System.ComponentModel;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Processors
{
	[DesignTimeVisible(false)]
	[Preserve]
	internal class CompensateRotationProcessor : InputProcessor<Quaternion>
	{
		public override Quaternion Process(Quaternion value, InputControl control)
		{
			if (!InputSystem.settings.compensateForScreenOrientation)
			{
				return value;
			}
			Quaternion quaternion = Quaternion.identity;
			switch (InputRuntime.s_Instance.screenOrientation)
			{
			case ScreenOrientation.PortraitUpsideDown:
				quaternion = new Quaternion(0f, 0f, 1f, 0f);
				break;
			case ScreenOrientation.LandscapeLeft:
				quaternion = new Quaternion(0f, 0f, 0.70710677f, -0.70710677f);
				break;
			case ScreenOrientation.LandscapeRight:
				quaternion = new Quaternion(0f, 0f, -0.70710677f, -0.70710677f);
				break;
			}
			return value * quaternion;
		}
	}
}
