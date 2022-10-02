using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public sealed class InputRemoting : IObservable<InputRemoting.Message>, IObserver<InputRemoting.Message>
	{
		public enum MessageType
		{
			Connect = 0,
			Disconnect = 1,
			NewLayout = 2,
			NewDevice = 3,
			NewEvents = 4,
			RemoveDevice = 5,
			RemoveLayout = 6,
			ChangeUsages = 7,
			StartSending = 8,
			StopSending = 9
		}

		public struct Message
		{
			public int participantId;

			public MessageType type;

			public byte[] data;
		}

		[Flags]
		private enum Flags
		{
			Sending = 1,
			StartSendingOnConnect = 2
		}

		[Serializable]
		internal struct RemoteSender
		{
			public int senderId;

			public string[] layouts;

			public RemoteInputDevice[] devices;
		}

		[Serializable]
		internal struct RemoteInputDevice
		{
			public int remoteId;

			public int localId;

			public string layoutName;

			public InputDeviceDescription description;
		}

		internal class Subscriber : IDisposable
		{
			public InputRemoting owner;

			public IObserver<Message> observer;

			public void Dispose()
			{
				ArrayHelpers.Erase(ref owner.m_Subscribers, this);
			}
		}

		private static class ConnectMsg
		{
			public static void Process(InputRemoting receiver)
			{
				if (receiver.sending)
				{
					receiver.SendAllDevices();
				}
				else if ((receiver.m_Flags & Flags.StartSendingOnConnect) == Flags.StartSendingOnConnect)
				{
					receiver.StartSending();
				}
			}
		}

		private static class StartSendingMsg
		{
			public static void Process(InputRemoting receiver)
			{
				receiver.StartSending();
			}
		}

		private static class StopSendingMsg
		{
			public static void Process(InputRemoting receiver)
			{
				receiver.StopSending();
			}
		}

		private static class DisconnectMsg
		{
			public static void Process(InputRemoting receiver, Message msg)
			{
				Debug.Log("DisconnectMsg.Process");
				receiver.RemoveRemoteDevices(msg.participantId);
				receiver.StopSending();
			}
		}

		private static class NewLayoutMsg
		{
			public static Message? Create(InputRemoting sender, string layoutName)
			{
				InputControlLayout inputControlLayout;
				try
				{
					inputControlLayout = sender.m_LocalManager.TryLoadControlLayout(new InternedString(layoutName));
					if (inputControlLayout == null)
					{
						Debug.Log(string.Format("Could not find layout '{0}' meant to be sent through remote connection; this should not happen", layoutName));
						return null;
					}
				}
				catch (Exception arg)
				{
					Debug.Log(string.Format("Could not load layout '{0}'; not sending to remote listeners (exception: {1})", layoutName, arg));
					return null;
				}
				string s = inputControlLayout.ToJson();
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				Message value = default(Message);
				value.type = MessageType.NewLayout;
				value.data = bytes;
				return value;
			}

			public static void Process(InputRemoting receiver, Message msg)
			{
				string @string = Encoding.UTF8.GetString(msg.data);
				int num = receiver.FindOrCreateSenderRecord(msg.participantId);
				receiver.m_LocalManager.RegisterControlLayout(@string);
				ArrayHelpers.Append(ref receiver.m_Senders[num].layouts, @string);
			}
		}

		private static class RemoveLayoutMsg
		{
			public static Message Create(string layoutName)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(layoutName);
				Message result = default(Message);
				result.type = MessageType.RemoveLayout;
				result.data = bytes;
				return result;
			}

			public static void Process(InputRemoting receiver, Message msg)
			{
				string @string = Encoding.UTF8.GetString(msg.data);
				receiver.m_LocalManager.RemoveControlLayout(@string);
			}
		}

		private static class NewDeviceMsg
		{
			[Serializable]
			public struct Data
			{
				public string name;

				public string layout;

				public int deviceId;

				public InputDeviceDescription description;
			}

			public static Message Create(InputDevice device)
			{
				Data data = default(Data);
				data.name = device.name;
				data.layout = device.layout;
				data.deviceId = device.deviceId;
				data.description = device.description;
				string s = JsonUtility.ToJson(data);
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				Message result = default(Message);
				result.type = MessageType.NewDevice;
				result.data = bytes;
				return result;
			}

			public static void Process(InputRemoting receiver, Message msg)
			{
				int num = receiver.FindOrCreateSenderRecord(msg.participantId);
				Data data = DeserializeData<Data>(msg.data);
				RemoteInputDevice[] devices = receiver.m_Senders[num].devices;
				if (devices != null)
				{
					RemoteInputDevice[] array = devices;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].remoteId == data.deviceId)
						{
							Debug.LogError(string.Format("Already received device with id {0} (layout '{1}', description '{3}) from remote {2}", data.deviceId, data.layout, msg.participantId, data.description));
							return;
						}
					}
				}
				InputDevice inputDevice;
				try
				{
					inputDevice = receiver.m_LocalManager.AddDevice(data.layout);
					inputDevice.m_ParticipantId = msg.participantId;
				}
				catch (Exception arg)
				{
					Debug.LogError(string.Format("Could not create remote device '{0}' with layout '{1}' locally (exception: {2})", data.description, data.layout, arg));
					return;
				}
				inputDevice.m_Description = data.description;
				inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.Remote;
				RemoteInputDevice remoteInputDevice = default(RemoteInputDevice);
				remoteInputDevice.remoteId = data.deviceId;
				remoteInputDevice.localId = inputDevice.deviceId;
				remoteInputDevice.description = data.description;
				remoteInputDevice.layoutName = data.layout;
				RemoteInputDevice value = remoteInputDevice;
				ArrayHelpers.Append(ref receiver.m_Senders[num].devices, value);
			}
		}

		private static class NewEventsMsg
		{
			public unsafe static Message Create(InputEvent* events, int eventCount)
			{
				uint num = 0u;
				InputEventPtr inputEventPtr = new InputEventPtr(events);
				int num2 = 0;
				while (num2 < eventCount)
				{
					num += inputEventPtr.sizeInBytes;
					num2++;
					inputEventPtr = inputEventPtr.Next();
				}
				byte[] array = new byte[num];
				fixed (byte* destination = array)
				{
					UnsafeUtility.MemCpy(destination, events, num);
				}
				Message result = default(Message);
				result.type = MessageType.NewEvents;
				result.data = array;
				return result;
			}

			public unsafe static void Process(InputRemoting receiver, Message msg)
			{
				InputManager localManager = receiver.m_LocalManager;
				fixed (byte* ptr = msg.data)
				{
					IntPtr intPtr = new IntPtr(ptr + msg.data.Length);
					int num = 0;
					InputEventPtr ptr2 = new InputEventPtr((InputEvent*)ptr);
					int senderIndex = receiver.FindOrCreateSenderRecord(msg.participantId);
					while ((long)ptr2.data < intPtr.ToInt64())
					{
						int deviceId = ptr2.deviceId;
						if ((ptr2.deviceId = receiver.FindLocalDeviceId(deviceId, senderIndex)) != 0)
						{
							localManager.QueueEvent(ptr2);
						}
						num++;
						ptr2 = ptr2.Next();
					}
				}
			}
		}

		private static class ChangeUsageMsg
		{
			[Serializable]
			public struct Data
			{
				public int deviceId;

				public string[] usages;
			}

			[Serializable]
			[CompilerGenerated]
			private sealed class _003C_003Ec
			{
				public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

				public static Func<InternedString, string> _003C_003E9__1_0;

				internal string _003CCreate_003Eb__1_0(InternedString x)
				{
					return x.ToString();
				}
			}

			public static Message Create(InputDevice device)
			{
				Data data = default(Data);
				data.deviceId = device.deviceId;
				data.usages = device.usages.Select(_003C_003Ec._003C_003E9__1_0 ?? (_003C_003Ec._003C_003E9__1_0 = _003C_003Ec._003C_003E9._003CCreate_003Eb__1_0)).ToArray();
				Data data2 = data;
				Message result = default(Message);
				result.type = MessageType.ChangeUsages;
				result.data = SerializeData(data2);
				return result;
			}

			public static void Process(InputRemoting receiver, Message msg)
			{
				int senderIndex = receiver.FindOrCreateSenderRecord(msg.participantId);
				Data data = DeserializeData<Data>(msg.data);
				InputDevice inputDevice = receiver.TryGetDeviceByRemoteId(data.deviceId, senderIndex);
				if (inputDevice != null && data.usages.Length == 1)
				{
					receiver.m_LocalManager.SetDeviceUsage(inputDevice, new InternedString(data.usages[0]));
				}
			}
		}

		private static class RemoveDeviceMsg
		{
			public static Message Create(InputDevice device)
			{
				Message result = default(Message);
				result.type = MessageType.RemoveDevice;
				result.data = BitConverter.GetBytes(device.deviceId);
				return result;
			}

			public static void Process(InputRemoting receiver, Message msg)
			{
				int senderIndex = receiver.FindOrCreateSenderRecord(msg.participantId);
				int remoteDeviceId = BitConverter.ToInt32(msg.data, 0);
				InputDevice inputDevice = receiver.TryGetDeviceByRemoteId(remoteDeviceId, senderIndex);
				if (inputDevice != null)
				{
					receiver.m_LocalManager.RemoveDevice(inputDevice);
				}
			}
		}

		private Flags m_Flags;

		private InputManager m_LocalManager;

		private Subscriber[] m_Subscribers;

		private RemoteSender[] m_Senders;

		public bool sending
		{
			get
			{
				return (m_Flags & Flags.Sending) == Flags.Sending;
			}
			private set
			{
				if (value)
				{
					m_Flags |= Flags.Sending;
				}
				else
				{
					m_Flags &= ~Flags.Sending;
				}
			}
		}

		internal InputRemoting(InputManager manager, bool startSendingOnConnect = false)
		{
			if (manager == null)
			{
				throw new ArgumentNullException("manager");
			}
			m_LocalManager = manager;
			if (startSendingOnConnect)
			{
				m_Flags |= Flags.StartSendingOnConnect;
			}
		}

		public void StartSending()
		{
			if (!sending)
			{
				m_LocalManager.onEvent += SendEvent;
				m_LocalManager.onDeviceChange += SendDeviceChange;
				m_LocalManager.onLayoutChange += SendLayoutChange;
				sending = true;
				SendAllDevices();
			}
		}

		public void StopSending()
		{
			if (sending)
			{
				m_LocalManager.onEvent -= SendEvent;
				m_LocalManager.onDeviceChange -= SendDeviceChange;
				m_LocalManager.onLayoutChange -= SendLayoutChange;
				sending = false;
			}
		}

		void IObserver<Message>.OnNext(Message msg)
		{
			switch (msg.type)
			{
			case MessageType.Connect:
				ConnectMsg.Process(this);
				break;
			case MessageType.Disconnect:
				DisconnectMsg.Process(this, msg);
				break;
			case MessageType.NewLayout:
				NewLayoutMsg.Process(this, msg);
				break;
			case MessageType.RemoveLayout:
				RemoveLayoutMsg.Process(this, msg);
				break;
			case MessageType.NewDevice:
				NewDeviceMsg.Process(this, msg);
				break;
			case MessageType.NewEvents:
				NewEventsMsg.Process(this, msg);
				break;
			case MessageType.ChangeUsages:
				ChangeUsageMsg.Process(this, msg);
				break;
			case MessageType.RemoveDevice:
				RemoveDeviceMsg.Process(this, msg);
				break;
			case MessageType.StartSending:
				StartSendingMsg.Process(this);
				break;
			case MessageType.StopSending:
				StopSendingMsg.Process(this);
				break;
			}
		}

		void IObserver<Message>.OnError(Exception error)
		{
		}

		void IObserver<Message>.OnCompleted()
		{
		}

		public IDisposable Subscribe(IObserver<Message> observer)
		{
			if (observer == null)
			{
				throw new ArgumentNullException("observer");
			}
			Subscriber subscriber = new Subscriber
			{
				owner = this,
				observer = observer
			};
			ArrayHelpers.Append(ref m_Subscribers, subscriber);
			return subscriber;
		}

		private void SendAllDevices()
		{
			foreach (InputDevice device in m_LocalManager.devices)
			{
				SendDevice(device);
			}
		}

		private void SendDevice(InputDevice device)
		{
			Message msg = NewDeviceMsg.Create(device);
			Send(msg);
		}

		private unsafe void SendEvent(InputEventPtr eventPtr, InputDevice device)
		{
			if (m_Subscribers != null && (device == null || !device.remote))
			{
				Message msg = NewEventsMsg.Create(eventPtr.data, 1);
				Send(msg);
			}
		}

		private void SendDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (m_Subscribers != null && !device.remote)
			{
				Message msg;
				switch (change)
				{
				default:
					return;
				case InputDeviceChange.Added:
					msg = NewDeviceMsg.Create(device);
					break;
				case InputDeviceChange.Removed:
					msg = RemoveDeviceMsg.Create(device);
					break;
				case InputDeviceChange.UsageChanged:
					msg = ChangeUsageMsg.Create(device);
					break;
				}
				Send(msg);
			}
		}

		private void SendLayoutChange(string layout, InputControlLayoutChange change)
		{
			if (m_Subscribers == null)
			{
				return;
			}
			Message msg;
			switch (change)
			{
			default:
				return;
			case InputControlLayoutChange.Added:
			case InputControlLayoutChange.Replaced:
			{
				Message? message = NewLayoutMsg.Create(this, layout);
				if (!message.HasValue)
				{
					return;
				}
				msg = message.Value;
				break;
			}
			case InputControlLayoutChange.Removed:
				msg = RemoveLayoutMsg.Create(layout);
				break;
			}
			Send(msg);
		}

		private void Send(Message msg)
		{
			Subscriber[] subscribers = m_Subscribers;
			for (int i = 0; i < subscribers.Length; i++)
			{
				subscribers[i].observer.OnNext(msg);
			}
		}

		private int FindOrCreateSenderRecord(int senderId)
		{
			if (m_Senders != null)
			{
				int num = m_Senders.Length;
				for (int i = 0; i < num; i++)
				{
					if (m_Senders[i].senderId == senderId)
					{
						return i;
					}
				}
			}
			RemoteSender remoteSender = default(RemoteSender);
			remoteSender.senderId = senderId;
			RemoteSender value = remoteSender;
			return ArrayHelpers.Append(ref m_Senders, value);
		}

		private int FindLocalDeviceId(int remoteDeviceId, int senderIndex)
		{
			RemoteInputDevice[] devices = m_Senders[senderIndex].devices;
			if (devices != null)
			{
				int num = devices.Length;
				for (int i = 0; i < num; i++)
				{
					if (devices[i].remoteId == remoteDeviceId)
					{
						return devices[i].localId;
					}
				}
			}
			return 0;
		}

		private InputDevice TryGetDeviceByRemoteId(int remoteDeviceId, int senderIndex)
		{
			int id = FindLocalDeviceId(remoteDeviceId, senderIndex);
			return m_LocalManager.TryGetDeviceById(id);
		}

		public void RemoveRemoteDevices(int participantId)
		{
			int num = FindOrCreateSenderRecord(participantId);
			RemoteInputDevice[] devices = m_Senders[num].devices;
			if (devices != null)
			{
				RemoteInputDevice[] array = devices;
				for (int i = 0; i < array.Length; i++)
				{
					RemoteInputDevice remoteInputDevice = array[i];
					InputDevice inputDevice = m_LocalManager.TryGetDeviceById(remoteInputDevice.localId);
					if (inputDevice != null)
					{
						m_LocalManager.RemoveDevice(inputDevice);
					}
				}
			}
			ArrayHelpers.EraseAt(ref m_Senders, num);
		}

		private static byte[] SerializeData<TData>(TData data)
		{
			string s = JsonUtility.ToJson(data);
			return Encoding.UTF8.GetBytes(s);
		}

		private static TData DeserializeData<TData>(byte[] data)
		{
			return JsonUtility.FromJson<TData>(Encoding.UTF8.GetString(data));
		}
	}
}
