using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public static class Mathf
	{
		public static float Deg2Rad
		{
			get
			{
				return (float)Math.PI / 180f;
			}
		}

		public static float Epsilon
		{
			get
			{
				return UnityEngine.Mathf.Epsilon;
			}
		}

		public static float Infinity
		{
			get
			{
				return float.PositiveInfinity;
			}
		}

		public static float NegativeInfinity
		{
			get
			{
				return float.NegativeInfinity;
			}
		}

		public static float PI
		{
			get
			{
				return (float)Math.PI;
			}
		}

		public static float Rad2Deg
		{
			get
			{
				return 57.29578f;
			}
		}

		public static float Abs(float f)
		{
			return UnityEngine.Mathf.Abs(f);
		}

		public static float Acos(float f)
		{
			return UnityEngine.Mathf.Acos(f);
		}

		public static bool Approximately(float a, float b)
		{
			return UnityEngine.Mathf.Approximately(a, b);
		}

		public static float Asin(float f)
		{
			return UnityEngine.Mathf.Asin(f);
		}

		public static float Atan(float f)
		{
			return UnityEngine.Mathf.Atan(f);
		}

		public static float Atan2(float y, float x)
		{
			return UnityEngine.Mathf.Atan2(y, x);
		}

		public static float Ceil(float f)
		{
			return UnityEngine.Mathf.Ceil(f);
		}

		public static float Clamp(float value, float min, float max)
		{
			return UnityEngine.Mathf.Clamp(value, min, max);
		}

		public static float Clamp01(float value)
		{
			return UnityEngine.Mathf.Clamp01(value);
		}

		public static int ClosestPowerOfTwo(int value)
		{
			return UnityEngine.Mathf.ClosestPowerOfTwo(value);
		}

		public static float Cos(float f)
		{
			return UnityEngine.Mathf.Cos(f);
		}

		public static float DeltaAngle(float current, float target)
		{
			return UnityEngine.Mathf.DeltaAngle(current, target);
		}

		public static float Exp(float power)
		{
			return UnityEngine.Mathf.Exp(power);
		}

		public static float Floor(float f)
		{
			return UnityEngine.Mathf.Floor(f);
		}

		public static float InverseLerp(float a, float b, float value)
		{
			return UnityEngine.Mathf.InverseLerp(a, b, value);
		}

		public static bool IsPowerOfTwo(int value)
		{
			return UnityEngine.Mathf.IsPowerOfTwo(value);
		}

		public static float Lerp(float a, float b, float t)
		{
			return UnityEngine.Mathf.Lerp(a, b, t);
		}

		public static float LerpAngle(float a, float b, float t)
		{
			return UnityEngine.Mathf.LerpAngle(a, b, t);
		}

		public static float LerpUnclamped(float a, float b, float t)
		{
			return UnityEngine.Mathf.LerpUnclamped(a, b, t);
		}

		public static float Log(float f, float p)
		{
			return UnityEngine.Mathf.Log(f, p);
		}

		public static float Log10(float f)
		{
			return UnityEngine.Mathf.Log10(f);
		}

		public static float Max(float a, float b)
		{
			return UnityEngine.Mathf.Max(a, b);
		}

		public static float Max(params float[] values)
		{
			return UnityEngine.Mathf.Max(values);
		}

		public static float Min(float a, float b)
		{
			return UnityEngine.Mathf.Min(a, b);
		}

		public static float Min(params float[] values)
		{
			return UnityEngine.Mathf.Min(values);
		}

		public static float MoveTowards(float current, float target, float maxDelta)
		{
			return UnityEngine.Mathf.MoveTowards(current, target, maxDelta);
		}

		public static int NextPowerOfTwo(int value)
		{
			return UnityEngine.Mathf.NextPowerOfTwo(value);
		}

		public static float PerlinNoise(float x, float y)
		{
			return PerlinNoise(x, y);
		}

		public static float PingPong(float t, float length)
		{
			return UnityEngine.Mathf.PingPong(t, length);
		}

		public static float Pow(float f, float p)
		{
			return UnityEngine.Mathf.Pow(f, p);
		}

		public static float Repeat(float t, float length)
		{
			return UnityEngine.Mathf.Repeat(t, length);
		}

		public static float Round(float f)
		{
			return UnityEngine.Mathf.Round(f);
		}

		public static float Sign(float f)
		{
			return UnityEngine.Mathf.Sign(f);
		}

		public static float Sin(float f)
		{
			return UnityEngine.Mathf.Sin(f);
		}

		public static float Sqrt(float f)
		{
			return UnityEngine.Mathf.Sqrt(f);
		}

		public static float SmoothStep(float from, float to, float t)
		{
			return UnityEngine.Mathf.SmoothStep(from, to, t);
		}

		public static float Tan(float f)
		{
			return UnityEngine.Mathf.Tan(f);
		}

		public static float ConvertToMeter(string measurement)
		{
			return GameController.ConvertHumanReadableToScale(measurement);
		}

		public static float ConvertToMeter(float measurement, string unit)
		{
			return GameController.ConvertHumanReadableToScale(measurement.ToString("0.00") + unit);
		}

		public static string DistanceToString(float distance)
		{
			return GameController.ConvertScaleToHumanReadable(Abs(distance));
		}
	}
}
