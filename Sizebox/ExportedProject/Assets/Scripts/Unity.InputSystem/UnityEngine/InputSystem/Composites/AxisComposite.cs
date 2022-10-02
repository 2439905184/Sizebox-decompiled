using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Processors;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Composites
{
	[Preserve]
	[DisplayStringFormat("{negative}/{positive}")]
	public class AxisComposite : InputBindingComposite<float>
	{
		public enum WhichSideWins
		{
			Neither = 0,
			Positive = 1,
			Negative = 2
		}

		[InputControl(layout = "Button")]
		public int negative;

		[InputControl(layout = "Button")]
		public int positive;

		[Tooltip("Value to return when the negative side is fully actuated.")]
		public float minValue = -1f;

		[Tooltip("Value to return when the positive side is fully actuated.")]
		public float maxValue = 1f;

		[Tooltip("If both the positive and negative side are actuated, decides what value to return. 'Neither' (default) means that the resulting value is the midpoint between min and max. 'Positive' means that max will be returned. 'Negative' means that min will be returned.")]
		public WhichSideWins whichSideWins;

		public float midPoint
		{
			get
			{
				return (maxValue + minValue) / 2f;
			}
		}

		public override float ReadValue(ref InputBindingCompositeContext context)
		{
			float num = context.ReadValue<float>(negative);
			float num2 = context.ReadValue<float>(positive);
			bool flag = num > 0f;
			bool flag2 = num2 > 0f;
			if (flag == flag2)
			{
				switch (whichSideWins)
				{
				case WhichSideWins.Negative:
					return 0f - num;
				case WhichSideWins.Positive:
					return num2;
				case WhichSideWins.Neither:
					return midPoint;
				}
			}
			if (flag)
			{
				return 0f - num;
			}
			return num2;
		}

		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			float num = ReadValue(ref context);
			if (num < midPoint)
			{
				num = Mathf.Abs(num - midPoint);
				return NormalizeProcessor.Normalize(num, 0f, Mathf.Abs(minValue), 0f);
			}
			num = Mathf.Abs(num - midPoint);
			return NormalizeProcessor.Normalize(num, 0f, Mathf.Abs(maxValue), 0f);
		}
	}
}
