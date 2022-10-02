using System;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public struct InputEventPtr : IEquatable<InputEventPtr>
	{
		private unsafe readonly InputEvent* m_EventPtr;

		public unsafe bool valid
		{
			get
			{
				return m_EventPtr != null;
			}
		}

		public unsafe bool handled
		{
			get
			{
				if (!valid)
				{
					return false;
				}
				return m_EventPtr->handled;
			}
			set
			{
				if (!valid)
				{
					throw new InvalidOperationException("The InputEventPtr is not valid.");
				}
				m_EventPtr->handled = value;
			}
		}

		public unsafe int id
		{
			get
			{
				if (!valid)
				{
					return 0;
				}
				return m_EventPtr->eventId;
			}
			set
			{
				if (!valid)
				{
					throw new InvalidOperationException("The InputEventPtr is not valid.");
				}
				m_EventPtr->eventId = value;
			}
		}

		public unsafe FourCC type
		{
			get
			{
				if (!valid)
				{
					return default(FourCC);
				}
				return m_EventPtr->type;
			}
		}

		public unsafe uint sizeInBytes
		{
			get
			{
				if (!valid)
				{
					return 0u;
				}
				return m_EventPtr->sizeInBytes;
			}
		}

		public unsafe int deviceId
		{
			get
			{
				if (!valid)
				{
					return 0;
				}
				return m_EventPtr->deviceId;
			}
			set
			{
				if (!valid)
				{
					throw new InvalidOperationException("The InputEventPtr is not valid.");
				}
				m_EventPtr->deviceId = value;
			}
		}

		public unsafe double time
		{
			get
			{
				if (!valid)
				{
					return 0.0;
				}
				return m_EventPtr->time;
			}
			set
			{
				if (!valid)
				{
					throw new InvalidOperationException("The InputEventPtr is not valid.");
				}
				m_EventPtr->time = value;
			}
		}

		internal unsafe double internalTime
		{
			get
			{
				if (!valid)
				{
					return 0.0;
				}
				return m_EventPtr->internalTime;
			}
			set
			{
				if (!valid)
				{
					throw new InvalidOperationException("The InputEventPtr is not valid.");
				}
				m_EventPtr->internalTime = value;
			}
		}

		public unsafe InputEvent* data
		{
			get
			{
				return m_EventPtr;
			}
		}

		internal unsafe FourCC stateFormat
		{
			get
			{
				if (IsA<StateEvent>())
				{
					return StateEvent.From(this)->stateFormat;
				}
				if (IsA<DeltaStateEvent>())
				{
					return DeltaStateEvent.From(this)->stateFormat;
				}
				throw new InvalidOperationException("Event must be a StateEvent or DeltaStateEvent but is " + this);
			}
		}

		internal unsafe uint stateSizeInBytes
		{
			get
			{
				if (IsA<StateEvent>())
				{
					return StateEvent.From(this)->stateSizeInBytes;
				}
				if (IsA<DeltaStateEvent>())
				{
					return DeltaStateEvent.From(this)->deltaStateSizeInBytes;
				}
				throw new InvalidOperationException("Event must be a StateEvent or DeltaStateEvent but is " + this);
			}
		}

		internal unsafe uint stateOffset
		{
			get
			{
				if (IsA<DeltaStateEvent>())
				{
					return DeltaStateEvent.From(this)->stateOffset;
				}
				throw new InvalidOperationException("Event must be a DeltaStateEvent but is " + this);
			}
		}

		public unsafe InputEventPtr(InputEvent* eventPtr)
		{
			m_EventPtr = eventPtr;
		}

		public unsafe bool IsA<TOtherEvent>() where TOtherEvent : struct, IInputEventTypeInfo
		{
			if (m_EventPtr == null)
			{
				return false;
			}
			return m_EventPtr->type == default(TOtherEvent).typeStatic;
		}

		public unsafe InputEventPtr Next()
		{
			if (!valid)
			{
				return default(InputEventPtr);
			}
			return new InputEventPtr(InputEvent.GetNextInMemory(m_EventPtr));
		}

		public unsafe override string ToString()
		{
			if (!valid)
			{
				return "null";
			}
			InputEvent eventPtr = *m_EventPtr;
			return eventPtr.ToString();
		}

		public unsafe InputEvent* ToPointer()
		{
			return this;
		}

		public unsafe bool Equals(InputEventPtr other)
		{
			if (m_EventPtr != other.m_EventPtr)
			{
				return InputEvent.Equals(m_EventPtr, other.m_EventPtr);
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			object obj2;
			if ((obj2 = obj) is InputEventPtr)
			{
				InputEventPtr other = (InputEventPtr)obj2;
				return Equals(other);
			}
			return false;
		}

		public unsafe override int GetHashCode()
		{
			return (int)m_EventPtr;
		}

		public unsafe static bool operator ==(InputEventPtr left, InputEventPtr right)
		{
			return left.m_EventPtr == right.m_EventPtr;
		}

		public unsafe static bool operator !=(InputEventPtr left, InputEventPtr right)
		{
			return left.m_EventPtr != right.m_EventPtr;
		}

		public unsafe static implicit operator InputEventPtr(InputEvent* eventPtr)
		{
			return new InputEventPtr(eventPtr);
		}

		public unsafe static InputEventPtr From(InputEvent* eventPtr)
		{
			return new InputEventPtr(eventPtr);
		}

		public unsafe static implicit operator InputEvent*(InputEventPtr eventPtr)
		{
			return eventPtr.data;
		}

		public unsafe static InputEvent* FromInputEventPtr(InputEventPtr eventPtr)
		{
			return eventPtr.data;
		}
	}
}
