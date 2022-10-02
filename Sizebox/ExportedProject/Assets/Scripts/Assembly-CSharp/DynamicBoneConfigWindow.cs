using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicBoneConfigWindow : MonoBehaviour
{
	[SerializeField]
	private Slider dampening;

	[SerializeField]
	private Slider elasticity;

	[SerializeField]
	private Slider stiffness;

	[SerializeField]
	private Slider inertia;

	[SerializeField]
	private Button doneButton;

	private List<DynamicBone> _targets = new List<DynamicBone>();

	private void Awake()
	{
		dampening.onValueChanged.AddListener(OnDampening);
		elasticity.onValueChanged.AddListener(OnElasticity);
		stiffness.onValueChanged.AddListener(OnStiffness);
		inertia.onValueChanged.AddListener(OnInertia);
		doneButton.onClick.AddListener(OnDone);
	}

	public void SetTargets(List<DynamicBone> targets)
	{
		_targets = targets;
		if (_targets.Count != 0)
		{
			DynamicBone dynamicBone = _targets[0];
			dampening.value = dynamicBone.m_Damping;
			elasticity.value = dynamicBone.m_Elasticity;
			stiffness.value = dynamicBone.m_Stiffness;
			inertia.value = dynamicBone.m_Inert;
		}
	}

	private void OnDampening(float value)
	{
		foreach (DynamicBone target in _targets)
		{
			target.m_Damping = value;
		}
	}

	private void OnElasticity(float value)
	{
		foreach (DynamicBone target in _targets)
		{
			target.m_Elasticity = value;
		}
	}

	private void OnStiffness(float value)
	{
		foreach (DynamicBone target in _targets)
		{
			target.m_Stiffness = value;
		}
	}

	private void OnInertia(float value)
	{
		foreach (DynamicBone target in _targets)
		{
			target.m_Inert = value;
		}
	}

	private void OnDone()
	{
		foreach (DynamicBone target in _targets)
		{
			target.UpdateParameters();
		}
		SetTargets(new List<DynamicBone>());
		base.gameObject.SetActive(false);
	}
}
