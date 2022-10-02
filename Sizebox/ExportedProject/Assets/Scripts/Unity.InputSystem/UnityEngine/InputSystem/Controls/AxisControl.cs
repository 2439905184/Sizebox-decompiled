using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[Preserve]
	public class AxisControl : InputControl<float>
	{
		public enum Clamp
		{
			None = 0,
			BeforeNormalize = 1,
			AfterNormalize = 2,
			ToConstantBeforeNormalize = 3
		}

		public Clamp clamp;

		public float clampMin;

		public float clampMax;

		public float clampConstant;

		public bool invert;

		public bool normalize;

		public float normalizeMin;

		public float normalizeMax;

		public float normalizeZero;

		public bool scale;

		public float scaleFactor;

		protected float Preprocess(float value)
		{
			if (scale)
			{
				value *= scaleFactor;
			}
			if (clamp == Clamp.ToConstantBeforeNormalize)
			{
				if (value < clampMin || value > clampMax)
				{
					value = clampConstant;
				}
			}
			else if (clamp == Clamp.BeforeNormalize)
			{
				value = Mathf.Clamp(value, clampMin, clampMax);
			}
			if (normalize)
			{
				value = NormalizeProcessor.Normalize(value, normalizeMin, normalizeMax, normalizeZero);
			}
			if (clamp == Clamp.AfterNormalize)
			{
				value = Mathf.Clamp(value, clampMin, clampMax);
			}
			if (invert)
			{
				value *= -1f;
			}
			return value;
		}

		public AxisControl()
		{
			m_StateBlock.format = InputStateBlock.FormatFloat;
		}

		protected override void FinishSetup()
		{
			base.FinishSetup();
			if (!base.hasDefaultState && normalize && Mathf.Abs(normalizeZero) > Mathf.Epsilon)
			{
				m_DefaultState = base.stateBlock.FloatToPrimitiveValue(normalizeZero);
			}
		}

		public unsafe override float ReadUnprocessedValueFromState(void* statePtr)
		{
			float value = base.stateBlock.ReadFloat(statePtr);
			return Preprocess(value);
		}

		public unsafe override void WriteValueIntoState(float value, void* statePtr)
		{
			base.stateBlock.WriteFloat(statePtr, value);
		}

		public unsafe override bool CompareValue(void* firstStatePtr, void* secondStatePtr)
		{
			float a = ReadValueFromState(firstStatePtr);
			float b = ReadValueFromState(secondStatePtr);
			return !Mathf.Approximately(a, b);
		}

		public unsafe override float EvaluateMagnitude(void* statePtr)
		{
			if (m_MinValue.isEmpty || m_MaxValue.isEmpty)
			{
				return -1f;
			}
			float value = ReadValueFromState(statePtr);
			float num = m_MinValue.ToSingle();
			float max = m_MaxValue.ToSingle();
			value = Mathf.Clamp(value, num, max);
			if (num < 0f)
			{
				if (value < 0f)
				{
					return NormalizeProcessor.Normalize(Mathf.Abs(value), 0f, Mathf.Abs(num), 0f);
				}
				return NormalizeProcessor.Normalize(value, 0f, max, 0f);
			}
			return NormalizeProcessor.Normalize(value, num, max, 0f);
		}
	}
}
