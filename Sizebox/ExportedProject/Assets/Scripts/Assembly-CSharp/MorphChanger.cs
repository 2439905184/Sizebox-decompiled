using UnityEngine;

public class MorphChanger : MonoBehaviour
{
	private MMD4MecanimModel model;

	private MMD4MecanimModel.Morph[] morphs;

	private void Start()
	{
		model = GetComponent<MMD4MecanimModel>();
		morphs = model.morphList;
		for (int i = 0; i < morphs.Length; i++)
		{
			Debug.Log(morphs[i].morphData.nameEn + ", " + morphs[i].morphCategory);
			morphs[i].weight2 = 1f;
			Debug.Log("weigth 1: " + morphs[i].weight + ", weigth 2: " + morphs[i].weight2);
		}
	}

	private void Update()
	{
	}
}
