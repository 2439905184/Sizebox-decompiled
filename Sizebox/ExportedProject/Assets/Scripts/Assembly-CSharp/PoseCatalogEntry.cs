using UnityEngine;
using UnityEngine.UI;

public class PoseCatalogEntry : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private Button deleteButton;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Text text;

	public Button Button
	{
		get
		{
			return button;
		}
	}

	public Button DeleteButton
	{
		get
		{
			return deleteButton;
		}
	}

	public Image Image
	{
		get
		{
			return image;
		}
	}

	public Text Text
	{
		get
		{
			return text;
		}
	}

	public CustomPose CustomPoseData { get; private set; }

	public void SetPoseData(CustomPose poseData)
	{
		CustomPoseData = poseData;
		deleteButton.gameObject.SetActive(poseData != null);
	}
}
