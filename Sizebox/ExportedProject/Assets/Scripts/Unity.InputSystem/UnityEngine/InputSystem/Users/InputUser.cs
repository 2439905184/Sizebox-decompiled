using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Users
{
	public struct InputUser : IEquatable<InputUser>
	{
		public struct ControlSchemeChangeSyntax
		{
			internal int m_UserIndex;

			public ControlSchemeChangeSyntax AndPairRemainingDevices()
			{
				UpdateControlSchemeMatch(m_UserIndex, true);
				return this;
			}
		}

		[Flags]
		internal enum UserFlags
		{
			BindToAllDevices = 1,
			UserAccountSelectionInProgress = 2
		}

		internal struct UserData
		{
			public InputUserAccountHandle? platformUserAccountHandle;

			public string platformUserAccountName;

			public string platformUserAccountId;

			public int deviceCount;

			public int deviceStartIndex;

			public IInputActionCollection actions;

			public InputControlScheme? controlScheme;

			public InputControlScheme.MatchResult controlSchemeMatch;

			public int lostDeviceCount;

			public int lostDeviceStartIndex;

			public UserFlags flags;
		}

		private struct CompareDevicesByUserAccount : IComparer<InputDevice>
		{
			public InputUserAccountHandle platformUserAccountHandle;

			public int Compare(InputDevice x, InputDevice y)
			{
				InputUserAccountHandle? userAccountHandleForDevice = GetUserAccountHandleForDevice(x);
				InputUserAccountHandle? userAccountHandleForDevice2 = GetUserAccountHandleForDevice(x);
				InputUserAccountHandle? inputUserAccountHandle = userAccountHandleForDevice;
				InputUserAccountHandle inputUserAccountHandle2 = platformUserAccountHandle;
				if (inputUserAccountHandle.HasValue && (!inputUserAccountHandle.HasValue || inputUserAccountHandle.GetValueOrDefault() == inputUserAccountHandle2) && userAccountHandleForDevice2 == platformUserAccountHandle)
				{
					return 0;
				}
				if (userAccountHandleForDevice == platformUserAccountHandle)
				{
					return -1;
				}
				if (userAccountHandleForDevice2 == platformUserAccountHandle)
				{
					return 1;
				}
				return 0;
			}

			private static InputUserAccountHandle? GetUserAccountHandleForDevice(InputDevice device)
			{
				return null;
			}
		}

		private struct OngoingAccountSelection
		{
			public InputDevice device;

			public uint userId;
		}

		public const uint InvalidId = 0u;

		private uint m_Id;

		private static int s_PairingStateVersion;

		private static uint s_LastUserId;

		private static int s_AllUserCount;

		private static int s_AllPairedDeviceCount;

		private static int s_AllLostDeviceCount;

		private static InputUser[] s_AllUsers;

		private static UserData[] s_AllUserData;

		private static InputDevice[] s_AllPairedDevices;

		private static InputDevice[] s_AllLostDevices;

		private static InlinedArray<OngoingAccountSelection> s_OngoingAccountSelections;

		private static InlinedArray<Action<InputUser, InputUserChange, InputDevice>> s_OnChange;

		private static InlinedArray<Action<InputControl, InputEventPtr>> s_OnUnpairedDeviceUsed;

		private static Action<object, InputActionChange> s_ActionChangeDelegate;

		private static Action<InputDevice, InputDeviceChange> s_OnDeviceChangeDelegate;

		private static Action<InputEventPtr, InputDevice> s_OnEventDelegate;

		private static bool s_OnActionChangeHooked;

		private static bool s_OnDeviceChangeHooked;

		private static bool s_OnEventHooked;

		private static int s_ListenForUnpairedDeviceActivity;

		public bool valid
		{
			get
			{
				if (m_Id == 0)
				{
					return false;
				}
				for (int i = 0; i < s_AllUserCount; i++)
				{
					if (s_AllUsers[i].m_Id == m_Id)
					{
						return true;
					}
				}
				return false;
			}
		}

		public int index
		{
			get
			{
				if (m_Id == 0)
				{
					throw new InvalidOperationException("Invalid user");
				}
				int num = TryFindUserIndex(m_Id);
				if (num == -1)
				{
					throw new InvalidOperationException(string.Format("User with ID {0} is no longer valid", m_Id));
				}
				return num;
			}
		}

		public uint id
		{
			get
			{
				return m_Id;
			}
		}

		public InputUserAccountHandle? platformUserAccountHandle
		{
			get
			{
				return s_AllUserData[index].platformUserAccountHandle;
			}
		}

		public string platformUserAccountName
		{
			get
			{
				return s_AllUserData[index].platformUserAccountName;
			}
		}

		public string platformUserAccountId
		{
			get
			{
				return s_AllUserData[index].platformUserAccountId;
			}
		}

		public ReadOnlyArray<InputDevice> pairedDevices
		{
			get
			{
				int num = index;
				return new ReadOnlyArray<InputDevice>(s_AllPairedDevices, s_AllUserData[num].deviceStartIndex, s_AllUserData[num].deviceCount);
			}
		}

		public ReadOnlyArray<InputDevice> lostDevices
		{
			get
			{
				int num = index;
				return new ReadOnlyArray<InputDevice>(s_AllLostDevices, s_AllUserData[num].lostDeviceStartIndex, s_AllUserData[num].lostDeviceCount);
			}
		}

		public IInputActionCollection actions
		{
			get
			{
				return s_AllUserData[index].actions;
			}
		}

		public InputControlScheme? controlScheme
		{
			get
			{
				return s_AllUserData[index].controlScheme;
			}
		}

		public InputControlScheme.MatchResult controlSchemeMatch
		{
			get
			{
				return s_AllUserData[index].controlSchemeMatch;
			}
		}

		public bool hasMissingRequiredDevices
		{
			get
			{
				return s_AllUserData[index].controlSchemeMatch.hasMissingRequiredDevices;
			}
		}

		public static ReadOnlyArray<InputUser> all
		{
			get
			{
				return new ReadOnlyArray<InputUser>(s_AllUsers, 0, s_AllUserCount);
			}
		}

		public static int listenForUnpairedDeviceActivity
		{
			get
			{
				return s_ListenForUnpairedDeviceActivity;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "Cannot be negative");
				}
				if (value > 0 && s_OnUnpairedDeviceUsed.length > 0)
				{
					HookIntoEvents();
				}
				else if (value == 0)
				{
					UnhookFromDeviceStateChange();
				}
				s_ListenForUnpairedDeviceActivity = value;
			}
		}

		public static event Action<InputUser, InputUserChange, InputDevice> onChange
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				s_OnChange.AppendWithCapacity(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				int num = s_OnChange.IndexOf(value);
				if (num != -1)
				{
					s_OnChange.RemoveAtWithCapacity(num);
				}
			}
		}

		public static event Action<InputControl, InputEventPtr> onUnpairedDeviceUsed
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				s_OnUnpairedDeviceUsed.AppendWithCapacity(value);
				if (s_ListenForUnpairedDeviceActivity > 0)
				{
					HookIntoEvents();
				}
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				int num = s_OnUnpairedDeviceUsed.IndexOf(value);
				if (num != -1)
				{
					s_OnUnpairedDeviceUsed.RemoveAtWithCapacity(num);
				}
				if (s_OnUnpairedDeviceUsed.length == 0)
				{
					UnhookFromDeviceStateChange();
				}
			}
		}

		public void AssociateActionsWithUser(IInputActionCollection actions)
		{
			int num = index;
			if (s_AllUserData[num].actions == actions)
			{
				return;
			}
			IInputActionCollection inputActionCollection = s_AllUserData[num].actions;
			if (inputActionCollection != null)
			{
				inputActionCollection.devices = null;
				inputActionCollection.bindingMask = null;
			}
			s_AllUserData[num].actions = actions;
			if (actions != null)
			{
				HookIntoActionChange();
				actions.devices = pairedDevices;
				if (s_AllUserData[num].controlScheme.HasValue)
				{
					ActivateControlSchemeInternal(num, s_AllUserData[num].controlScheme.Value);
				}
			}
		}

		public ControlSchemeChangeSyntax ActivateControlScheme(string schemeName)
		{
			InputControlScheme inputControlScheme = default(InputControlScheme);
			if (!string.IsNullOrEmpty(schemeName))
			{
				int num = index;
				if (s_AllUserData[num].actions == null)
				{
					throw new InvalidOperationException(string.Format("Cannot set control scheme '{0}' by name on user #{1} as not actions have been associated with the user yet (AssociateActionsWithUser)", schemeName, num));
				}
				ReadOnlyArray<InputControlScheme> controlSchemes = s_AllUserData[num].actions.controlSchemes;
				for (int i = 0; i < controlSchemes.Count; i++)
				{
					if (string.Compare(controlSchemes[i].name, schemeName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						inputControlScheme = controlSchemes[i];
						break;
					}
				}
				if (inputControlScheme == default(InputControlScheme))
				{
					throw new ArgumentException(string.Format("Cannot find control scheme '{0}' in actions '{1}'", schemeName, s_AllUserData[num].actions));
				}
			}
			return ActivateControlScheme(inputControlScheme);
		}

		public ControlSchemeChangeSyntax ActivateControlScheme(InputControlScheme scheme)
		{
			int num = index;
			InputControlScheme? inputControlScheme = s_AllUserData[num].controlScheme;
			InputControlScheme inputControlScheme2 = scheme;
			if (!inputControlScheme.HasValue || (inputControlScheme.HasValue && inputControlScheme.GetValueOrDefault() != inputControlScheme2) || (scheme == default(InputControlScheme) && s_AllUserData[num].controlScheme.HasValue))
			{
				ActivateControlSchemeInternal(num, scheme);
				Notify(num, InputUserChange.ControlSchemeChanged, null);
			}
			ControlSchemeChangeSyntax result = default(ControlSchemeChangeSyntax);
			result.m_UserIndex = num;
			return result;
		}

		private void ActivateControlSchemeInternal(int userIndex, InputControlScheme scheme)
		{
			bool flag = scheme == default(InputControlScheme);
			if (flag)
			{
				s_AllUserData[userIndex].controlScheme = null;
			}
			else
			{
				s_AllUserData[userIndex].controlScheme = scheme;
			}
			if (s_AllUserData[userIndex].actions != null)
			{
				if (flag)
				{
					s_AllUserData[userIndex].actions.bindingMask = null;
					s_AllUserData[userIndex].controlSchemeMatch.Dispose();
					s_AllUserData[userIndex].controlSchemeMatch = default(InputControlScheme.MatchResult);
				}
				else
				{
					s_AllUserData[userIndex].actions.bindingMask = new InputBinding
					{
						groups = scheme.bindingGroup
					};
					UpdateControlSchemeMatch(userIndex);
				}
			}
		}

		public void UnpairDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			int userIndex = index;
			if (pairedDevices.ContainsReference(device))
			{
				RemoveDeviceFromUser(userIndex, device);
			}
		}

		public void UnpairDevices()
		{
			int num = index;
			int deviceCount = s_AllUserData[num].deviceCount;
			int deviceStartIndex = s_AllUserData[num].deviceStartIndex;
			s_AllUserData[num].deviceCount = 0;
			s_AllUserData[num].deviceStartIndex = 0;
			IInputActionCollection inputActionCollection = s_AllUserData[num].actions;
			if (inputActionCollection != null)
			{
				inputActionCollection.devices = null;
			}
			if (s_AllUserData[num].controlScheme.HasValue)
			{
				UpdateControlSchemeMatch(num);
			}
			for (int i = 0; i < deviceCount; i++)
			{
				Notify(num, InputUserChange.DeviceUnpaired, s_AllPairedDevices[deviceStartIndex + i]);
			}
			ArrayHelpers.EraseSliceWithCapacity(ref s_AllPairedDevices, ref s_AllPairedDeviceCount, deviceStartIndex, deviceCount);
			if (s_AllUserData[num].lostDeviceCount > 0)
			{
				ArrayHelpers.EraseSliceWithCapacity(ref s_AllLostDevices, ref s_AllLostDeviceCount, s_AllUserData[num].lostDeviceStartIndex, s_AllUserData[num].lostDeviceCount);
				s_AllUserData[num].lostDeviceCount = 0;
				s_AllUserData[num].lostDeviceStartIndex = 0;
			}
			for (int j = 0; j < s_AllUserCount; j++)
			{
				if (s_AllUserData[j].deviceStartIndex > deviceStartIndex)
				{
					s_AllUserData[j].deviceStartIndex -= deviceCount;
				}
			}
		}

		public void UnpairDevicesAndRemoveUser()
		{
			UnpairDevices();
			RemoveUser(index);
			m_Id = 0u;
		}

		public static InputControlList<InputDevice> GetUnpairedInputDevices()
		{
			InputControlList<InputDevice> list = new InputControlList<InputDevice>(Allocator.Temp);
			GetUnpairedInputDevices(ref list);
			return list;
		}

		public static int GetUnpairedInputDevices(ref InputControlList<InputDevice> list)
		{
			int count = list.Count;
			foreach (InputDevice device in InputSystem.devices)
			{
				if (!ArrayHelpers.ContainsReference(s_AllPairedDevices, s_AllPairedDeviceCount, device))
				{
					list.Add(device);
				}
			}
			return list.Count - count;
		}

		public static InputUser? FindUserPairedToDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			int num = TryFindUserIndex(device);
			if (num == -1)
			{
				return null;
			}
			return s_AllUsers[num];
		}

		public static InputUser? FindUserByAccount(InputUserAccountHandle platformUserAccountHandle)
		{
			if (platformUserAccountHandle == default(InputUserAccountHandle))
			{
				throw new ArgumentException("Empty platform user account handle", "platformUserAccountHandle");
			}
			int num = TryFindUserIndex(platformUserAccountHandle);
			if (num == -1)
			{
				return null;
			}
			return s_AllUsers[num];
		}

		public static InputUser CreateUserWithoutPairedDevices()
		{
			int num = AddUser();
			return s_AllUsers[num];
		}

		public static InputUser PerformPairingWithDevice(InputDevice device, InputUser user = default(InputUser), InputUserPairingOptions options = InputUserPairingOptions.None)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (user != default(InputUser) && !user.valid)
			{
				throw new ArgumentException("Invalid user", "user");
			}
			int num;
			if (user == default(InputUser))
			{
				num = AddUser();
			}
			else
			{
				num = user.index;
				if ((options & InputUserPairingOptions.UnpairCurrentDevicesFromUser) != 0)
				{
					user.UnpairDevices();
				}
				if (user.pairedDevices.ContainsReference(device))
				{
					if ((options & InputUserPairingOptions.ForcePlatformUserAccountSelection) != 0)
					{
						InitiateUserAccountSelection(num, device, options);
					}
					return user;
				}
			}
			if (!InitiateUserAccountSelection(num, device, options))
			{
				AddDeviceToUser(num, device);
			}
			return s_AllUsers[num];
		}

		private static bool InitiateUserAccountSelection(int userIndex, InputDevice device, InputUserPairingOptions options)
		{
			long num = (((options & InputUserPairingOptions.ForcePlatformUserAccountSelection) == 0) ? UpdatePlatformUserAccount(userIndex, device) : 0);
			if (((options & InputUserPairingOptions.ForcePlatformUserAccountSelection) != 0 || (num != -1 && (num & 2) == 0L && (options & InputUserPairingOptions.ForceNoPlatformUserAccountSelection) == 0)) && InitiateUserAccountSelectionAtPlatformLevel(device))
			{
				s_AllUserData[userIndex].flags |= UserFlags.UserAccountSelectionInProgress;
				s_OngoingAccountSelections.Append(new OngoingAccountSelection
				{
					device = device,
					userId = s_AllUsers[userIndex].id
				});
				HookIntoDeviceChange();
				Notify(userIndex, InputUserChange.AccountSelectionInProgress, device);
				return true;
			}
			return false;
		}

		public bool Equals(InputUser other)
		{
			return m_Id == other.m_Id;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is InputUser)
			{
				return Equals((InputUser)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)m_Id;
		}

		public static bool operator ==(InputUser left, InputUser right)
		{
			return left.m_Id == right.m_Id;
		}

		public static bool operator !=(InputUser left, InputUser right)
		{
			return left.m_Id != right.m_Id;
		}

		private static int AddUser()
		{
			uint num = ++s_LastUserId;
			int count = s_AllUserCount;
			ArrayHelpers.AppendWithCapacity(ref s_AllUsers, ref count, new InputUser
			{
				m_Id = num
			});
			int num2 = ArrayHelpers.AppendWithCapacity(ref s_AllUserData, ref s_AllUserCount, default(UserData));
			Notify(num2, InputUserChange.Added, null);
			return num2;
		}

		private static void RemoveUser(int userIndex)
		{
			if (s_AllUserData[userIndex].controlScheme.HasValue && s_AllUserData[userIndex].actions != null)
			{
				s_AllUserData[userIndex].actions.bindingMask = null;
			}
			s_AllUserData[userIndex].controlSchemeMatch.Dispose();
			int lostDeviceCount = s_AllUserData[userIndex].lostDeviceCount;
			if (lostDeviceCount > 0)
			{
				ArrayHelpers.EraseSliceWithCapacity(ref s_AllLostDevices, ref s_AllLostDeviceCount, s_AllUserData[userIndex].lostDeviceStartIndex, lostDeviceCount);
			}
			for (int i = 0; i < s_OngoingAccountSelections.length; i++)
			{
				if (s_OngoingAccountSelections[i].userId == s_AllUsers[userIndex].id)
				{
					s_OngoingAccountSelections.RemoveAtByMovingTailWithCapacity(i);
					i--;
				}
			}
			Notify(userIndex, InputUserChange.Removed, null);
			int count = s_AllUserCount;
			ArrayHelpers.EraseAtWithCapacity(s_AllUsers, ref count, userIndex);
			ArrayHelpers.EraseAtWithCapacity(s_AllUserData, ref s_AllUserCount, userIndex);
			if (s_AllUserCount == 0)
			{
				UnhookFromDeviceChange();
				UnhookFromActionChange();
			}
		}

		private static void Notify(int userIndex, InputUserChange change, InputDevice device)
		{
			for (int i = 0; i < s_OnChange.length; i++)
			{
				s_OnChange[i](s_AllUsers[userIndex], change, device);
			}
		}

		private static int TryFindUserIndex(uint userId)
		{
			for (int i = 0; i < s_AllUserCount; i++)
			{
				if (s_AllUsers[i].m_Id == userId)
				{
					return i;
				}
			}
			return -1;
		}

		private static int TryFindUserIndex(InputUserAccountHandle platformHandle)
		{
			for (int i = 0; i < s_AllUserCount; i++)
			{
				if (s_AllUserData[i].platformUserAccountHandle == platformHandle)
				{
					return i;
				}
			}
			return -1;
		}

		private static int TryFindUserIndex(InputDevice device)
		{
			int num = s_AllPairedDevices.IndexOfReference(device, s_AllPairedDeviceCount);
			if (num == -1)
			{
				return -1;
			}
			for (int i = 0; i < s_AllUserCount; i++)
			{
				int deviceStartIndex = s_AllUserData[i].deviceStartIndex;
				if (deviceStartIndex <= num && num < deviceStartIndex + s_AllUserData[i].deviceCount)
				{
					return i;
				}
			}
			return -1;
		}

		private static void AddDeviceToUser(int userIndex, InputDevice device, bool asLostDevice = false, bool dontUpdateControlScheme = false)
		{
			int num = (asLostDevice ? s_AllUserData[userIndex].lostDeviceCount : s_AllUserData[userIndex].deviceCount);
			int num2 = (asLostDevice ? s_AllUserData[userIndex].lostDeviceStartIndex : s_AllUserData[userIndex].deviceStartIndex);
			s_PairingStateVersion++;
			if (num > 0)
			{
				ArrayHelpers.MoveSlice(asLostDevice ? s_AllLostDevices : s_AllPairedDevices, num2, asLostDevice ? (s_AllLostDeviceCount - num) : (s_AllPairedDeviceCount - num), num);
				for (int i = 0; i < s_AllUserCount; i++)
				{
					if (i != userIndex && (asLostDevice ? s_AllUserData[i].lostDeviceStartIndex : s_AllUserData[i].deviceStartIndex) > num2)
					{
						if (asLostDevice)
						{
							s_AllUserData[i].lostDeviceStartIndex -= num;
						}
						else
						{
							s_AllUserData[i].deviceStartIndex -= num;
						}
					}
				}
			}
			if (asLostDevice)
			{
				s_AllUserData[userIndex].lostDeviceStartIndex = s_AllLostDeviceCount - num;
				ArrayHelpers.AppendWithCapacity(ref s_AllLostDevices, ref s_AllLostDeviceCount, device);
				s_AllUserData[userIndex].lostDeviceCount++;
			}
			else
			{
				s_AllUserData[userIndex].deviceStartIndex = s_AllPairedDeviceCount - num;
				ArrayHelpers.AppendWithCapacity(ref s_AllPairedDevices, ref s_AllPairedDeviceCount, device);
				s_AllUserData[userIndex].deviceCount++;
				IInputActionCollection inputActionCollection = s_AllUserData[userIndex].actions;
				if (inputActionCollection != null)
				{
					inputActionCollection.devices = s_AllUsers[userIndex].pairedDevices;
					if (!dontUpdateControlScheme && s_AllUserData[userIndex].controlScheme.HasValue)
					{
						UpdateControlSchemeMatch(userIndex);
					}
				}
			}
			HookIntoDeviceChange();
			Notify(userIndex, asLostDevice ? InputUserChange.DeviceLost : InputUserChange.DevicePaired, device);
		}

		private static void RemoveDeviceFromUser(int userIndex, InputDevice device, bool asLostDevice = false)
		{
			int num = (asLostDevice ? s_AllLostDevices.IndexOfReference(device, s_AllLostDeviceCount) : s_AllPairedDevices.IndexOfReference(device, s_AllPairedDeviceCount));
			if (num == -1)
			{
				return;
			}
			if (asLostDevice)
			{
				ArrayHelpers.EraseAtWithCapacity(s_AllLostDevices, ref s_AllLostDeviceCount, num);
				s_AllUserData[userIndex].lostDeviceCount--;
			}
			else
			{
				s_PairingStateVersion--;
				ArrayHelpers.EraseAtWithCapacity(s_AllPairedDevices, ref s_AllPairedDeviceCount, num);
				s_AllUserData[userIndex].deviceCount--;
			}
			for (int i = 0; i < s_AllUserCount; i++)
			{
				if ((asLostDevice ? s_AllUserData[i].lostDeviceStartIndex : s_AllUserData[i].deviceStartIndex) > num)
				{
					if (asLostDevice)
					{
						s_AllUserData[i].lostDeviceStartIndex--;
					}
					else
					{
						s_AllUserData[i].deviceStartIndex--;
					}
				}
			}
			if (asLostDevice)
			{
				return;
			}
			for (int j = 0; j < s_OngoingAccountSelections.length; j++)
			{
				if (s_OngoingAccountSelections[j].userId == s_AllUsers[userIndex].id && s_OngoingAccountSelections[j].device == device)
				{
					s_OngoingAccountSelections.RemoveAtByMovingTailWithCapacity(j);
					j--;
				}
			}
			IInputActionCollection inputActionCollection = s_AllUserData[userIndex].actions;
			if (inputActionCollection != null)
			{
				inputActionCollection.devices = s_AllUsers[userIndex].pairedDevices;
				if (s_AllUsers[userIndex].controlScheme.HasValue)
				{
					UpdateControlSchemeMatch(userIndex);
				}
			}
			Notify(userIndex, InputUserChange.DeviceUnpaired, device);
		}

		private static void UpdateControlSchemeMatch(int userIndex, bool autoPairMissing = false)
		{
			if (!s_AllUserData[userIndex].controlScheme.HasValue)
			{
				return;
			}
			s_AllUserData[userIndex].controlSchemeMatch.Dispose();
			InputControlScheme.MatchResult matchResult = default(InputControlScheme.MatchResult);
			try
			{
				InputControlScheme value = s_AllUserData[userIndex].controlScheme.Value;
				if (value.deviceRequirements.Count > 0)
				{
					InputControlList<InputDevice> list = new InputControlList<InputDevice>(Allocator.Temp);
					try
					{
						list.AddSlice(s_AllUsers[userIndex].pairedDevices);
						if (autoPairMissing)
						{
							int count = list.Count;
							int unpairedInputDevices = GetUnpairedInputDevices(ref list);
							if (s_AllUserData[userIndex].platformUserAccountHandle.HasValue)
							{
								list.Sort(count, unpairedInputDevices, new CompareDevicesByUserAccount
								{
									platformUserAccountHandle = s_AllUserData[userIndex].platformUserAccountHandle.Value
								});
							}
						}
						matchResult = value.PickDevicesFrom(list);
						if (matchResult.isSuccessfulMatch)
						{
							if (s_AllUserData[userIndex].lostDeviceCount > 0)
							{
								ArrayHelpers.EraseSliceWithCapacity(ref s_AllLostDevices, ref s_AllLostDeviceCount, s_AllUserData[userIndex].lostDeviceStartIndex, s_AllUserData[userIndex].lostDeviceCount);
							}
							if (autoPairMissing)
							{
								s_AllUserData[userIndex].controlSchemeMatch = matchResult;
								foreach (InputDevice device in matchResult.devices)
								{
									if (!s_AllUsers[userIndex].pairedDevices.ContainsReference(device))
									{
										AddDeviceToUser(userIndex, device, false, true);
									}
								}
							}
						}
					}
					finally
					{
						list.Dispose();
					}
				}
				s_AllUserData[userIndex].controlSchemeMatch = matchResult;
			}
			catch (Exception)
			{
				matchResult.Dispose();
				throw;
			}
		}

		private static long UpdatePlatformUserAccount(int userIndex, InputDevice device)
		{
			InputUserAccountHandle? platformAccountHandle;
			string platformAccountName;
			string platformAccountId;
			long num = QueryPairedPlatformUserAccount(device, out platformAccountHandle, out platformAccountName, out platformAccountId);
			if (num == -1)
			{
				if ((s_AllUserData[userIndex].flags & UserFlags.UserAccountSelectionInProgress) != 0)
				{
					Notify(userIndex, InputUserChange.AccountSelectionCanceled, null);
				}
				s_AllUserData[userIndex].platformUserAccountHandle = null;
				s_AllUserData[userIndex].platformUserAccountName = null;
				s_AllUserData[userIndex].platformUserAccountId = null;
				return num;
			}
			if ((s_AllUserData[userIndex].flags & UserFlags.UserAccountSelectionInProgress) != 0)
			{
				if ((num & 4) == 0L)
				{
					if ((num & 0x10) != 0L)
					{
						Notify(userIndex, InputUserChange.AccountSelectionCanceled, device);
					}
					else
					{
						s_AllUserData[userIndex].flags &= ~UserFlags.UserAccountSelectionInProgress;
						s_AllUserData[userIndex].platformUserAccountHandle = platformAccountHandle;
						s_AllUserData[userIndex].platformUserAccountName = platformAccountName;
						s_AllUserData[userIndex].platformUserAccountId = platformAccountId;
						Notify(userIndex, InputUserChange.AccountSelectionComplete, device);
					}
				}
			}
			else
			{
				InputUserAccountHandle? inputUserAccountHandle = s_AllUserData[userIndex].platformUserAccountHandle;
				InputUserAccountHandle? inputUserAccountHandle2 = platformAccountHandle;
				if (inputUserAccountHandle.HasValue != inputUserAccountHandle2.HasValue || (inputUserAccountHandle.HasValue && inputUserAccountHandle.GetValueOrDefault() != inputUserAccountHandle2.GetValueOrDefault()) || s_AllUserData[userIndex].platformUserAccountId != platformAccountId)
				{
					s_AllUserData[userIndex].platformUserAccountHandle = platformAccountHandle;
					s_AllUserData[userIndex].platformUserAccountName = platformAccountName;
					s_AllUserData[userIndex].platformUserAccountId = platformAccountId;
					Notify(userIndex, InputUserChange.AccountChanged, device);
				}
				else if (s_AllUserData[userIndex].platformUserAccountName != platformAccountName)
				{
					Notify(userIndex, InputUserChange.AccountNameChanged, device);
				}
			}
			return num;
		}

		private static long QueryPairedPlatformUserAccount(InputDevice device, out InputUserAccountHandle? platformAccountHandle, out string platformAccountName, out string platformAccountId)
		{
			QueryPairedUserAccountCommand command = QueryPairedUserAccountCommand.Create();
			long num = device.ExecuteCommand(ref command);
			if (num == -1)
			{
				platformAccountHandle = null;
				platformAccountName = null;
				platformAccountId = null;
				return -1L;
			}
			if ((num & 2) != 0L)
			{
				platformAccountHandle = new InputUserAccountHandle(device.description.interfaceName ?? "<Unknown>", command.handle);
				platformAccountName = command.name;
				platformAccountId = command.id;
			}
			else
			{
				platformAccountHandle = null;
				platformAccountName = null;
				platformAccountId = null;
			}
			return num;
		}

		private static bool InitiateUserAccountSelectionAtPlatformLevel(InputDevice device)
		{
			InitiateUserAccountPairingCommand command = InitiateUserAccountPairingCommand.Create();
			long num = device.ExecuteCommand(ref command);
			if (num == -2)
			{
				throw new InvalidOperationException("User pairing already in progress");
			}
			return num == 1;
		}

		private static void OnActionChange(object obj, InputActionChange change)
		{
			if (change != InputActionChange.BoundControlsChanged)
			{
				return;
			}
			for (int i = 0; i < s_AllUserCount; i++)
			{
				if (s_AllUsers[i].actions == obj)
				{
					Notify(i, InputUserChange.ControlsChanged, null);
				}
			}
		}

		private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			switch (change)
			{
			case InputDeviceChange.Removed:
			{
				for (int num4 = s_AllPairedDevices.IndexOfReference(device, s_AllPairedDeviceCount); num4 != -1; num4 = s_AllPairedDevices.IndexOfReference(device, num4 + 1, s_AllPairedDeviceCount))
				{
					int userIndex3 = -1;
					for (int l = 0; l < s_AllUserCount; l++)
					{
						int deviceStartIndex2 = s_AllUserData[l].deviceStartIndex;
						if (deviceStartIndex2 <= num4 && num4 < deviceStartIndex2 + s_AllUserData[l].deviceCount)
						{
							userIndex3 = l;
							break;
						}
					}
					AddDeviceToUser(userIndex3, device, true);
					RemoveDeviceFromUser(userIndex3, device);
				}
				break;
			}
			case InputDeviceChange.Added:
			{
				for (int num3 = s_AllLostDevices.IndexOfReference(device, s_AllLostDeviceCount); num3 != -1; num3 = s_AllLostDevices.IndexOfReference(device, num3 + 1, s_AllLostDeviceCount))
				{
					int userIndex2 = -1;
					for (int k = 0; k < s_AllUserCount; k++)
					{
						int lostDeviceStartIndex = s_AllUserData[k].lostDeviceStartIndex;
						if (lostDeviceStartIndex <= num3 && num3 < lostDeviceStartIndex + s_AllUserData[k].lostDeviceCount)
						{
							userIndex2 = k;
							break;
						}
					}
					RemoveDeviceFromUser(userIndex2, device, true);
					Notify(userIndex2, InputUserChange.DeviceRegained, device);
					AddDeviceToUser(userIndex2, device);
				}
				break;
			}
			case InputDeviceChange.ConfigurationChanged:
			{
				bool flag = false;
				for (int i = 0; i < s_OngoingAccountSelections.length; i++)
				{
					if (s_OngoingAccountSelections[i].device != device)
					{
						continue;
					}
					InputUser inputUser = default(InputUser);
					inputUser.m_Id = s_OngoingAccountSelections[i].userId;
					int num = inputUser.index;
					if ((UpdatePlatformUserAccount(num, device) & 4) == 0L)
					{
						flag = true;
						s_OngoingAccountSelections.RemoveAtByMovingTailWithCapacity(i);
						i--;
						if (!s_AllUsers[num].pairedDevices.ContainsReference(device))
						{
							AddDeviceToUser(num, device);
						}
					}
				}
				if (flag)
				{
					break;
				}
				for (int num2 = s_AllPairedDevices.IndexOfReference(device, s_AllPairedDeviceCount); num2 != -1; num2 = s_AllPairedDevices.IndexOfReference(device, num2 + 1, s_AllPairedDeviceCount))
				{
					int userIndex = -1;
					for (int j = 0; j < s_AllUserCount; j++)
					{
						int deviceStartIndex = s_AllUserData[j].deviceStartIndex;
						if (deviceStartIndex <= num2 && num2 < deviceStartIndex + s_AllUserData[j].deviceCount)
						{
							userIndex = j;
							break;
						}
					}
					UpdatePlatformUserAccount(userIndex, device);
				}
				break;
			}
			}
		}

		private unsafe static void OnEvent(InputEventPtr eventPtr, InputDevice device)
		{
			if (s_ListenForUnpairedDeviceActivity == 0 || (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) || ArrayHelpers.ContainsReference(s_AllPairedDevices, s_AllPairedDeviceCount, device))
			{
				return;
			}
			ReadOnlyArray<InputControl> allControls = device.allControls;
			for (int i = 0; i < allControls.Count; i++)
			{
				InputControl inputControl = allControls[i];
				if (inputControl.noisy || inputControl.synthetic || inputControl.children.Count > 0)
				{
					continue;
				}
				void* statePtrFromStateEvent = inputControl.GetStatePtrFromStateEvent(eventPtr);
				if (statePtrFromStateEvent == null || inputControl.CheckStateIsAtDefault(statePtrFromStateEvent, null))
				{
					continue;
				}
				float num = inputControl.EvaluateMagnitude(statePtrFromStateEvent);
				if (!(num > 0f) && num != -1f)
				{
					continue;
				}
				bool flag = false;
				for (int j = 0; j < s_OnUnpairedDeviceUsed.length; j++)
				{
					int num2 = s_PairingStateVersion;
					s_OnUnpairedDeviceUsed[j](inputControl, eventPtr);
					if (num2 != s_PairingStateVersion && FindUserPairedToDevice(device).HasValue)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}

		private static void HookIntoActionChange()
		{
			if (!s_OnActionChangeHooked)
			{
				if (s_ActionChangeDelegate == null)
				{
					s_ActionChangeDelegate = OnActionChange;
				}
				InputSystem.onActionChange += OnActionChange;
				s_OnActionChangeHooked = true;
			}
		}

		private static void UnhookFromActionChange()
		{
			if (s_OnActionChangeHooked)
			{
				InputSystem.onActionChange -= OnActionChange;
				s_OnActionChangeHooked = true;
			}
		}

		private static void HookIntoDeviceChange()
		{
			if (!s_OnDeviceChangeHooked)
			{
				if (s_OnDeviceChangeDelegate == null)
				{
					s_OnDeviceChangeDelegate = OnDeviceChange;
				}
				InputSystem.onDeviceChange += s_OnDeviceChangeDelegate;
				s_OnDeviceChangeHooked = true;
			}
		}

		private static void UnhookFromDeviceChange()
		{
			if (s_OnDeviceChangeHooked)
			{
				InputSystem.onDeviceChange -= s_OnDeviceChangeDelegate;
				s_OnDeviceChangeHooked = false;
			}
		}

		private static void HookIntoEvents()
		{
			if (!s_OnEventHooked)
			{
				if (s_OnEventDelegate == null)
				{
					s_OnEventDelegate = OnEvent;
				}
				InputSystem.onEvent += s_OnEventDelegate;
				s_OnEventHooked = true;
			}
		}

		private static void UnhookFromDeviceStateChange()
		{
			if (s_OnEventHooked)
			{
				InputSystem.onEvent -= s_OnEventDelegate;
				s_OnEventHooked = false;
			}
		}

		internal static void ResetGlobals()
		{
			for (int i = 0; i < s_AllUserCount; i++)
			{
				s_AllUserData[i].controlSchemeMatch.Dispose();
			}
			s_PairingStateVersion = 0;
			s_AllUserCount = 0;
			s_AllPairedDeviceCount = 0;
			s_AllUsers = null;
			s_AllUserData = null;
			s_AllPairedDevices = null;
			s_OngoingAccountSelections = default(InlinedArray<OngoingAccountSelection>);
			s_OnChange = default(InlinedArray<Action<InputUser, InputUserChange, InputDevice>>);
			s_OnUnpairedDeviceUsed = default(InlinedArray<Action<InputControl, InputEventPtr>>);
			s_OnDeviceChangeDelegate = null;
			s_OnEventDelegate = null;
			s_OnDeviceChangeHooked = false;
			s_OnActionChangeHooked = false;
			s_OnEventHooked = false;
			s_ListenForUnpairedDeviceActivity = 0;
		}
	}
}
