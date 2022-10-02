using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 56)]
	public struct TouchState : IInputStateTypeInfo
	{
		internal const int kSizeInBytes = 56;

		[FieldOffset(0)]
		[InputControl(displayName = "Touch ID", layout = "Integer", synthetic = true)]
		public int touchId;

		[FieldOffset(4)]
		[InputControl(displayName = "Position")]
		public Vector2 position;

		[FieldOffset(12)]
		[InputControl(displayName = "Delta")]
		public Vector2 delta;

		[FieldOffset(20)]
		[InputControl(displayName = "Pressure", layout = "Axis")]
		public float pressure;

		[FieldOffset(24)]
		[InputControl(displayName = "Radius")]
		public Vector2 radius;

		[FieldOffset(32)]
		[InputControl(name = "phase", displayName = "Touch Phase", layout = "TouchPhase", synthetic = true)]
		[InputControl(name = "press", displayName = "Touch Contact?", layout = "TouchPress", useStateFrom = "phase")]
		public byte phaseId;

		[FieldOffset(33)]
		[InputControl(name = "tapCount", displayName = "Tap Count", layout = "Integer")]
		public byte tapCount;

		[FieldOffset(34)]
		private byte displayIndex;

		[FieldOffset(35)]
		[InputControl(name = "indirectTouch", displayName = "Indirect Touch?", layout = "Button", bit = 0u, synthetic = true)]
		[InputControl(name = "tap", displayName = "Tap", layout = "Button", bit = 5u)]
		public byte flags;

		[FieldOffset(36)]
		internal int padding;

		[FieldOffset(40)]
		[InputControl(displayName = "Start Time", layout = "Double", synthetic = true)]
		public double startTime;

		[FieldOffset(48)]
		[InputControl(displayName = "Start Position", synthetic = true)]
		public Vector2 startPosition;

		public static FourCC Format
		{
			get
			{
				return new FourCC('T', 'O', 'U', 'C');
			}
		}

		public TouchPhase phase
		{
			get
			{
				return (TouchPhase)phaseId;
			}
			set
			{
				phaseId = (byte)value;
			}
		}

		public bool isNoneEndedOrCanceled
		{
			get
			{
				if (phase != 0 && phase != TouchPhase.Ended)
				{
					return phase == TouchPhase.Canceled;
				}
				return true;
			}
		}

		public bool isInProgress
		{
			get
			{
				if (phase != TouchPhase.Began && phase != TouchPhase.Moved)
				{
					return phase == TouchPhase.Stationary;
				}
				return true;
			}
		}

		public bool isPrimaryTouch
		{
			get
			{
				return (flags & 0x10) != 0;
			}
			set
			{
				if (value)
				{
					flags |= 16;
				}
				else
				{
					flags &= 239;
				}
			}
		}

		internal bool isOrphanedPrimaryTouch
		{
			get
			{
				return (flags & 0x40) != 0;
			}
			set
			{
				if (value)
				{
					flags |= 64;
				}
				else
				{
					flags &= 191;
				}
			}
		}

		public bool isIndirectTouch
		{
			get
			{
				return (flags & 1) != 0;
			}
			set
			{
				if (value)
				{
					flags |= 1;
				}
				else
				{
					flags &= 254;
				}
			}
		}

		public bool isTap
		{
			get
			{
				return (flags & 0x20) != 0;
			}
			set
			{
				if (value)
				{
					flags |= 32;
				}
				else
				{
					flags &= 223;
				}
			}
		}

		public FourCC format
		{
			get
			{
				return Format;
			}
		}

		public override string ToString()
		{
			return string.Format("{{ id={0} phase={1} pos={2} delta={3} pressure={4} radius={5} primary={6} }}", touchId, phase, position, delta, pressure, radius, isPrimaryTouch);
		}
	}
}
