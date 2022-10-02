using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Sizebox.CharacterEditor;
using SizeboxUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class PoseView : ViewCommon
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass24_0
	{
		public int number;

		public PoseCatalogEntry newEntry;

		public PoseView _003C_003E4__this;

		internal void _003CAwake_003Eb__1()
		{
			_003C_003E4__this.OnElementClick(number);
		}

		internal void _003CAwake_003Eb__2()
		{
			_003C_003E4__this.OnDeletePose(newEntry);
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<Pose, string> _003C_003E9__24_0;

		internal string _003CAwake_003Eb__24_0(Pose p)
		{
			return p.name;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass44_0
	{
		public PoseView _003C_003E4__this;

		public PoseCatalogEntry entry;

		internal void _003COnDeletePose_003Eb__0()
		{
			_003C_003E4__this.DeleteCustomPose(entry.CustomPoseData.name);
		}
	}

	private const string PoseExtension = ".pose";

	[Header("Required References")]
	[SerializeField]
	private Button previousButton;

	[SerializeField]
	private Button nextButton;

	[SerializeField]
	private Button customizeButton;

	[SerializeField]
	private Button saveButton;

	[SerializeField]
	private GridLayoutGroup grid;

	[SerializeField]
	private PoseCatalogEntry poseCatalogEntryPrefab;

	[SerializeField]
	private SaveDialog saveDialog;

	[SerializeField]
	private ConfirmationDialog confirmationDialog;

	[SerializeField]
	private PoseSearchBar searchBar;

	[SerializeField]
	private Sprite customPoseSprite;

	private int _thumbCount;

	private InterfaceControl _control;

	private PoseCatalogEntry[] _activeEntries;

	private EditPlacement _placement;

	private Color _originalColor;

	private int handleCount = 14;

	private PoseHandle _poseTarget;

	private PoseHandle[] _poseHandles;

	private Camera _mainCamera;

	private ThreadSafeList<Pose> _poses;

	private string _currentAnimation;

	private bool _customPoseEnabled;

	private IPosable _target;

	private readonly Dictionary<string, Pose> _customPoses = new Dictionary<string, Pose>();

	private string _folderPath;

	private void Awake()
	{
		Sprite[] array = Resources.LoadAll<Sprite>("PosesThumb");
		_poses = new ThreadSafeList<Pose>();
		Sprite[] array2 = array;
		foreach (Sprite sprite in array2)
		{
			_poses.Add(new Pose(sprite));
		}
		_mainCamera = Camera.main;
		_control = GuiManager.Instance.InterfaceControl;
		_placement = GuiManager.Instance.EditPlacement;
		saveButton.onClick.AddListener(OnSave);
		nextButton.onClick.AddListener(base.OnNext);
		previousButton.onClick.AddListener(base.OnPrevious);
		customizeButton.onClick.AddListener(OnCustomize);
		grid = GetComponentInChildren<GridLayoutGroup>();
		_poseTarget = Resources.Load<PoseHandle>("UI/Pose/Pose Target");
		Rect rect = grid.GetComponent<RectTransform>().rect;
		int num = (int)rect.width / 72;
		int num2 = (int)rect.height / 84;
		_thumbCount = num * num2;
		_activeEntries = new PoseCatalogEntry[_thumbCount];
		LoadCustomPoses();
		_originalColor = poseCatalogEntryPrefab.Image.color;
		for (int j = 0; j < _thumbCount; j++)
		{
			_003C_003Ec__DisplayClass24_0 _003C_003Ec__DisplayClass24_ = new _003C_003Ec__DisplayClass24_0();
			_003C_003Ec__DisplayClass24_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass24_.newEntry = UnityEngine.Object.Instantiate(poseCatalogEntryPrefab, grid.transform);
			_activeEntries[j] = _003C_003Ec__DisplayClass24_.newEntry;
			_003C_003Ec__DisplayClass24_.newEntry.name = "Pose " + j;
			_003C_003Ec__DisplayClass24_.number = j;
			_003C_003Ec__DisplayClass24_.newEntry.Button.onClick.AddListener(_003C_003Ec__DisplayClass24_._003CAwake_003Eb__1);
			_003C_003Ec__DisplayClass24_.newEntry.DeleteButton.onClick.AddListener(_003C_003Ec__DisplayClass24_._003CAwake_003Eb__2);
		}
		InterfaceControl control = _control;
		control.onSelected = (UnityAction)Delegate.Combine(control.onSelected, new UnityAction(OnChangedCharacter));
		_poseHandles = new PoseHandle[handleCount];
		for (int k = 0; k < handleCount; k++)
		{
			_poseHandles[k] = UnityEngine.Object.Instantiate(_poseTarget);
			_poseHandles[k].gameObject.layer = Layers.auxLayer;
			_poseHandles[k].transform.GetChild(0).gameObject.layer = Layers.auxLayer;
			_poseHandles[k].gameObject.SetActive(false);
		}
		searchBar.SetSearchableCollection(_poses, _003C_003Ec._003C_003E9__24_0 ?? (_003C_003Ec._003C_003E9__24_0 = _003C_003Ec._003C_003E9._003CAwake_003Eb__24_0));
		PoseSearchBar poseSearchBar = searchBar;
		poseSearchBar.onSearchCompleted = (UnityAction<ThreadSafeList<Pose>>)Delegate.Combine(poseSearchBar.onSearchCompleted, new UnityAction<ThreadSafeList<Pose>>(OnSearchComplete));
		base.gameObject.SetActive(false);
	}

	private void LoadCustomPoses()
	{
		GeneratePath();
		string[] files = Directory.GetFiles(_folderPath);
		foreach (string filePath in files)
		{
			LoadPose(filePath);
		}
	}

	private void LoadPose(string filePath)
	{
		if (Path.GetExtension(filePath) == ".pose" && File.Exists(filePath))
		{
			CustomPose newPose = JsonUtility.FromJson<CustomPose>(File.ReadAllText(filePath));
			AddCustomPose(newPose);
		}
	}

	private void OnElementClick(int i)
	{
		if (_target == null)
		{
			return;
		}
		if (_activeEntries[i].CustomPoseData == null || !_activeEntries[i].CustomPoseData.IsCustom)
		{
			if ((bool)_activeEntries[i].Image.sprite)
			{
				string text = _activeEntries[i].Image.sprite.name;
				_target.SetPose(text);
				EnablePosingMode(false);
				_currentAnimation = text;
			}
		}
		else if (_activeEntries[i].CustomPoseData != null)
		{
			string animationName = _activeEntries[i].CustomPoseData.animationName;
			_target.SetPose(animationName);
			EnablePosingMode(false);
			_target.LoadPose(_activeEntries[i].CustomPoseData);
		}
	}

	private void OnChangedCharacter()
	{
		DisableHandles();
		UnParentHandles();
		searchBar.ClearSearch();
		UpdateTarget();
		if (base.gameObject.activeSelf)
		{
			if (_target == null)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				LoadPage(base.page);
			}
		}
	}

	private void UnParentHandles()
	{
		if (_poseHandles == null)
		{
			return;
		}
		for (int i = 0; i < handleCount; i++)
		{
			PoseHandle poseHandle = _poseHandles[i];
			if ((bool)poseHandle)
			{
				poseHandle.transform.SetParent(null);
			}
		}
		_customPoseEnabled = false;
	}

	private void PrepareCharacter(IPosable poser)
	{
		if ((bool)poser.Ik)
		{
			float accurateScale = poser.AccurateScale;
			PoseIK poseIK = poser.Ik.poseIK;
			Transform parentBone = poser.transform;
			Transform parentBone2 = _poseHandles[0].transform;
			_poseHandles[0].SetEffector(poser, poseIK.body, parentBone, accurateScale);
			_poseHandles[1].SetEffector(poser, poseIK.leftHand, parentBone2, accurateScale);
			_poseHandles[2].SetEffector(poser, poseIK.rightHand, parentBone2, accurateScale);
			_poseHandles[3].SetEffector(poser, poseIK.leftFoot, parentBone, accurateScale);
			_poseHandles[4].SetEffector(poser, poseIK.rightFoot, parentBone, accurateScale);
			_poseHandles[5].SetEffector(poser, poseIK.leftShoulder, parentBone2, accurateScale);
			_poseHandles[6].SetEffector(poser, poseIK.rightShoulder, parentBone2, accurateScale);
			_poseHandles[7].SetEffector(poser, poseIK.leftThigh, parentBone2, accurateScale);
			_poseHandles[8].SetEffector(poser, poseIK.rightThigh, parentBone2, accurateScale);
			_poseHandles[9].SetBendGoal(poser, poseIK.leftElbow, parentBone2, accurateScale);
			_poseHandles[10].SetBendGoal(poser, poseIK.rightElbow, parentBone2, accurateScale);
			_poseHandles[11].SetBendGoal(poser, poseIK.leftKnee, parentBone, accurateScale);
			_poseHandles[12].SetBendGoal(poser, poseIK.rightKnee, parentBone, accurateScale);
			_poseHandles[13].SetLookAt(poser, poseIK.head, parentBone, accurateScale);
			poser.CreateFingerPosers();
			_customPoseEnabled = true;
		}
	}

	private void UpdateTarget()
	{
		_target = (_control.selectedEntity ? _control.selectedEntity.GetComponent<IPosable>() : null);
	}

	private void OnDisable()
	{
		if ((bool)_control)
		{
			_control.UpdateCollider();
		}
		DisableHandles();
		UnParentHandles();
		_target = null;
	}

	private void OnEnable()
	{
		UpdateTarget();
		LoadPage(base.page);
	}

	private void DisableHandles()
	{
		if ((bool)_placement)
		{
			_placement.updateHandles = true;
		}
		if (_poseHandles == null)
		{
			return;
		}
		for (int i = 0; i < handleCount; i++)
		{
			if ((bool)_poseHandles[i])
			{
				_poseHandles[i].gameObject.SetActive(false);
			}
		}
		if (_target != null)
		{
			_target.ShowFingerPosers(false);
		}
	}

	private void Update()
	{
		RaycastHit hitInfo;
		if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0) && Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, float.PositiveInfinity, Layers.auxMask))
		{
			_placement.Gizmo.SetTarget(hitInfo.collider.transform.parent);
		}
	}

	private void AddCustomPose(CustomPose newPose)
	{
		searchBar.ClearSearch();
		if (_customPoses.ContainsKey(newPose.name))
		{
			Pose item = _customPoses[newPose.name];
			int index = _poses.IndexOf(item);
			_poses[index] = newPose;
			_customPoses[newPose.name] = newPose;
		}
		else
		{
			_customPoses.Add(newPose.name, newPose);
			_poses.Add(newPose);
		}
	}

	protected override void LoadPage(int pageNumber)
	{
		IList<Pose> list = (searchBar.HasSearchResults ? searchBar.SearchResults : _poses);
		int num = pageNumber * _thumbCount;
		for (int i = 0; i < _thumbCount; i++)
		{
			int num2 = num + i;
			if (num2 < list.Count)
			{
				if (!list[num2].IsCustom)
				{
					Pose pose = list[num2];
					_activeEntries[i].Image.sprite = pose.sprite;
					_activeEntries[i].Image.color = Color.white;
					_activeEntries[i].Text.text = pose.name;
					_activeEntries[i].SetPoseData(null);
				}
				else
				{
					CustomPose customPose = (CustomPose)list[num2];
					_activeEntries[i].Image.sprite = customPoseSprite;
					_activeEntries[i].Image.color = Color.white;
					_activeEntries[i].Text.text = customPose.name;
					_activeEntries[i].SetPoseData(customPose);
				}
			}
			else
			{
				_activeEntries[i].Image.sprite = null;
				_activeEntries[i].Image.color = _originalColor;
				_activeEntries[i].Text.text = "";
				_activeEntries[i].SetPoseData(null);
			}
		}
	}

	private void OnSearchComplete(ThreadSafeList<Pose> results)
	{
		if (searchBar.HasSearchQuery)
		{
			base.page = 0;
		}
		else
		{
			LoadPage(base.page);
		}
	}

	protected override int CalculatePageCount()
	{
		return Mathf.CeilToInt((float)((ICollection<Pose>)(searchBar.HasSearchResults ? searchBar.SearchResults : _poses)).Count / (float)_thumbCount);
	}

	private void OnCustomize()
	{
		if (_target != null && _target.IsPosed)
		{
			EnablePosingMode(true);
		}
	}

	private void EnablePosingMode(bool enableIK)
	{
		if (_target == null)
		{
			enableIK = false;
		}
		_placement.updateHandles = !enableIK;
		if (_target == null || _target.Ik == null)
		{
			_placement.updateHandles = true;
			return;
		}
		_target.SetPoseIk(enableIK);
		if (enableIK)
		{
			PrepareCharacter(_target);
		}
		else
		{
			DisableHandles();
			UnParentHandles();
			_target.DestroyFingerPosers();
		}
		Humanoid component = _target.gameObject.GetComponent<Humanoid>();
		if (!component)
		{
			return;
		}
		component.animationManager.UpdateAnimationSpeed();
		if (component.isGiantess)
		{
			Giantess giantess = component as Giantess;
			if ((bool)giantess)
			{
				giantess.gtsMovement.doNotMoveGts = true;
			}
		}
	}

	private void OnSave()
	{
		if (_customPoseEnabled)
		{
			saveDialog.Open(DoSave);
		}
	}

	private void OnDeletePose(PoseCatalogEntry entry)
	{
		_003C_003Ec__DisplayClass44_0 _003C_003Ec__DisplayClass44_ = new _003C_003Ec__DisplayClass44_0();
		_003C_003Ec__DisplayClass44_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass44_.entry = entry;
		confirmationDialog.Open(_003C_003Ec__DisplayClass44_._003COnDeletePose_003Eb__0, null, "Are you sure you wish to delete this pose?");
	}

	private void DeleteCustomPose(string poseName)
	{
		string path = Path.Combine(_folderPath, poseName + ".pose");
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		if (_customPoses.ContainsKey(poseName))
		{
			Pose item = _customPoses[poseName];
			_poses.Remove(item);
			_customPoses.Remove(poseName);
		}
		searchBar.ClearSearch();
	}

	private void DoSave(string poseName)
	{
		if (_target != null)
		{
			CustomPose customPose = _target.Ik.poseIK.SavePose(poseName, _currentAnimation);
			WritePose(customPose);
			AddCustomPose(customPose);
			LoadPage(base.page);
		}
	}

	private void WritePose(CustomPose pose)
	{
		if (_folderPath == null)
		{
			GeneratePath();
		}
		string value = JsonUtility.ToJson(pose);
		StreamWriter streamWriter = new StreamWriter(_folderPath + Path.DirectorySeparatorChar + pose.name + ".pose");
		streamWriter.Write(value);
		streamWriter.Close();
	}

	private void GeneratePath()
	{
		string text = Sbox.StringPreferenceOrArgument(GlobalPreferences.PathData, "-path-data");
		text = Path.Combine(string.IsNullOrEmpty(text) ? Application.persistentDataPath : text, "Poses");
		_folderPath = text;
		if (!Directory.Exists(_folderPath))
		{
			Directory.CreateDirectory(_folderPath);
		}
	}
}
