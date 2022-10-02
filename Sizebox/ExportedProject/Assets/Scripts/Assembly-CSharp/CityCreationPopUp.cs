using System;
using UnityEngine;
using UnityEngine.UI;

internal class CityCreationPopUp : MonoBehaviour
{
	private Slider popModifier;

	private Slider cityRadius;

	private Slider SkyScraperHeight;

	private Slider OpenSpaceChance;

	private InputField seedField;

	public int PopModifierValue { get; private set; }

	public int CityRadiusValue { get; private set; }

	public int SkyScraperHeightValue { get; private set; }

	public int OpenSpaceChanceValue { get; private set; }

	public bool AutoPoplulate { get; private set; }

	public string SeedValue { get; private set; }

	private event Action OnAccept;

	private event Action OnCancel;

	private event Action OnValueChanged;

	private void Start()
	{
		PopModifierValue = 0;
		CityRadiusValue = 80;
		SkyScraperHeightValue = 0;
		OpenSpaceChanceValue = 100;
		Text[] componentsInChildren = base.transform.GetComponentsInChildren<Text>();
		componentsInChildren[1].text = "Population Modifier";
		componentsInChildren[2].text = "City radius";
		componentsInChildren[3].text = "Skyscraper height";
		componentsInChildren[4].text = "Open space chance";
		Button[] componentsInChildren2 = base.transform.GetComponentsInChildren<Button>();
		componentsInChildren2[0].onClick.AddListener(Accept);
		componentsInChildren2[1].onClick.AddListener(Close);
		popModifier = GetSliderByTag(componentsInChildren[1]);
		cityRadius = GetSliderByTag(componentsInChildren[2]);
		SkyScraperHeight = GetSliderByTag(componentsInChildren[3]);
		OpenSpaceChance = GetSliderByTag(componentsInChildren[4]);
		base.transform.GetComponentsInChildren<Toggle>()[0].onValueChanged.AddListener(AutoPopulateSet);
		InputField[] componentsInChildren3 = base.transform.GetComponentsInChildren<InputField>();
		seedField = componentsInChildren3[0];
		Sbox.AddSBoxInputFieldEvents(seedField);
		seedField.characterLimit = 32;
		seedField.onValueChanged.AddListener(SeedChanged);
		SetMinMaxToSlider(popModifier, -100, 100, PopModifierValue);
		SetMinMaxToSlider(cityRadius, 80, 350, CityRadiusValue);
		SetMinMaxToSlider(SkyScraperHeight, 0, 100, SkyScraperHeightValue);
		SetMinMaxToSlider(OpenSpaceChance, 0, 100, OpenSpaceChanceValue);
		ReloadValues();
		popModifier.onValueChanged.AddListener(PopulationChanged);
		cityRadius.onValueChanged.AddListener(CityRadiusChanged);
		SkyScraperHeight.onValueChanged.AddListener(SkyScraperHeightChanged);
		OpenSpaceChance.onValueChanged.AddListener(OpenSpacesChanged);
		popModifier.interactable = true;
	}

	private void AutoPopulateSet(bool populationValue)
	{
		AutoPoplulate = populationValue;
		this.OnValueChanged();
	}

	private void Close()
	{
		Deconstruct();
		this.OnCancel();
	}

	private void OpenSpacesChanged(float newvalue)
	{
		OpenSpaceChanceValue = (int)newvalue;
	}

	private void SkyScraperHeightChanged(float newvalue)
	{
		SkyScraperHeightValue = (int)newvalue;
		this.OnValueChanged();
	}

	private void CityRadiusChanged(float newvalue)
	{
		CityRadiusValue = (int)newvalue;
		this.OnValueChanged();
	}

	private void PopulationChanged(float newvalue)
	{
		PopModifierValue = (int)newvalue;
	}

	private void SeedChanged(string newValue)
	{
		SeedValue = newValue;
	}

	private void ReloadValues()
	{
		popModifier.value = PopModifierValue;
		cityRadius.value = CityRadiusValue;
		SkyScraperHeight.value = SkyScraperHeightValue;
		OpenSpaceChance.value = OpenSpaceChanceValue;
		seedField.text = SeedValue;
	}

	private void Accept()
	{
		Deconstruct();
		this.OnAccept();
	}

	private void Deconstruct()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		popModifier.onValueChanged.RemoveAllListeners();
		SkyScraperHeight.onValueChanged.RemoveAllListeners();
		OpenSpaceChance.onValueChanged.RemoveAllListeners();
		cityRadius.onValueChanged.RemoveAllListeners();
		seedField.onValueChanged.RemoveAllListeners();
		Button[] componentsInChildren = base.transform.GetComponentsInChildren<Button>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onClick.RemoveAllListeners();
		}
		Toggle[] componentsInChildren2 = base.transform.GetComponentsInChildren<Toggle>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].onValueChanged.RemoveAllListeners();
		}
	}

	public void SetHandlers(Action onAccept, Action onCancel, Action onValueChanged)
	{
		this.OnAccept = onAccept;
		this.OnCancel = onCancel;
		this.OnValueChanged = onValueChanged;
		base.gameObject.SetActive(true);
	}

	private Slider GetSliderByTag(Text tag)
	{
		Slider component = tag.transform.parent.parent.GetComponent<Slider>();
		if (component == null)
		{
			Debug.LogError("Slider not found");
		}
		return component;
	}

	private void SetMinMaxToSlider(Slider slider, int min, int max, int initialValue)
	{
		slider.maxValue = max;
		slider.minValue = min;
		slider.value = initialValue;
	}
}
