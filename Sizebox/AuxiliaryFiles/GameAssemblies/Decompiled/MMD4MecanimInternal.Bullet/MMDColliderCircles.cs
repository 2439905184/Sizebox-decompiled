using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet;

public class MMDColliderCircles
{
	private struct Segment
	{
		public int count;

		public float start;

		public float interval;
	}

	private int _shape;

	private Vector3 _size;

	private Vector3[] _vertices;

	private Vector3[] _transformVertices;

	private float _radius;

	private float _radius2;

	private float _diameter;

	private int _segments = 3;

	private float _radiusBias = 0.98f;

	public Vector3 size => _size;

	public MMDColliderCircles(int shape_, Vector3 size_)
	{
		_shape = shape_;
		_size = size_;
		switch (_shape)
		{
		case 0:
			_radius = size_[0] * _radiusBias;
			_diameter = _radius * 2f;
			if (_radius > float.Epsilon)
			{
				_vertices = new Vector3[1];
				ref Vector3 reference5 = ref _vertices[0];
				reference5 = Vector3.zero;
			}
			break;
		case 1:
		{
			_radius = Mathf.Min(Mathf.Min(size_[0], size_[1]), size_[2]) * _radiusBias;
			_diameter = _radius * 2f;
			if (!(_radius > float.Epsilon))
			{
				break;
			}
			int num3 = 0;
			Segment segment2 = _ComputeSegment(size_[0] * 2f * _radiusBias);
			Segment segment3 = _ComputeSegment(size_[1] * 2f * _radiusBias);
			Segment segment4 = _ComputeSegment(size_[2] * 2f * _radiusBias);
			if (size_[0] <= size_[1] && size_[0] <= size_[2])
			{
				_vertices = new Vector3[segment3.count * segment4.count];
				float num4 = segment3.start;
				int num5 = 0;
				while (num5 < segment3.count)
				{
					float num6 = segment4.start;
					int num7 = 0;
					while (num7 < segment4.count)
					{
						ref Vector3 reference2 = ref _vertices[num3];
						reference2 = new Vector3(0f, num4, num6);
						num7++;
						num6 += segment4.interval;
						num3++;
					}
					num5++;
					num4 += segment3.interval;
				}
				break;
			}
			if (size_[1] <= size_[0] && size_[1] <= size_[2])
			{
				_vertices = new Vector3[segment2.count * segment4.count];
				float num8 = segment2.start;
				int num9 = 0;
				while (num9 < segment2.count)
				{
					float num10 = segment4.start;
					int num11 = 0;
					while (num11 < segment4.count)
					{
						ref Vector3 reference3 = ref _vertices[num3];
						reference3 = new Vector3(num8, 0f, num10);
						num11++;
						num10 += segment4.interval;
						num3++;
					}
					num9++;
					num8 += segment2.interval;
				}
				break;
			}
			_vertices = new Vector3[segment2.count * segment3.count];
			float num12 = segment2.start;
			int num13 = 0;
			while (num13 < segment2.count)
			{
				float num14 = segment3.start;
				int num15 = 0;
				while (num15 < segment3.count)
				{
					ref Vector3 reference4 = ref _vertices[num3];
					reference4 = new Vector3(num12, num14, 0f);
					num15++;
					num14 += segment3.interval;
					num3++;
				}
				num13++;
				num12 += segment2.interval;
			}
			break;
		}
		case 2:
			_radius = size_[0] * _radiusBias;
			_diameter = _radius * 2f;
			if (_radius > float.Epsilon)
			{
				Segment segment = _ComputeSegment((size_[1] + size_[0] * 2f) * _radiusBias);
				_vertices = new Vector3[segment.count];
				float num = segment.start;
				int num2 = 0;
				while (num2 < segment.count)
				{
					ref Vector3 reference = ref _vertices[num2];
					reference = new Vector3(0f, num, 0f);
					num2++;
					num += segment.interval;
				}
			}
			break;
		}
		_radius2 = _radius * _radius;
		if (_vertices != null)
		{
			_transformVertices = new Vector3[_vertices.Length];
		}
		else
		{
			_transformVertices = new Vector3[0];
		}
	}

	public void Transform(IndexedMatrix transform)
	{
		if (_transformVertices != null && _vertices != null)
		{
			for (int i = 0; i < _vertices.Length; i++)
			{
				ref Vector3 reference = ref _transformVertices[i];
				reference = transform * _vertices[i];
			}
		}
	}

	public void ForceTranslate(Vector3 movedTranslate)
	{
		if (_transformVertices != null)
		{
			for (int i = 0; i < _transformVertices.Length; i++)
			{
				_transformVertices[i] += movedTranslate;
			}
		}
	}

	public Vector3[] GetTransformVertices()
	{
		return _transformVertices;
	}

	public float GetRadius()
	{
		return _radius;
	}

	public float GetRadius2()
	{
		return _radius2;
	}

	private Segment _ComputeSegment(float len_)
	{
		if (_radius <= float.Epsilon)
		{
			return default(Segment);
		}
		Segment result = default(Segment);
		result.count = Mathf.Max((int)(len_ / _diameter) + 1, _segments);
		if (result.count > 1)
		{
			float num = Mathf.Max(len_ - _diameter, 0f);
			result.start = (0f - num) * 0.5f;
			result.interval = num / (float)(result.count - 1);
		}
		return result;
	}
}
