using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Composites
{
	[Preserve]
	[DisplayStringFormat("{modifier}+{button}")]
	public class ButtonWithOneModifier : InputBindingComposite<float>
	{
		[InputControl(layout = "Button")]
		public int modifier;

		[InputControl(layout = "Button")]
		public int button;

		public override float ReadValue(ref InputBindingCompositeContext context)
		{
			if (context.ReadValueAsButton(modifier))
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
