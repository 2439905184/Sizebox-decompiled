using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[Preserve]
	public class KeyControl : ButtonControl
	{
		private int m_ScanCode;

		public Key keyCode { get; set; }

		public int scanCode
		{
			get
			{
				RefreshConfigurationIfNeeded();
				return m_ScanCode;
			}
		}

		protected override void RefreshConfiguration()
		{
			base.displayName = null;
			m_ScanCode = 0;
			QueryKeyNameCommand command = QueryKeyNameCommand.Create(keyCode);
			if (base.device.ExecuteCommand(ref command) > 0)
			{
				m_ScanCode = command.scanOrKeyCode;
				base.displayName = command.ReadKeyName();
			}
		}
	}
}
