using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Composites
{
	[Preserve]
	[DisplayStringFormat("{modifier1}+{modifier2}+{button}")]
	public class ButtonWithTwoModifiers : InputBindingComposite<float>
	{
		[InputControl(layout = "Button")]
		public int modifier1;

		[InputControl(layout = "Button")]
		public int modifier2;

		[InputControl(layout = "Button")]
		public int button;

		public override float ReadValue(ref InputBindingCompositeContext context)
		{
			if (context.ReadValueAsButton(modifier1) && context.ReadValueAsButton(modifier2))
			{
				return context.ReadValue<float>(button);
			}
			return 0f;
		}

		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			return ReadValue(ref context);
		}
	}
}
