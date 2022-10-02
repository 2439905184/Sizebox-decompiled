using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SizeboxUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class BaseView : MonoBehaviour
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_0
		{
			public ViewConfig viewConfig;

			public Button button;

			public BaseView _003C_003E4__this;

			internal void _003CAwake_003Eb__0()
			{
				_003C_003E4__this.OnButtonClick(viewConfig, button);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_1
		{
			public ViewConfig viewConfig;

			public Toggle toggle;

			public BaseView _003C_003E4__this;

			internal void _003CAwake_003Eb__1(bool value)
			{
				_003C_003E4__this.OnToggleClick(value, viewConfig, toggle);
			}
		}

		[Header("View Settings")]
		[SerializeField]
		protected List<ViewConfig> subViews = new List<ViewConfig>();

		[SerializeField]
		private bool disableOtherSubViews = true;

		[SerializeField]
		private bool disableSubViewsOnDisable;

		protected InterfaceControl Controller;

		protected virtual void Awake()
		{
			Controller = GuiManager.Instance.InterfaceControl;
			InterfaceControl controller = Controller;
			controller.onSelected = (UnityAction)Delegate.Combine(controller.onSelected, new UnityAction(OnSelection));
			if (subViews == null)
			{
				subViews = new List<ViewConfig>();
			}
			for (int i = 0; i < subViews.Count; i++)
			{
				if ((bool)subViews[i].activator)
				{
					if ((bool)(subViews[i].activator as Button))
					{
						_003C_003Ec__DisplayClass4_0 _003C_003Ec__DisplayClass4_ = new _003C_003Ec__DisplayClass4_0();
						_003C_003Ec__DisplayClass4_._003C_003E4__this = this;
						_003C_003Ec__DisplayClass4_.viewConfig = subViews[i];
						_003C_003Ec__DisplayClass4_.button = subViews[i].activator as Button;
						(_003C_003Ec__DisplayClass4_.viewConfig.activator as Button).onClick.AddListener(_003C_003Ec__DisplayClass4_._003CAwake_003Eb__0);
					}
					else if ((bool)(subViews[i].activator as Toggle))
					{
						_003C_003Ec__DisplayClass4_1 _003C_003Ec__DisplayClass4_2 = new _003C_003Ec__DisplayClass4_1();
						_003C_003Ec__DisplayClass4_2._003C_003E4__this = this;
						_003C_003Ec__DisplayClass4_2.viewConfig = subViews[i];
						_003C_003Ec__DisplayClass4_2.toggle = subViews[i].activator as Toggle;
						(_003C_003Ec__DisplayClass4_2.viewConfig.activator as Toggle).onValueChanged.AddListener(_003C_003Ec__DisplayClass4_2._003CAwake_003Eb__1);
					}
				}
			}
		}

		protected void Start()
		{
		}

		protected void OnToggleClick(bool value, ViewConfig viewConfig, Toggle toggle)
		{
			BaseView view = viewConfig.view;
			if (value)
			{
				if (disableOtherSubViews)
				{
					foreach (ViewConfig subView in subViews)
					{
						subView.view.gameObject.SetActive(false);
					}
				}
				view.gameObject.SetActive(true);
				viewConfig._OpenEvent();
			}
			else if (viewConfig.activatorIsToggle)
			{
				view.gameObject.SetActive(false);
			}
			else
			{
				toggle.isOn = true;
			}
		}

		protected void OnButtonClick(ViewConfig viewConfig, Button button)
		{
			BaseView view = viewConfig.view;
			if (view.gameObject.activeSelf && viewConfig.activatorIsToggle)
			{
				view.gameObject.SetActive(false);
			}
			else if (!view.gameObject.activeSelf)
			{
				if (disableOtherSubViews)
				{
					DisableAllSubViews();
				}
				view.gameObject.SetActive(true);
				viewConfig._OpenEvent();
				if (viewConfig.disableInteractionOnClick)
				{
					button.interactable = false;
				}
			}
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
			if (!disableSubViewsOnDisable)
			{
				return;
			}
			foreach (ViewConfig subView in subViews)
			{
				subView.view.gameObject.SetActive(false);
			}
		}

		protected virtual void OnSelection()
		{
			if (base.isActiveAndEnabled)
			{
				OnEnable();
			}
		}

		protected void DisableAllSubViews()
		{
			foreach (ViewConfig subView in subViews)
			{
				if (subView.disableInteractionOnClick)
				{
					subView.activator.interactable = true;
				}
				subView.view.gameObject.SetActive(false);
			}
		}

		protected void RegisterForViewEnabled(BaseView view, Action callback)
		{
			int subViewIndex = GetSubViewIndex(subViews, view);
			if (subViewIndex != -1)
			{
				subViews[subViewIndex].OnViewOpen += callback.Invoke;
			}
		}

		private static int GetSubViewIndex(List<ViewConfig> subviews, BaseView view)
		{
			for (int i = 0; i < subviews.Count; i++)
			{
				if (subviews[i].view == view)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
