using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResizeCharacter : MonoBehaviour
{
	private EntityBase _entity;

	private InputAction _changeSize;

	private IEnumerator _sizeChangedRoutine;

	private static Toast _playerSizeToast;

	public float scaleModifier = 1f;

	public float sizeChangeRate = 0.7f;

	public void SetEntity(EntityBase entity)
	{
		_entity = entity;
		if ((bool)entity)
		{
			scaleModifier = 1f;
			if (entity.isGiantess)
			{
				scaleModifier = 1000f;
			}
		}
	}

	private void Awake()
	{
		_sizeChangedRoutine = ChangeSizeRoutine();
		_changeSize = InputManager.inputs.Player.ChangeSize;
	}

	private void Start()
	{
		_playerSizeToast = new Toast("_PlayerSize");
	}

	private void OnEnable()
	{
		_changeSize.started += ChangeSizeOnPerformed;
		_changeSize.canceled += ChangeSizeOnCanceled;
	}

	private void OnDisable()
	{
		_changeSize.started -= ChangeSizeOnPerformed;
		_changeSize.canceled -= ChangeSizeOnCanceled;
	}

	private void ChangeSizeOnCanceled(InputAction.CallbackContext obj)
	{
		StopCoroutine(_sizeChangedRoutine);
	}

	private void ChangeSizeOnPerformed(InputAction.CallbackContext obj)
	{
		if ((bool)_entity)
		{
			StartCoroutine(_sizeChangedRoutine);
		}
	}

	private IEnumerator ChangeSizeRoutine()
	{
		do
		{
			float num = _changeSize.ReadValue<float>();
			ChangeScale(_entity.Scale * (1f + sizeChangeRate * num * Time.deltaTime));
			_playerSizeToast.Print(GameController.ConvertScaleToHumanReadable(_entity.MeshHeight));
			yield return null;
		}
		while ((bool)_entity);
	}

	public void ChangeScale(float scale)
	{
		if ((bool)_entity)
		{
			bool isGiantess = _entity.isGiantess;
			float num = (isGiantess ? (MapSettingInternal.minGtsSize * 1000f) : MapSettingInternal.minPlayerSize);
			float num2 = (isGiantess ? (MapSettingInternal.maxGtsSize * 1000f) : MapSettingInternal.maxPlayerSize);
			if (scale < num / scaleModifier)
			{
				scale = num / scaleModifier;
			}
			else if (scale > num2 / scaleModifier)
			{
				scale = num2 / scaleModifier;
			}
			_entity.ChangeScale(scale);
		}
	}
}
