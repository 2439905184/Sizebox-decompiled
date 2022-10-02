using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[Preserve]
	public class DpadControl : Vector2Control
	{
		[InputControlLayout(hideInUI = true)]
		[Preserve]
		internal class DpadAxisControl : AxisControl
		{
			public int component;

			protected override void FinishSetup()
			{
				base.FinishSetup();
				component = ((!(base.name == "x")) ? 1 : 0);
				m_StateBlock = m_Parent.m_StateBlock;
			}

			public unsafe override float ReadUnprocessedValueFromState(void* statePtr)
			{
				return (m_Parent as DpadControl).ReadUnprocessedValueFromState(statePtr)[component];
			}
		}

		internal enum ButtonBits
		{
			Up = 0,
			Down = 1,
			Left = 2,
			Right = 3
		}

		[InputControl(name = "x", layout = "DpadAxis", useStateFrom = "right", synthetic = true)]
		[InputControl(name = "y", layout = "DpadAxis", useStateFrom = "up", synthetic = true)]
		[InputControl(bit = 0u, displayName = "Up")]
		public ButtonControl up { get; private set; }

		[InputControl(bit = 1u, displayName = "Down")]
		public ButtonControl down { get; private set; }

		[InputControl(bit = 2u, displayName = "Left")]
		public ButtonControl left { get; private set; }

		[InputControl(bit = 3u, displayName = "Right")]
		public ButtonControl right { get; private set; }

		public DpadControl()
		{
			m_StateBlock.sizeInBits = 4u;
			m_StateBlock.format = InputStateBlock.FormatBit;
		}

		protected override void FinishSetup()
		{
			up = GetChildControl<ButtonControl>("up");
			down = GetChildControl<ButtonControl>("down");
			left = GetChildControl<ButtonControl>("left");
			right = GetChildControl<ButtonControl>("right");
			base.FinishSetup();
		}

		public unsafe override Vector2 ReadUnprocessedValueFromState(void* statePtr)
		{
			bool num = up.ReadValueFromState(statePtr) >= up.pressPointOrDefault;
			bool flag = down.ReadValueFromState(statePtr) >= down.pressPointOrDefault;
			bool flag2 = left.ReadValueFromState(statePtr) >= left.pressPointOrDefault;
			bool flag3 = right.ReadValueFromState(statePtr) >= right.pressPointOrDefault;
			return MakeDpadVector(num, flag, flag2, flag3);
		}

		public unsafe override void WriteValueIntoState(Vector2 value, void* statePtr)
		{
			throw new NotImplementedException();
		}

		public static Vector2 MakeDpadVector(bool up, bool down, bool left, bool right, bool normalize = true)
		{
			float num = (up ? 1f : 0f);
			float num2 = (down ? (-1f) : 0f);
			float num3 = (left ? (-1f) : 0f);
			float num4 = (right ? 1f : 0f);
			Vector2 result = new Vector2(num3 + num4, num + num2);
			if (normalize && result.x != 0f && result.y != 0f)
			{
				result = new Vector2(result.x * 0.707107f, result.y * 0.707107f);
			}
			return result;
		}

		public static Vector2 MakeDpadVector(float up, float down, float left, float right)
		{
			return new Vector2(0f - left + right, up - down);
		}
	}
}
