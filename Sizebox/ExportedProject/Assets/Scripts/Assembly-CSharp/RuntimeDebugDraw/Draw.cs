using System;
using System.Diagnostics;
using RuntimeDebugDraw.Internal;
using UnityEngine;

namespace RuntimeDebugDraw
{
	public static class Draw
	{
		public const int DrawLineLayer = 4;

		public const int DrawTextDefaultSize = 12;

		public static Color DrawDefaultColor = Color.white;

		private static RuntimeDebugDraw.Internal.RuntimeDebugDraw _rtDraw;

		public static Camera GetDebugDrawCamera()
		{
			return Camera.main;
		}

		[Conditional("_DEBUG")]
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterLine(start, end, color, duration, !depthTest);
		}

		[Conditional("_DEBUG")]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterLine(start, start + dir, color, duration, !depthTest);
		}

		[Conditional("_DEBUG")]
		public static void DrawText(Vector3 pos, string text, Color color, int size, float duration, bool popUp)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterDrawText(pos, text, color, size, duration, popUp);
		}

		[Conditional("_DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc, Vector3 offset, Color color, int size)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterAttachText(transform, strFunc, offset, color, size);
		}

		[Conditional("_DEBUG")]
		public static void DrawLine(Vector3 start, Vector3 end)
		{
		}

		[Conditional("_DEBUG")]
		public static void DrawLine(Vector3 start, Vector3 end, Color color)
		{
		}

		[Conditional("_DEBUG")]
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
		{
		}

		[Conditional("_DEBUG")]
		public static void DrawRay(Vector3 start, Vector3 dir)
		{
		}

		[Conditional("_DEBUG")]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color)
		{
		}

		[Conditional("_DEBUG")]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
		{
		}

		[Conditional("_DEBUG")]
		public static void DrawText(Vector3 pos, string text)
		{
		}

		[Conditional("_DEBUG")]
		public static void DrawText(Vector3 pos, string text, Color color)
		{
		}

		[Conditional("_DEBUG")]
		public static void DrawText(Vector3 pos, string text, Color color, int size)
		{
		}

		[Conditional("_DEBUG")]
		public static void DrawText(Vector3 pos, string text, Color color, int size, float duration)
		{
		}

		[Conditional("_DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc)
		{
		}

		[Conditional("_DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc, Vector3 offset)
		{
		}

		[Conditional("_DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc, Vector3 offset, Color color)
		{
		}

		private static void CheckAndBuildHiddenRTDrawObject()
		{
			if (_rtDraw == null)
			{
				GameObject gameObject = new GameObject("________HIDDEN_C4F6A87F298241078E21C0D7C1D87A76_");
				GameObject gameObject2 = new GameObject("________HIDDEN_9D08E9B9785041CD863FF172480C31B2_");
				gameObject2.transform.parent = gameObject.transform;
				_rtDraw = gameObject2.AddComponent<RuntimeDebugDraw.Internal.RuntimeDebugDraw>();
				gameObject.hideFlags = HideFlags.HideInHierarchy;
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}
	}
}
