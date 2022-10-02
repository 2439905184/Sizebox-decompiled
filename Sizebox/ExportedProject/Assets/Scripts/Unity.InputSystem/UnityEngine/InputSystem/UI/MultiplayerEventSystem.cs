using UnityEngine.EventSystems;

namespace UnityEngine.InputSystem.UI
{
	public class MultiplayerEventSystem : EventSystem
	{
		[Tooltip("If set, only process mouse events for any game objects which are children of this game object.")]
		[SerializeField]
		private GameObject m_PlayerRoot;

		public GameObject playerRoot
		{
			get
			{
				return m_PlayerRoot;
			}
			set
			{
				m_PlayerRoot = value;
			}
		}

		protected override void Update()
		{
			EventSystem eventSystem = EventSystem.current;
			EventSystem.current = this;
			try
			{
				base.Update();
			}
			finally
			{
				EventSystem.current = eventSystem;
			}
		}
	}
}
