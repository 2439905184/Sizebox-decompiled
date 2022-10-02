using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeGizmos
{
	[RequireComponent(typeof(Camera))]
	public class TransformGizmo : MonoBehaviour
	{
		private bool getTarget;

		public TransformSpace space;

		public TransformType type;

		public KeyCode SetMoveType = KeyCode.T;

		public KeyCode SetRotateType = KeyCode.R;

		public KeyCode SetScaleType = KeyCode.Y;

		public bool overrideSpace;

		public bool allowScaling = true;

		private Color xColor = new Color(1f, 0f, 0f, 0.8f);

		private Color yColor = new Color(0f, 1f, 0f, 0.8f);

		private Color zColor = new Color(0f, 0f, 1f, 0.8f);

		private Color allColor = new Color(0.7f, 0.7f, 0.7f, 0.8f);

		private Color selectedColor = new Color(1f, 1f, 0f, 0.8f);

		private float handleLength = 0.25f;

		private float triangleSize = 0.03f;

		private float boxSize = 0.01f;

		private int circleDetail = 40;

		private float minSelectedDistanceCheck = 0.04f;

		private float moveSpeedMultiplier = 1f;

		private float scaleSpeedMultiplier = 1f;

		private float rotateSpeedMultiplier = 200f;

		private float allRotateSpeedMultiplier = 20f;

		private AxisVectors handleLines = new AxisVectors();

		private AxisVectors handleTriangles = new AxisVectors();

		private AxisVectors handleSquares = new AxisVectors();

		private AxisVectors circlesLines = new AxisVectors();

		private AxisVectors drawCurrentCirclesLines = new AxisVectors();

		private bool isTransforming;

		private float totalScaleAmount;

		private Quaternion totalRotationAmount;

		private Axis selectedAxis;

		private AxisInfo axisInfo;

		private Transform target;

		private EntityBase entity;

		private Camera myCamera;

		private float startClick = -1f;

		private float minClickTime = 0.1f;

		private static Material lineMaterial;

		private AxisVectors selectedLinesBuffer = new AxisVectors();

		private void Awake()
		{
			myCamera = GetComponent<Camera>();
			SetMaterial();
		}

		private void Update()
		{
			SetSpaceAndType();
			SelectAxis();
			GetTarget();
			if (target != null)
			{
				TransformSelected();
				if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift) && target.GetComponent<FingerPoserHandle>() != null)
				{
					target.GetComponent<FingerPoserHandle>().Reset();
				}
			}
		}

		private void LateUpdate()
		{
			if (!(target == null))
			{
				SetAxisInfo();
				SetLines();
			}
		}

		private bool MouseOverUI()
		{
			return EventSystem.current.IsPointerOverGameObject();
		}

		private void OnPostRender()
		{
			if (!(target == null))
			{
				lineMaterial.SetPass(0);
				Color color = ((selectedAxis == Axis.X) ? selectedColor : xColor);
				Color color2 = ((selectedAxis == Axis.Y) ? selectedColor : yColor);
				Color color3 = ((selectedAxis == Axis.Z) ? selectedColor : zColor);
				Color color4 = ((selectedAxis == Axis.Any) ? selectedColor : allColor);
				DrawLines(handleLines.x, color);
				DrawLines(handleLines.y, color2);
				DrawLines(handleLines.z, color3);
				DrawTriangles(handleTriangles.x, color);
				DrawTriangles(handleTriangles.y, color2);
				DrawTriangles(handleTriangles.z, color3);
				DrawSquares(handleSquares.x, color);
				DrawSquares(handleSquares.y, color2);
				DrawSquares(handleSquares.z, color3);
				DrawSquares(handleSquares.all, color4);
				AxisVectors axisVectors = circlesLines;
				if (space == TransformSpace.Global && type == TransformType.Rotate)
				{
					axisVectors = drawCurrentCirclesLines;
					AxisInfo axisInfo = default(AxisInfo);
					axisInfo.xDirection = totalRotationAmount * Vector3.right;
					axisInfo.yDirection = totalRotationAmount * Vector3.up;
					axisInfo.zDirection = totalRotationAmount * Vector3.forward;
					SetCircles(axisInfo, drawCurrentCirclesLines);
				}
				DrawCircles(axisVectors.x, color);
				DrawCircles(axisVectors.y, color2);
				DrawCircles(axisVectors.z, color3);
				DrawCircles(axisVectors.all, color4);
			}
		}

		private void SetSpaceAndType()
		{
			if (Input.GetKeyDown(SetMoveType))
			{
				SetToolMode(TransformType.Move);
			}
			else if (Input.GetKeyDown(SetRotateType))
			{
				SetToolMode(TransformType.Rotate);
			}
			else if (Input.GetKeyDown(SetScaleType))
			{
				SetToolMode(TransformType.Scale);
			}
			if (Input.GetButtonDown(ButtonInput.ToggleTransformSpace))
			{
				ToggleSpaceMode();
			}
			if (type == TransformType.Scale && !overrideSpace)
			{
				space = TransformSpace.Local;
			}
		}

		public void SetToolMode(TransformType mode)
		{
			if (allowScaling || mode != TransformType.Scale)
			{
				type = mode;
			}
		}

		public void ToggleSpaceMode()
		{
			if (!overrideSpace)
			{
				if (space == TransformSpace.Global)
				{
					space = TransformSpace.Local;
				}
				else if (space == TransformSpace.Local)
				{
					space = TransformSpace.Global;
				}
			}
		}

		private void TransformSelected()
		{
			float time = Time.time;
			if (Input.GetMouseButtonDown(0))
			{
				startClick = time;
			}
			if (startClick > 0f && time > startClick + minClickTime)
			{
				startClick = -1f;
				if (selectedAxis != 0 && Input.GetMouseButton(0) && !MouseOverUI())
				{
					StartCoroutine(TransformSelected(type));
				}
			}
		}

		private IEnumerator TransformSelected(TransformType type)
		{
			isTransforming = true;
			totalScaleAmount = 0f;
			totalRotationAmount = Quaternion.identity;
			Vector3 originalTargetPosition = GetTargetPosition();
			Vector3 planeNormal = (base.transform.position - GetTargetPosition()).normalized;
			Vector3 axis = GetSelectedAxisDirection();
			Vector3 projectedAxis = Vector3.ProjectOnPlane(axis, planeNormal).normalized;
			Vector3 previousMousePosition = Vector3.zero;
			while (!Input.GetMouseButtonUp(0))
			{
				Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
				Vector3 vector = Geometry.LinePlaneIntersect(ray.origin, ray.direction, originalTargetPosition, planeNormal);
				if (previousMousePosition != Vector3.zero && vector != Vector3.zero)
				{
					if (type == TransformType.Move)
					{
						float num = ExtVector3.MagnitudeInDirection(vector - previousMousePosition, projectedAxis) * moveSpeedMultiplier;
						if (entity != null)
						{
							entity.Move(entity.transform.position + axis * num);
						}
						else
						{
							target.Translate(axis * num, Space.World);
						}
					}
					if (type == TransformType.Scale)
					{
						Vector3 direction = ((selectedAxis == Axis.Any) ? base.transform.right : projectedAxis);
						float num2 = ExtVector3.MagnitudeInDirection(vector - previousMousePosition, direction) * scaleSpeedMultiplier;
						Vector3 vector2 = ((space == TransformSpace.Local && selectedAxis != Axis.Any) ? target.InverseTransformDirection(axis) : axis);
						float num3 = 1f;
						if (entity is Giantess)
						{
							num3 = 0.001f;
						}
						if (selectedAxis == Axis.Any)
						{
							target.localScale += target.localScale.normalized.Abs() * num2 * num3;
						}
						else
						{
							target.localScale += vector2 * num2 * num3;
						}
						totalScaleAmount += num2;
					}
					if (type == TransformType.Rotate)
					{
						Transform transform = target;
						if ((bool)entity && (bool)entity.Rigidbody)
						{
							transform = entity.Rigidbody.transform;
						}
						if (selectedAxis == Axis.Any)
						{
							Vector3 vector3 = base.transform.TransformDirection(new Vector3(Input.GetAxis("Mouse Y"), 0f - Input.GetAxis("Mouse X"), 0f));
							transform.Rotate(vector3 * allRotateSpeedMultiplier, Space.World);
							totalRotationAmount *= Quaternion.Euler(vector3 * allRotateSpeedMultiplier);
						}
						else
						{
							Vector3 direction2 = ((selectedAxis == Axis.Any || ExtVector3.IsParallel(axis, planeNormal)) ? planeNormal : Vector3.Cross(axis, planeNormal));
							float num4 = ExtVector3.MagnitudeInDirection(vector - previousMousePosition, direction2) * rotateSpeedMultiplier / GetDistanceMultiplier();
							transform.Rotate(axis, num4, Space.World);
							totalRotationAmount *= Quaternion.Euler(axis * num4);
						}
					}
				}
				previousMousePosition = vector;
				yield return null;
			}
			totalRotationAmount = Quaternion.identity;
			totalScaleAmount = 0f;
			isTransforming = false;
		}

		private Vector3 GetSelectedAxisDirection()
		{
			if (selectedAxis != 0)
			{
				if (selectedAxis == Axis.X)
				{
					return axisInfo.xDirection;
				}
				if (selectedAxis == Axis.Y)
				{
					return axisInfo.yDirection;
				}
				if (selectedAxis == Axis.Z)
				{
					return axisInfo.zDirection;
				}
				if (selectedAxis == Axis.Any)
				{
					return Vector3.one;
				}
			}
			return Vector3.zero;
		}

		private void GetTarget()
		{
			if (getTarget && !MouseOverUI() && selectedAxis == Axis.None && Input.GetMouseButtonDown(0))
			{
				RaycastHit hitInfo;
				if (Physics.Raycast(myCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
				{
					SetTarget(hitInfo.transform);
				}
				else
				{
					target = null;
				}
			}
		}

		public void SetTarget(Transform newTarget)
		{
			target = newTarget;
			if ((bool)target)
			{
				entity = target.GetComponent<EntityBase>();
			}
			else
			{
				entity = null;
			}
		}

		private Vector3 GetTargetPosition()
		{
			if (target.GetComponent<FingerPoserHandle>() != null)
			{
				return target.GetComponent<FingerPoserHandle>().realPosition;
			}
			return target.position;
		}

		private void SelectAxis()
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}
			selectedAxis = Axis.None;
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MaxValue;
			float num4 = float.MaxValue;
			float num5 = minSelectedDistanceCheck * GetDistanceMultiplier();
			if (type == TransformType.Move || type == TransformType.Scale)
			{
				selectedLinesBuffer.Clear();
				selectedLinesBuffer.Add(handleLines);
				if (type == TransformType.Move)
				{
					selectedLinesBuffer.Add(handleTriangles);
				}
				else if (type == TransformType.Scale)
				{
					selectedLinesBuffer.Add(handleSquares);
				}
				num = ClosestDistanceFromMouseToLines(selectedLinesBuffer.x);
				num2 = ClosestDistanceFromMouseToLines(selectedLinesBuffer.y);
				num3 = ClosestDistanceFromMouseToLines(selectedLinesBuffer.z);
				num4 = ClosestDistanceFromMouseToLines(selectedLinesBuffer.all);
			}
			else if (type == TransformType.Rotate)
			{
				num = ClosestDistanceFromMouseToLines(circlesLines.x);
				num2 = ClosestDistanceFromMouseToLines(circlesLines.y);
				num3 = ClosestDistanceFromMouseToLines(circlesLines.z);
				num4 = ClosestDistanceFromMouseToLines(circlesLines.all);
			}
			if (type == TransformType.Scale && num4 <= num5)
			{
				selectedAxis = Axis.Any;
			}
			else if (num <= num5 && num <= num2 && num <= num3)
			{
				selectedAxis = Axis.X;
			}
			else if (num2 <= num5 && num2 <= num && num2 <= num3)
			{
				selectedAxis = Axis.Y;
			}
			else if (num3 <= num5 && num3 <= num && num3 <= num2)
			{
				selectedAxis = Axis.Z;
			}
			else if (type == TransformType.Rotate && target != null)
			{
				Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
				Vector3 vector = Geometry.LinePlaneIntersect(ray.origin, ray.direction, GetTargetPosition(), (base.transform.position - GetTargetPosition()).normalized);
				if ((GetTargetPosition() - vector).sqrMagnitude <= (handleLength * GetDistanceMultiplier()).Squared())
				{
					selectedAxis = Axis.Any;
				}
			}
		}

		private float ClosestDistanceFromMouseToLines(List<Vector3> lines)
		{
			Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
			float num = float.MaxValue;
			for (int i = 0; i < lines.Count; i += 2)
			{
				IntersectPoints intersectPoints = Geometry.ClosestPointsOnSegmentToLine(lines[i], lines[i + 1], ray.origin, ray.direction);
				float num2 = Vector3.Distance(intersectPoints.first, intersectPoints.second);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		private void SetAxisInfo()
		{
			float num = handleLength * GetDistanceMultiplier();
			axisInfo.Set(target, num, space);
			if (isTransforming && type == TransformType.Scale)
			{
				if (selectedAxis == Axis.Any)
				{
					axisInfo.Set(target, num + totalScaleAmount, space);
				}
				if (selectedAxis == Axis.X)
				{
					axisInfo.xAxisEnd += axisInfo.xDirection * totalScaleAmount;
				}
				if (selectedAxis == Axis.Y)
				{
					axisInfo.yAxisEnd += axisInfo.yDirection * totalScaleAmount;
				}
				if (selectedAxis == Axis.Z)
				{
					axisInfo.zAxisEnd += axisInfo.zDirection * totalScaleAmount;
				}
			}
		}

		private float GetDistanceMultiplier()
		{
			if (target == null)
			{
				return 0f;
			}
			return Mathf.Max(0.01f, Mathf.Abs(ExtVector3.MagnitudeInDirection(GetTargetPosition() - base.transform.position, myCamera.transform.forward)));
		}

		private void SetLines()
		{
			SetHandleLines();
			SetHandleTriangles();
			SetHandleSquares();
			SetCircles(axisInfo, circlesLines);
		}

		private void SetHandleLines()
		{
			handleLines.Clear();
			if (type == TransformType.Move || type == TransformType.Scale)
			{
				handleLines.x.Add(GetTargetPosition());
				handleLines.x.Add(axisInfo.xAxisEnd);
				handleLines.y.Add(GetTargetPosition());
				handleLines.y.Add(axisInfo.yAxisEnd);
				handleLines.z.Add(GetTargetPosition());
				handleLines.z.Add(axisInfo.zAxisEnd);
			}
		}

		private void SetHandleTriangles()
		{
			handleTriangles.Clear();
			if (type == TransformType.Move)
			{
				float size = triangleSize * GetDistanceMultiplier();
				AddTriangles(axisInfo.xAxisEnd, axisInfo.xDirection, axisInfo.yDirection, axisInfo.zDirection, size, handleTriangles.x);
				AddTriangles(axisInfo.yAxisEnd, axisInfo.yDirection, axisInfo.xDirection, axisInfo.zDirection, size, handleTriangles.y);
				AddTriangles(axisInfo.zAxisEnd, axisInfo.zDirection, axisInfo.yDirection, axisInfo.xDirection, size, handleTriangles.z);
			}
		}

		private void AddTriangles(Vector3 axisEnd, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size, List<Vector3> resultsBuffer)
		{
			Vector3 item = axisEnd + axisDirection * (size * 2f);
			Square baseSquare = GetBaseSquare(axisEnd, axisOtherDirection1, axisOtherDirection2, size / 2f);
			resultsBuffer.Add(baseSquare.bottomLeft);
			resultsBuffer.Add(baseSquare.topLeft);
			resultsBuffer.Add(baseSquare.topRight);
			resultsBuffer.Add(baseSquare.topLeft);
			resultsBuffer.Add(baseSquare.bottomRight);
			resultsBuffer.Add(baseSquare.topRight);
			for (int i = 0; i < 4; i++)
			{
				resultsBuffer.Add(baseSquare[i]);
				resultsBuffer.Add(baseSquare[i + 1]);
				resultsBuffer.Add(item);
			}
		}

		private void SetHandleSquares()
		{
			handleSquares.Clear();
			if (type == TransformType.Scale)
			{
				float num = boxSize * GetDistanceMultiplier();
				AddSquares(axisInfo.xAxisEnd, axisInfo.xDirection, axisInfo.yDirection, axisInfo.zDirection, num, handleSquares.x);
				AddSquares(axisInfo.yAxisEnd, axisInfo.yDirection, axisInfo.xDirection, axisInfo.zDirection, num, handleSquares.y);
				AddSquares(axisInfo.zAxisEnd, axisInfo.zDirection, axisInfo.xDirection, axisInfo.yDirection, num, handleSquares.z);
				AddSquares(GetTargetPosition() - axisInfo.xDirection * num, axisInfo.xDirection, axisInfo.yDirection, axisInfo.zDirection, num, handleSquares.all);
			}
		}

		private void AddSquares(Vector3 axisEnd, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size, List<Vector3> resultsBuffer)
		{
			Square baseSquare = GetBaseSquare(axisEnd, axisOtherDirection1, axisOtherDirection2, size);
			Square baseSquare2 = GetBaseSquare(axisEnd + axisDirection * (size * 2f), axisOtherDirection1, axisOtherDirection2, size);
			resultsBuffer.Add(baseSquare.bottomLeft);
			resultsBuffer.Add(baseSquare.topLeft);
			resultsBuffer.Add(baseSquare.bottomRight);
			resultsBuffer.Add(baseSquare.topRight);
			resultsBuffer.Add(baseSquare2.bottomLeft);
			resultsBuffer.Add(baseSquare2.topLeft);
			resultsBuffer.Add(baseSquare2.bottomRight);
			resultsBuffer.Add(baseSquare2.topRight);
			for (int i = 0; i < 4; i++)
			{
				resultsBuffer.Add(baseSquare[i]);
				resultsBuffer.Add(baseSquare[i + 1]);
				resultsBuffer.Add(baseSquare2[i + 1]);
				resultsBuffer.Add(baseSquare2[i]);
			}
		}

		private Square GetBaseSquare(Vector3 axisEnd, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size)
		{
			Vector3 vector = axisOtherDirection1 * size + axisOtherDirection2 * size;
			Vector3 vector2 = axisOtherDirection1 * size - axisOtherDirection2 * size;
			Square result = default(Square);
			result.bottomLeft = axisEnd + vector2;
			result.topLeft = axisEnd + vector;
			result.bottomRight = axisEnd - vector2;
			result.topRight = axisEnd - vector;
			return result;
		}

		private void SetCircles(AxisInfo axisInfo, AxisVectors axisVectors)
		{
			axisVectors.Clear();
			if (type == TransformType.Rotate)
			{
				float size = handleLength * GetDistanceMultiplier();
				AddCircle(GetTargetPosition(), axisInfo.xDirection, size, axisVectors.x);
				AddCircle(GetTargetPosition(), axisInfo.yDirection, size, axisVectors.y);
				AddCircle(GetTargetPosition(), axisInfo.zDirection, size, axisVectors.z);
				AddCircle(GetTargetPosition(), (GetTargetPosition() - base.transform.position).normalized, size, axisVectors.all, false);
			}
		}

		private void AddCircle(Vector3 origin, Vector3 axisDirection, float size, List<Vector3> resultsBuffer, bool depthTest = true)
		{
			Vector3 vector = axisDirection.normalized * size;
			Vector3 rhs = Vector3.Slerp(vector, -vector, 0.5f);
			Vector3 vector2 = Vector3.Cross(vector, rhs).normalized * size;
			Matrix4x4 matrix4x = default(Matrix4x4);
			matrix4x[0] = vector2.x;
			matrix4x[1] = vector2.y;
			matrix4x[2] = vector2.z;
			matrix4x[4] = vector.x;
			matrix4x[5] = vector.y;
			matrix4x[6] = vector.z;
			matrix4x[8] = rhs.x;
			matrix4x[9] = rhs.y;
			matrix4x[10] = rhs.z;
			Vector3 vector3 = origin + matrix4x.MultiplyPoint3x4(new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)));
			Vector3 vector4 = Vector3.zero;
			float num = 360f / (float)circleDetail;
			Plane plane = new Plane((base.transform.position - GetTargetPosition()).normalized, GetTargetPosition());
			for (int i = 0; i < circleDetail + 1; i++)
			{
				vector4.x = Mathf.Cos((float)i * num * ((float)Math.PI / 180f));
				vector4.z = Mathf.Sin((float)i * num * ((float)Math.PI / 180f));
				vector4.y = 0f;
				vector4 = origin + matrix4x.MultiplyPoint3x4(vector4);
				if (!depthTest || plane.GetSide(vector3))
				{
					resultsBuffer.Add(vector3);
					resultsBuffer.Add(vector4);
				}
				vector3 = vector4;
			}
		}

		private void DrawLines(List<Vector3> lines, Color color)
		{
			GL.Begin(1);
			GL.Color(color);
			for (int i = 0; i < lines.Count; i += 2)
			{
				GL.Vertex(lines[i]);
				GL.Vertex(lines[i + 1]);
			}
			GL.End();
		}

		private void DrawTriangles(List<Vector3> lines, Color color)
		{
			GL.Begin(4);
			GL.Color(color);
			for (int i = 0; i < lines.Count; i += 3)
			{
				GL.Vertex(lines[i]);
				GL.Vertex(lines[i + 1]);
				GL.Vertex(lines[i + 2]);
			}
			GL.End();
		}

		private void DrawSquares(List<Vector3> lines, Color color)
		{
			GL.Begin(7);
			GL.Color(color);
			for (int i = 0; i < lines.Count; i += 4)
			{
				GL.Vertex(lines[i]);
				GL.Vertex(lines[i + 1]);
				GL.Vertex(lines[i + 2]);
				GL.Vertex(lines[i + 3]);
			}
			GL.End();
		}

		private void DrawCircles(List<Vector3> lines, Color color)
		{
			GL.Begin(1);
			GL.Color(color);
			for (int i = 0; i < lines.Count; i += 2)
			{
				GL.Vertex(lines[i]);
				GL.Vertex(lines[i + 1]);
			}
			GL.End();
		}

		private void SetMaterial()
		{
			if (lineMaterial == null)
			{
				lineMaterial = new Material(Shader.Find("Custom/Lines"));
			}
		}
	}
}
