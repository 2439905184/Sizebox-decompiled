using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[DebuggerDisplay("{DebuggerDisplay(),nq}")]
	[Preserve]
	public abstract class InputControl
	{
		[Flags]
		internal enum ControlFlags
		{
			ConfigUpToDate = 1,
			IsNoisy = 2,
			IsSynthetic = 4
		}

		protected internal InputStateBlock m_StateBlock;

		internal InternedString m_Name;

		internal string m_Path;

		internal string m_DisplayName;

		internal string m_DisplayNameFromLayout;

		internal string m_ShortDisplayName;

		internal string m_ShortDisplayNameFromLayout;

		internal InternedString m_Layout;

		internal InternedString m_Variants;

		internal InputDevice m_Device;

		internal InputControl m_Parent;

		internal int m_UsageCount;

		internal int m_UsageStartIndex;

		internal int m_AliasCount;

		internal int m_AliasStartIndex;

		internal int m_ChildCount;

		internal int m_ChildStartIndex;

		internal ControlFlags m_ControlFlags;

		internal PrimitiveValue m_DefaultState;

		internal PrimitiveValue m_MinValue;

		internal PrimitiveValue m_MaxValue;

		public string name
		{
			get
			{
				return m_Name;
			}
		}

		public string displayName
		{
			get
			{
				RefreshConfigurationIfNeeded();
				if (m_DisplayName != null)
				{
					return m_DisplayName;
				}
				if (m_DisplayNameFromLayout != null)
				{
					return m_DisplayNameFromLayout;
				}
				return m_Name;
			}
			protected set
			{
				m_DisplayName = value;
			}
		}

		public string shortDisplayName
		{
			get
			{
				RefreshConfigurationIfNeeded();
				if (m_ShortDisplayName != null)
				{
					return m_ShortDisplayName;
				}
				if (m_ShortDisplayNameFromLayout != null)
				{
					return m_ShortDisplayNameFromLayout;
				}
				return null;
			}
			protected set
			{
				m_ShortDisplayName = value;
			}
		}

		public string path
		{
			get
			{
				if (m_Path == null)
				{
					m_Path = InputControlPath.Combine(m_Parent, m_Name);
				}
				return m_Path;
			}
		}

		public string layout
		{
			get
			{
				return m_Layout;
			}
		}

		public string variants
		{
			get
			{
				return m_Variants;
			}
		}

		public InputDevice device
		{
			get
			{
				return m_Device;
			}
		}

		public InputControl parent
		{
			get
			{
				return m_Parent;
			}
		}

		public ReadOnlyArray<InputControl> children
		{
			get
			{
				return new ReadOnlyArray<InputControl>(m_Device.m_ChildrenForEachControl, m_ChildStartIndex, m_ChildCount);
			}
		}

		public ReadOnlyArray<InternedString> usages
		{
			get
			{
				return new ReadOnlyArray<InternedString>(m_Device.m_UsagesForEachControl, m_UsageStartIndex, m_UsageCount);
			}
		}

		public ReadOnlyArray<InternedString> aliases
		{
			get
			{
				return new ReadOnlyArray<InternedString>(m_Device.m_AliasesForEachControl, m_AliasStartIndex, m_AliasCount);
			}
		}

		public InputStateBlock stateBlock
		{
			get
			{
				return m_StateBlock;
			}
		}

		public bool noisy
		{
			get
			{
				return (m_ControlFlags & ControlFlags.IsNoisy) != 0;
			}
			internal set
			{
				if (value)
				{
					m_ControlFlags |= ControlFlags.IsNoisy;
					ReadOnlyArray<InputControl> readOnlyArray = children;
					for (int i = 0; i < readOnlyArray.Count; i++)
					{
						readOnlyArray[i].noisy = true;
					}
				}
				else
				{
					m_ControlFlags &= ~ControlFlags.IsNoisy;
				}
			}
		}

		public bool synthetic
		{
			get
			{
				return (m_ControlFlags & ControlFlags.IsSynthetic) != 0;
			}
			internal set
			{
				if (value)
				{
					m_ControlFlags |= ControlFlags.IsSynthetic;
				}
				else
				{
					m_ControlFlags &= ~ControlFlags.IsSynthetic;
				}
			}
		}

		public InputControl this[string path]
		{
			get
			{
				InputControl inputControl = InputControlPath.TryFindChild(this, path);
				if (inputControl == null)
				{
					throw new KeyNotFoundException(string.Format("Cannot find control '{0}' as child of '{1}'", path, this));
				}
				return inputControl;
			}
		}

		public abstract Type valueType { get; }

		public abstract int valueSizeInBytes { get; }

		protected internal unsafe void* currentStatePtr
		{
			get
			{
				return InputStateBuffers.GetFrontBufferForDevice(ResolveDeviceIndex());
			}
		}

		protected internal unsafe void* previousFrameStatePtr
		{
			get
			{
				return InputStateBuffers.GetBackBufferForDevice(ResolveDeviceIndex());
			}
		}

		protected internal unsafe void* defaultStatePtr
		{
			get
			{
				return InputStateBuffers.s_DefaultStateBuffer;
			}
		}

		protected internal unsafe void* noiseMaskPtr
		{
			get
			{
				return InputStateBuffers.s_NoiseMaskBuffer;
			}
		}

		protected internal uint stateOffsetRelativeToDeviceRoot
		{
			get
			{
				uint byteOffset = device.m_StateBlock.byteOffset;
				return m_StateBlock.byteOffset - byteOffset;
			}
		}

		internal bool isConfigUpToDate
		{
			get
			{
				return (m_ControlFlags & ControlFlags.ConfigUpToDate) == ControlFlags.ConfigUpToDate;
			}
			set
			{
				if (value)
				{
					m_ControlFlags |= ControlFlags.ConfigUpToDate;
				}
				else
				{
					m_ControlFlags &= ~ControlFlags.ConfigUpToDate;
				}
			}
		}

		internal bool hasDefaultState
		{
			get
			{
				return !m_DefaultState.isEmpty;
			}
		}

		public override string ToString()
		{
			return layout + ":" + path;
		}

		private string DebuggerDisplay()
		{
			if (!device.added)
			{
				return ToString();
			}
			try
			{
				return string.Format("{0}:{1}={2}", layout, path, this.ReadValueAsObject());
			}
			catch (Exception)
			{
				return ToString();
			}
		}

		public unsafe float EvaluateMagnitude()
		{
			return EvaluateMagnitude(currentStatePtr);
		}

		public unsafe virtual float EvaluateMagnitude(void* statePtr)
		{
			return -1f;
		}

		public unsafe abstract object ReadValueFromBufferAsObject(void* buffer, int bufferSize);

		public unsafe abstract object ReadValueFromStateAsObject(void* statePtr);

		public unsafe abstract void ReadValueFromStateIntoBuffer(void* statePtr, void* bufferPtr, int bufferSize);

		public unsafe virtual void WriteValueFromBufferIntoState(void* bufferPtr, int bufferSize, void* statePtr)
		{
			throw new NotSupportedException(string.Format("Control '{0}' does not support writing", this));
		}

		public unsafe virtual void WriteValueFromObjectIntoState(object value, void* statePtr)
		{
			throw new NotSupportedException(string.Format("Control '{0}' does not support writing", this));
		}

		public unsafe abstract bool CompareValue(void* firstStatePtr, void* secondStatePtr);

		public InputControl TryGetChildControl(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			return InputControlPath.TryFindChild(this, path);
		}

		public TControl TryGetChildControl<TControl>(string path) where TControl : InputControl
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			InputControl inputControl = TryGetChildControl(path);
			if (inputControl == null)
			{
				return null;
			}
			TControl val = inputControl as TControl;
			if (val == null)
			{
				throw new InvalidOperationException("Expected control '" + path + "' to be of type '" + typeof(TControl).Name + "' but is of type '" + inputControl.GetType().Name + "' instead!");
			}
			return val;
		}

		public InputControl GetChildControl(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			InputControl inputControl = TryGetChildControl(path);
			if (inputControl == null)
			{
				throw new ArgumentException("Cannot find input control '" + MakeChildPath(path) + "'", "path");
			}
			return inputControl;
		}

		public TControl GetChildControl<TControl>(string path) where TControl : InputControl
		{
			InputControl childControl = GetChildControl(path);
			TControl result;
			if ((result = childControl as TControl) == null)
			{
				throw new ArgumentException("Expected control '" + path + "' to be of type '" + typeof(TControl).Name + "' but is of type '" + childControl.GetType().Name + "' instead!", "path");
			}
			return result;
		}

		protected InputControl()
		{
			m_StateBlock.byteOffset = 4294967294u;
		}

		protected virtual void FinishSetup()
		{
		}

		protected void RefreshConfigurationIfNeeded()
		{
			if (!isConfigUpToDate)
			{
				RefreshConfiguration();
				isConfigUpToDate = true;
			}
		}

		protected virtual void RefreshConfiguration()
		{
		}

		internal void CallFinishSetupRecursive()
		{
			ReadOnlyArray<InputControl> readOnlyArray = children;
			for (int i = 0; i < readOnlyArray.Count; i++)
			{
				readOnlyArray[i].CallFinishSetupRecursive();
			}
			FinishSetup();
		}

		internal string MakeChildPath(string path)
		{
			if (this is InputDevice)
			{
				return path;
			}
			return this.path + "/" + path;
		}

		internal void BakeOffsetIntoStateBlockRecursive(uint offset)
		{
			m_StateBlock.byteOffset += offset;
			ReadOnlyArray<InputControl> readOnlyArray = children;
			for (int i = 0; i < readOnlyArray.Count; i++)
			{
				readOnlyArray[i].BakeOffsetIntoStateBlockRecursive(offset);
			}
		}

		internal int ResolveDeviceIndex()
		{
			int deviceIndex = m_Device.m_DeviceIndex;
			if (deviceIndex == -1)
			{
				throw new InvalidOperationException("Cannot query value of control '" + path + "' before '" + device.name + "' has been added to system!");
			}
			return deviceIndex;
		}

		internal virtual void AddProcessor(object first)
		{
		}
	}
	[Preserve]
	public abstract class InputControl<TValue> : InputControl where TValue : struct
	{
		internal InlinedArray<InputProcessor<TValue>> m_ProcessorStack;

		public override Type valueType
		{
			get
			{
				return typeof(TValue);
			}
		}

		public override int valueSizeInBytes
		{
			get
			{
				return UnsafeUtility.SizeOf<TValue>();
			}
		}

		internal InputProcessor<TValue>[] processors
		{
			get
			{
				return m_ProcessorStack.ToArray();
			}
		}

		public unsafe TValue ReadValue()
		{
			return ReadValueFromState(base.currentStatePtr);
		}

		public unsafe TValue ReadValueFromPreviousFrame()
		{
			return ReadValueFromState(base.previousFrameStatePtr);
		}

		public unsafe TValue ReadDefaultValue()
		{
			return ReadValueFromState(base.defaultStatePtr);
		}

		public unsafe TValue ReadValueFromState(void* statePtr)
		{
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return ProcessValue(ReadUnprocessedValueFromState(statePtr));
		}

		public unsafe TValue ReadUnprocessedValue()
		{
			return ReadUnprocessedValueFromState(base.currentStatePtr);
		}

		public unsafe abstract TValue ReadUnprocessedValueFromState(void* statePtr);

		public unsafe override object ReadValueFromStateAsObject(void* statePtr)
		{
			return ReadValueFromState(statePtr);
		}

		public unsafe override void ReadValueFromStateIntoBuffer(void* statePtr, void* bufferPtr, int bufferSize)
		{
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			if (bufferPtr == null)
			{
				throw new ArgumentNullException("bufferPtr");
			}
			int num = UnsafeUtility.SizeOf<TValue>();
			if (bufferSize < num)
			{
				throw new ArgumentException(string.Format("bufferSize={0} < sizeof(TValue)={1}", bufferSize, num), "bufferSize");
			}
			TValue output = ReadValueFromState(statePtr);
			void* source = UnsafeUtility.AddressOf(ref output);
			UnsafeUtility.MemCpy(bufferPtr, source, num);
		}

		public unsafe override void WriteValueFromBufferIntoState(void* bufferPtr, int bufferSize, void* statePtr)
		{
			if (bufferPtr == null)
			{
				throw new ArgumentNullException("bufferPtr");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			int num = UnsafeUtility.SizeOf<TValue>();
			if (bufferSize < num)
			{
				throw new ArgumentException(string.Format("bufferSize={0} < sizeof(TValue)={1}", bufferSize, num), "bufferSize");
			}
			TValue output = default(TValue);
			UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref output), bufferPtr, num);
			WriteValueIntoState(output, statePtr);
		}

		public unsafe override void WriteValueFromObjectIntoState(object value, void* statePtr)
		{
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is TValue))
			{
				value = Convert.ChangeType(value, typeof(TValue));
			}
			TValue value2 = (TValue)value;
			WriteValueIntoState(value2, statePtr);
		}

		public unsafe virtual void WriteValueIntoState(TValue value, void* statePtr)
		{
			throw new NotSupportedException(string.Format("Control '{0}' does not support writing", this));
		}

		public unsafe override object ReadValueFromBufferAsObject(void* buffer, int bufferSize)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = UnsafeUtility.SizeOf<TValue>();
			if (bufferSize < num)
			{
				throw new ArgumentException(string.Format("Expecting buffer of at least {0} bytes for value of type {1} but got buffer of only {2} bytes instead", num, typeof(TValue).Name, bufferSize), "bufferSize");
			}
			TValue output = default(TValue);
			UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref output), buffer, num);
			return output;
		}

		public unsafe override bool CompareValue(void* firstStatePtr, void* secondStatePtr)
		{
			TValue output = ReadValueFromState(firstStatePtr);
			TValue output2 = ReadValueFromState(secondStatePtr);
			void* ptr = UnsafeUtility.AddressOf(ref output);
			void* ptr2 = UnsafeUtility.AddressOf(ref output2);
			return UnsafeUtility.MemCmp(ptr, ptr2, UnsafeUtility.SizeOf<TValue>()) != 0;
		}

		public TValue ProcessValue(TValue value)
		{
			if (m_ProcessorStack.length > 0)
			{
				value = m_ProcessorStack.firstValue.Process(value, this);
				if (m_ProcessorStack.additionalValues != null)
				{
					for (int i = 0; i < m_ProcessorStack.length - 1; i++)
					{
						value = m_ProcessorStack.additionalValues[i].Process(value, this);
					}
				}
			}
			return value;
		}

		internal TProcessor TryGetProcessor<TProcessor>() where TProcessor : InputProcessor<TValue>
		{
			if (m_ProcessorStack.length > 0)
			{
				TProcessor result;
				if ((result = m_ProcessorStack.firstValue as TProcessor) != null)
				{
					return result;
				}
				if (m_ProcessorStack.additionalValues != null)
				{
					for (int i = 0; i < m_ProcessorStack.length - 1; i++)
					{
						TProcessor result2;
						if ((result2 = m_ProcessorStack.additionalValues[i] as TProcessor) != null)
						{
							return result2;
						}
					}
				}
			}
			return null;
		}

		internal override void AddProcessor(object processor)
		{
			InputProcessor<TValue> value;
			if ((value = processor as InputProcessor<TValue>) == null)
			{
				throw new ArgumentException("Cannot add processor of type '" + processor.GetType().Name + "' to control of type '" + GetType().Name + "'", "processor");
			}
			m_ProcessorStack.Append(value);
		}
	}
}
