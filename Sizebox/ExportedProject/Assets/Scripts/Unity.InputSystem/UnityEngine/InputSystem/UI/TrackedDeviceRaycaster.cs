using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.InputSystem.UI
{
	[AddComponentMenu("Event/Tracked Device Raycaster")]
	[RequireComponent(typeof(Canvas))]
	public class TrackedDeviceRaycaster : BaseRaycaster
	{
		private struct RaycastHitData
		{
			[CompilerGenerated]
			private readonly Graphic _003Cgraphic_003Ek__BackingField;

			[CompilerGenerated]
			private readonly Vector3 _003CworldHitPosition_003Ek__BackingField;

			[CompilerGenerated]
			private readonly Vector2 _003CscreenPosition_003Ek__BackingField;

			[CompilerGenerated]
			private readonly float _003Cdistance_003Ek__BackingField;

			public Graphic graphic
			{
				[CompilerGenerated]
				get
				{
					return _003Cgraphic_003Ek__BackingField;
				}
			}

			public Vector3 worldHitPosition
			{
				[CompilerGenerated]
				get
				{
					return _003CworldHitPosition_003Ek__BackingField;
				}
			}

			public Vector2 screenPosition
			{
				[CompilerGenerated]
				get
				{
					return _003CscreenPosition_003Ek__BackingField;
				}
			}

			public float distance
			{
				[CompilerGenerated]
				get
				{
					return _003Cdistance_003Ek__BackingField;
				}
			}

			public RaycastHitData(Graphic graphic, Vector3 worldHitPosition, Vector2 screenPosition, float distance)
			{
				_003Cgraphic_003Ek__BackingField = graphic;
				_003CworldHitPosition_003Ek__BackingField = worldHitPosition;
				_003CscreenPosition_003Ek__BackingField = screenPosition;
				_003Cdistance_003Ek__BackingField = distance;
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Comparison<RaycastHitData> _003C_003E9__25_0;

			internal int _003CSortedRaycastGraphics_003Eb__25_0(RaycastHitData g1, RaycastHitData g2)
			{
				return g2.graphic.depth.CompareTo(g1.graphic.depth);
			}
		}

		[NonSerialized]
		private List<RaycastHitData> m_RaycastResultsCache = new List<RaycastHitData>();

		internal static InlinedArray<TrackedDeviceRaycaster> s_Instances;

		private static readonly List<RaycastHitData> s_SortedGraphics = new List<RaycastHitData>();

		[FormerlySerializedAs("ignoreReversedGraphics")]
		[SerializeField]
		private bool m_IgnoreReversedGraphics;

		[FormerlySerializedAs("checkFor2DOcclusion")]
		[SerializeField]
		private bool m_CheckFor2DOcclusion;

		[FormerlySerializedAs("checkFor3DOcclusion")]
		[SerializeField]
		private bool m_CheckFor3DOcclusion;

		[Tooltip("Maximum distance (in 3D world space) that rays are traced to find a hit.")]
		[SerializeField]
		private float m_MaxDistance = 1000f;

		[SerializeField]
		private LayerMask m_BlockingMask;

		[NonSerialized]
		private Canvas m_Canvas;

		public override Camera eventCamera
		{
			get
			{
				Canvas canvas = this.canvas;
				if (!(canvas != null))
				{
					return null;
				}
				return canvas.worldCamera;
			}
		}

		public LayerMask blockingMask
		{
			get
			{
				return m_BlockingMask;
			}
			set
			{
				m_BlockingMask = value;
			}
		}

		public bool checkFor3DOcclusion
		{
			get
			{
				return m_CheckFor3DOcclusion;
			}
			set
			{
				m_CheckFor3DOcclusion = value;
			}
		}

		public bool checkFor2DOcclusion
		{
			get
			{
				return m_CheckFor2DOcclusion;
			}
			set
			{
				m_CheckFor2DOcclusion = value;
			}
		}

		public bool ignoreReversedGraphics
		{
			get
			{
				return m_IgnoreReversedGraphics;
			}
			set
			{
				m_IgnoreReversedGraphics = value;
			}
		}

		public float maxDistance
		{
			get
			{
				return m_MaxDistance;
			}
			set
			{
				m_MaxDistance = value;
			}
		}

		private Canvas canvas
		{
			get
			{
				if (m_Canvas != null)
				{
					return m_Canvas;
				}
				m_Canvas = GetComponent<Canvas>();
				return m_Canvas;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			s_Instances.AppendWithCapacity(this);
		}

		protected override void OnDisable()
		{
			int num = s_Instances.IndexOfReference(this);
			if (num != -1)
			{
				s_Instances.RemoveAtByMovingTailWithCapacity(num);
			}
			base.OnDisable();
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			ExtendedPointerEventData extendedPointerEventData;
			if ((extendedPointerEventData = eventData as ExtendedPointerEventData) != null && extendedPointerEventData.pointerType == UIPointerType.Tracked)
			{
				PerformRaycast(extendedPointerEventData, resultAppendList);
			}
		}

		internal void PerformRaycast(ExtendedPointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (canvas == null || eventCamera == null)
			{
				return;
			}
			Ray ray = new Ray(eventData.trackedDevicePosition, eventData.trackedDeviceOrientation * Vector3.forward);
			float num = m_MaxDistance;
			if (m_CheckFor3DOcclusion)
			{
				RaycastHit[] array = Physics.RaycastAll(ray, num, m_BlockingMask);
				if (array.Length != 0 && array[0].distance < num)
				{
					num = array[0].distance;
				}
			}
			if (m_CheckFor2DOcclusion)
			{
				float num2 = num;
				RaycastHit2D[] rayIntersectionAll = Physics2D.GetRayIntersectionAll(ray, num2, m_BlockingMask);
				if (rayIntersectionAll.Length != 0 && rayIntersectionAll[0].fraction * num2 < num)
				{
					num = rayIntersectionAll[0].fraction * num2;
				}
			}
			m_RaycastResultsCache.Clear();
			SortedRaycastGraphics(canvas, ray, m_RaycastResultsCache);
			for (int i = 0; i < m_RaycastResultsCache.Count; i++)
			{
				bool flag = true;
				RaycastHitData raycastHitData = m_RaycastResultsCache[i];
				GameObject gameObject = raycastHitData.graphic.gameObject;
				if (m_IgnoreReversedGraphics)
				{
					Vector3 direction = ray.direction;
					Vector3 rhs = gameObject.transform.rotation * Vector3.forward;
					flag = Vector3.Dot(direction, rhs) > 0f;
				}
				if (flag & (raycastHitData.distance < num))
				{
					RaycastResult raycastResult = default(RaycastResult);
					raycastResult.gameObject = gameObject;
					raycastResult.module = this;
					raycastResult.distance = raycastHitData.distance;
					raycastResult.index = resultAppendList.Count;
					raycastResult.depth = raycastHitData.graphic.depth;
					raycastResult.worldPosition = raycastHitData.worldHitPosition;
					raycastResult.screenPosition = raycastHitData.screenPosition;
					RaycastResult item = raycastResult;
					resultAppendList.Add(item);
				}
			}
		}

		private void SortedRaycastGraphics(Canvas canvas, Ray ray, List<RaycastHitData> results)
		{
			IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
			s_SortedGraphics.Clear();
			for (int i = 0; i < graphicsForCanvas.Count; i++)
			{
				Graphic graphic = graphicsForCanvas[i];
				Vector3 worldPosition;
				float distance;
				if (graphic.depth != -1 && RayIntersectsRectTransform(graphic.rectTransform, ray, out worldPosition, out distance))
				{
					Vector2 vector = eventCamera.WorldToScreenPoint(worldPosition);
					if (graphic.Raycast(vector, eventCamera))
					{
						s_SortedGraphics.Add(new RaycastHitData(graphic, worldPosition, vector, distance));
					}
				}
			}
			s_SortedGraphics.Sort(_003C_003Ec._003C_003E9__25_0 ?? (_003C_003Ec._003C_003E9__25_0 = _003C_003Ec._003C_003E9._003CSortedRaycastGraphics_003Eb__25_0));
			results.AddRange(s_SortedGraphics);
		}

		private static bool RayIntersectsRectTransform(RectTransform transform, Ray ray, out Vector3 worldPosition, out float distance)
		{
			Vector3[] array = new Vector3[4];
			transform.GetWorldCorners(array);
			float enter;
			if (new Plane(array[0], array[1], array[2]).Raycast(ray, out enter))
			{
				Vector3 point = ray.GetPoint(enter);
				Vector3 rhs = array[3] - array[0];
				Vector3 rhs2 = array[1] - array[0];
				float num = Vector3.Dot(point - array[0], rhs);
				if (Vector3.Dot(point - array[0], rhs2) >= 0f && num >= 0f)
				{
					Vector3 rhs3 = array[1] - array[2];
					Vector3 rhs4 = array[3] - array[2];
					float num2 = Vector3.Dot(point - array[2], rhs3);
					float num3 = Vector3.Dot(point - array[2], rhs4);
					if (num2 >= 0f && num3 >= 0f)
					{
						worldPosition = point;
						distance = enter;
						return true;
					}
				}
			}
			worldPosition = Vector3.zero;
			distance = 0f;
			return false;
		}
	}
}
