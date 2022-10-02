using UnityEngine;
using UnityEngine.EventSystems;

public class HandleControl : MonoBehaviour
{
	private enum State
	{
		NotDrag = 0,
		Drag = 1
	}

	public EntityBase smartObject;

	private State state;

	private Camera mainCamera;

	private Collider xAxis;

	private Collider yAxis;

	private Collider zAxis;

	private Collider planeX;

	private Collider planeY;

	private Collider planeZ;

	private Vector3 axis;

	private Vector3 initialPoint;

	private Vector3 initialTransform;

	private void Start()
	{
		state = State.NotDrag;
		mainCamera = Camera.main;
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		xAxis = componentsInChildren[0];
		yAxis = componentsInChildren[1];
		zAxis = componentsInChildren[2];
		planeX = componentsInChildren[3];
		planeY = componentsInChildren[4];
		planeZ = componentsInChildren[5];
		DisablePlanes();
	}

	private void DisablePlanes()
	{
		planeX.enabled = false;
		planeY.enabled = false;
		planeZ.enabled = false;
	}

	private void Update()
	{
		if (!smartObject)
		{
			return;
		}
		State state = this.state;
		Vector3 mousePosition = Input.mousePosition;
		RaycastHit hitInfo;
		switch (this.state)
		{
		case State.NotDrag:
			base.transform.position = smartObject.transform.position;
			base.transform.localScale = Vector3.one * smartObject.Height;
			if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0) && Physics.Raycast(mainCamera.ScreenPointToRay(mousePosition), out hitInfo, float.PositiveInfinity, Layers.actionSelectionMask))
			{
				if (hitInfo.collider.gameObject.layer != Layers.uiLayer)
				{
					return;
				}
				if (hitInfo.collider == xAxis)
				{
					axis = Vector3.right;
				}
				else if (hitInfo.collider == yAxis)
				{
					axis = Vector3.up;
				}
				else if (hitInfo.collider == zAxis)
				{
					axis = Vector3.forward;
				}
				planeX.enabled = axis.x == 0f;
				planeY.enabled = axis.y == 0f;
				planeZ.enabled = axis.z == 0f;
				initialTransform = base.transform.position;
				initialPoint = base.transform.position + Vector3.Scale(hitInfo.point - initialTransform, axis);
				state = State.Drag;
			}
			break;
		case State.Drag:
			if (Physics.Raycast(mainCamera.ScreenPointToRay(mousePosition), out hitInfo, float.PositiveInfinity, Layers.auxMask))
			{
				Vector3 vector = Vector3.Scale(hitInfo.point - initialPoint, axis);
				base.transform.position = initialTransform + vector;
				smartObject.Move(base.transform.position);
			}
			if (Input.GetMouseButtonUp(0))
			{
				DisablePlanes();
				state = State.NotDrag;
			}
			break;
		}
		this.state = state;
	}
}
