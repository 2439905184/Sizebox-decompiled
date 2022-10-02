using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Controller
{
	public class BreastPhysicsController : MonoBehaviour
	{
		private const float cModifier = 1000f;

		private Slider elasticitySlider;

		private Slider stiffnessSlider;

		private Slider inertSlider;

		private Slider dampeningSlider;

		private Toggle colliderToggle;

		public float Elasticity { get; set; }

		public float Stiffness { get; set; }

		public float Inert { get; set; }

		public float Dampening { get; set; }

		public bool Collider { get; set; }

		private event Action OnAccept;

		private event Action OnCancel;

		private event Action OnEnable;

		private event Action OnDisable;

		private void Awake()
		{
			Button[] componentsInChildren = base.transform.GetComponentsInChildren<Button>();
			componentsInChildren[0].onClick.AddListener(Enable);
			componentsInChildren[1].onClick.AddListener(Disable);
			componentsInChildren[2].onClick.AddListener(Accept);
			componentsInChildren[3].onClick.AddListener(Cancel);
			Slider[] componentsInChildren2 = base.transform.GetComponentsInChildren<Slider>();
			Slider[] array = componentsInChildren2;
			foreach (Slider obj in array)
			{
				obj.minValue = 0f;
				obj.maxValue = 1000f;
			}
			dampeningSlider = componentsInChildren2[0];
			elasticitySlider = componentsInChildren2[1];
			stiffnessSlider = componentsInChildren2[2];
			inertSlider = componentsInChildren2[3];
			Toggle[] componentsInChildren3 = base.transform.GetComponentsInChildren<Toggle>();
			colliderToggle = componentsInChildren3[0];
			dampeningSlider.onValueChanged.AddListener(DampeningChanged);
			elasticitySlider.onValueChanged.AddListener(ElasticityChanged);
			stiffnessSlider.onValueChanged.AddListener(StiffnessChanged);
			inertSlider.onValueChanged.AddListener(InertChanged);
			colliderToggle.onValueChanged.AddListener(ColliderChanged);
		}

		private void Deconstruct()
		{
			UnityEngine.Object.Destroy(base.gameObject);
			dampeningSlider.onValueChanged.RemoveAllListeners();
			elasticitySlider.onValueChanged.RemoveAllListeners();
			stiffnessSlider.onValueChanged.RemoveAllListeners();
			inertSlider.onValueChanged.RemoveAllListeners();
			colliderToggle.onValueChanged.RemoveAllListeners();
			Button[] componentsInChildren = base.transform.GetComponentsInChildren<Button>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].onClick.RemoveAllListeners();
			}
		}

		private void ColliderChanged(bool arg0)
		{
			Collider = colliderToggle.isOn;
		}

		private void InertChanged(float arg0)
		{
			Inert = inertSlider.value / 1000f;
		}

		private void StiffnessChanged(float arg0)
		{
			Stiffness = stiffnessSlider.value / 1000f;
		}

		private void ElasticityChanged(float arg0)
		{
			Elasticity = elasticitySlider.value / 1000f;
		}

		private void DampeningChanged(float arg0)
		{
			Dampening = dampeningSlider.value / 1000f;
		}

		private void Enable()
		{
			this.OnEnable();
		}

		private void Disable()
		{
			this.OnDisable();
		}

		private void Cancel()
		{
			Deconstruct();
			this.OnCancel();
		}

		private void Accept()
		{
			this.OnAccept();
			Deconstruct();
		}

		public void ReloadUi()
		{
			stiffnessSlider.value = Stiffness * 1000f;
			elasticitySlider.value = Elasticity * 1000f;
			inertSlider.value = Inert * 1000f;
			dampeningSlider.value = Dampening * 1000f;
			colliderToggle.isOn = Collider;
		}

		public void AttachActions(Action onAccept, Action onCancel, Action onEnable, Action onDisable)
		{
			this.OnAccept = onAccept;
			this.OnCancel = onCancel;
			this.OnEnable = onEnable;
			this.OnDisable = onDisable;
		}
	}
}
