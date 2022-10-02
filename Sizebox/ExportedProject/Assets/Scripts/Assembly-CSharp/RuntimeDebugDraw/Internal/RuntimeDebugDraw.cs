using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeDebugDraw.Internal
{
	internal class RuntimeDebugDraw : MonoBehaviour
	{
		private class DrawLineEntry
		{
			public bool occupied;

			public Vector3 start;

			public Vector3 end;

			public Color color;

			public float timer;

			public bool noZTest;
		}

		private class BatchedLineDraw : IDisposable
		{
			public Mesh mesh;

			public Material mat;

			private List<Vector3> _vertices;

			private List<Color> _colors;

			private List<int> _indices;

			public BatchedLineDraw(bool depthTest)
			{
				mesh = new Mesh();
				mesh.MarkDynamic();
				mat = new Material(Shader.Find("Hidden/Internal-Colored"));
				mat.SetInt("_ZTest", depthTest ? 4 : 0);
				_vertices = new List<Vector3>();
				_colors = new List<Color>();
				_indices = new List<int>();
			}

			public void AddLine(Vector3 from, Vector3 to, Color color)
			{
				_vertices.Add(from);
				_vertices.Add(to);
				_colors.Add(color);
				_colors.Add(color);
				int count = _vertices.Count;
				_indices.Add(count - 2);
				_indices.Add(count - 1);
			}

			public void Clear()
			{
				mesh.Clear();
				_vertices.Clear();
				_colors.Clear();
				_indices.Clear();
			}

			public void BuildBatch()
			{
				mesh.SetVertices(_vertices);
				mesh.SetColors(_colors);
				mesh.SetIndices(_indices.ToArray(), MeshTopology.Lines, 0);
			}

			public void Dispose()
			{
				UnityEngine.Object.Destroy(mesh);
				UnityEngine.Object.Destroy(mat);
			}
		}

		[Flags]
		public enum DrawFlag : byte
		{
			None = 0,
			DrawnGizmo = 1,
			DrawnGUI = 2,
			DrawnAll = 3
		}

		private class DrawTextEntry
		{
			public bool occupied;

			public GUIContent content;

			public Vector3 anchor;

			public int size;

			public Color color;

			public float timer;

			public bool popUp;

			public float duration;

			public DrawFlag flag;

			public DrawTextEntry()
			{
				content = new GUIContent();
			}
		}

		private class AttachTextEntry
		{
			public bool occupied;

			public GUIContent content;

			public Vector3 offset;

			public int size;

			public Color color;

			public Transform transform;

			public Func<string> strFunc;

			public DrawFlag flag;

			public AttachTextEntry()
			{
				content = new GUIContent();
			}
		}

		private List<DrawLineEntry> _lineEntries;

		private BatchedLineDraw _ZTestBatch;

		private BatchedLineDraw _AlwaysBatch;

		private bool _linesNeedRebuild;

		private List<DrawTextEntry> _drawTextEntries;

		private List<AttachTextEntry> _attachTextEntries;

		private GUIStyle _textStyle;

		private void Awake()
		{
			_ZTestBatch = new BatchedLineDraw(true);
			_AlwaysBatch = new BatchedLineDraw(false);
			_lineEntries = new List<DrawLineEntry>(16);
			_textStyle = new GUIStyle();
			_textStyle.alignment = TextAnchor.UpperLeft;
			_drawTextEntries = new List<DrawTextEntry>(16);
			_attachTextEntries = new List<AttachTextEntry>(16);
		}

		private void OnGUI()
		{
			DrawTextOnGUI();
		}

		public void LateUpdate()
		{
			TickAndDrawLines();
			TickTexts();
		}

		private void OnDestroy()
		{
			_AlwaysBatch.Dispose();
			_ZTestBatch.Dispose();
		}

		public void RegisterLine(Vector3 start, Vector3 end, Color color, float timer, bool noZTest)
		{
			DrawLineEntry drawLineEntry = null;
			for (int i = 0; i < _lineEntries.Count; i++)
			{
				if (!_lineEntries[i].occupied)
				{
					drawLineEntry = _lineEntries[i];
					break;
				}
			}
			if (drawLineEntry == null)
			{
				drawLineEntry = new DrawLineEntry();
				_lineEntries.Add(drawLineEntry);
			}
			drawLineEntry.occupied = true;
			drawLineEntry.start = start;
			drawLineEntry.end = end;
			drawLineEntry.color = color;
			drawLineEntry.timer = timer;
			drawLineEntry.noZTest = noZTest;
			_linesNeedRebuild = true;
		}

		private void RebuildDrawLineBatchMesh()
		{
			_ZTestBatch.Clear();
			_AlwaysBatch.Clear();
			for (int i = 0; i < _lineEntries.Count; i++)
			{
				DrawLineEntry drawLineEntry = _lineEntries[i];
				if (drawLineEntry.occupied)
				{
					if (drawLineEntry.noZTest)
					{
						_AlwaysBatch.AddLine(drawLineEntry.start, drawLineEntry.end, drawLineEntry.color);
					}
					else
					{
						_ZTestBatch.AddLine(drawLineEntry.start, drawLineEntry.end, drawLineEntry.color);
					}
				}
			}
			_ZTestBatch.BuildBatch();
			_AlwaysBatch.BuildBatch();
		}

		private void TickAndDrawLines()
		{
			if (_linesNeedRebuild)
			{
				RebuildDrawLineBatchMesh();
				_linesNeedRebuild = false;
			}
			Graphics.DrawMesh(_AlwaysBatch.mesh, Vector3.zero, Quaternion.identity, _AlwaysBatch.mat, 4, null, 0, null, false, false);
			Graphics.DrawMesh(_ZTestBatch.mesh, Vector3.zero, Quaternion.identity, _ZTestBatch.mat, 4, null, 0, null, false, false);
			for (int i = 0; i < _lineEntries.Count; i++)
			{
				DrawLineEntry drawLineEntry = _lineEntries[i];
				if (drawLineEntry.occupied)
				{
					drawLineEntry.timer -= Time.deltaTime;
					if (drawLineEntry.timer < 0f)
					{
						drawLineEntry.occupied = false;
						_linesNeedRebuild = true;
					}
				}
			}
		}

		public void RegisterDrawText(Vector3 anchor, string text, Color color, int size, float timer, bool popUp)
		{
			DrawTextEntry drawTextEntry = null;
			for (int i = 0; i < _drawTextEntries.Count; i++)
			{
				if (!_drawTextEntries[i].occupied)
				{
					drawTextEntry = _drawTextEntries[i];
					break;
				}
			}
			if (drawTextEntry == null)
			{
				drawTextEntry = new DrawTextEntry();
				_drawTextEntries.Add(drawTextEntry);
			}
			drawTextEntry.occupied = true;
			drawTextEntry.anchor = anchor;
			drawTextEntry.content.text = text;
			drawTextEntry.size = size;
			drawTextEntry.color = color;
			drawTextEntry.duration = (drawTextEntry.timer = timer);
			drawTextEntry.popUp = popUp;
			drawTextEntry.flag = DrawFlag.DrawnGizmo;
		}

		public void RegisterAttachText(Transform target, Func<string> strFunc, Vector3 offset, Color color, int size)
		{
			AttachTextEntry attachTextEntry = null;
			for (int i = 0; i < _attachTextEntries.Count; i++)
			{
				if (!_attachTextEntries[i].occupied)
				{
					attachTextEntry = _attachTextEntries[i];
					break;
				}
			}
			if (attachTextEntry == null)
			{
				attachTextEntry = new AttachTextEntry();
				_attachTextEntries.Add(attachTextEntry);
			}
			attachTextEntry.occupied = true;
			attachTextEntry.offset = offset;
			attachTextEntry.transform = target;
			attachTextEntry.strFunc = strFunc;
			attachTextEntry.color = color;
			attachTextEntry.size = size;
			attachTextEntry.content.text = strFunc();
			attachTextEntry.flag = DrawFlag.DrawnGizmo;
		}

		private void TickTexts()
		{
			for (int i = 0; i < _drawTextEntries.Count; i++)
			{
				DrawTextEntry drawTextEntry = _drawTextEntries[i];
				if (drawTextEntry.occupied)
				{
					drawTextEntry.timer -= Time.deltaTime;
					if (drawTextEntry.flag == DrawFlag.DrawnAll && drawTextEntry.timer < 0f)
					{
						drawTextEntry.occupied = false;
					}
				}
			}
			for (int j = 0; j < _attachTextEntries.Count; j++)
			{
				AttachTextEntry attachTextEntry = _attachTextEntries[j];
				if (attachTextEntry.occupied)
				{
					if (attachTextEntry.transform == null)
					{
						attachTextEntry.occupied = false;
						attachTextEntry.strFunc = null;
					}
					else if (attachTextEntry.flag == DrawFlag.DrawnAll)
					{
						attachTextEntry.content.text = attachTextEntry.strFunc();
						attachTextEntry.flag = DrawFlag.DrawnGizmo;
					}
				}
			}
		}

		private void DrawTextOnGUI()
		{
			Camera debugDrawCamera = Draw.GetDebugDrawCamera();
			if (debugDrawCamera == null)
			{
				return;
			}
			for (int i = 0; i < _drawTextEntries.Count; i++)
			{
				DrawTextEntry drawTextEntry = _drawTextEntries[i];
				if (drawTextEntry.occupied)
				{
					GUIDrawTextEntry(debugDrawCamera, drawTextEntry);
					drawTextEntry.flag |= DrawFlag.DrawnGUI;
				}
			}
			for (int j = 0; j < _attachTextEntries.Count; j++)
			{
				AttachTextEntry attachTextEntry = _attachTextEntries[j];
				if (attachTextEntry.occupied)
				{
					GUIAttachTextEntry(debugDrawCamera, attachTextEntry);
					attachTextEntry.flag |= DrawFlag.DrawnGUI;
				}
			}
		}

		private void GUIDrawTextEntry(Camera camera, DrawTextEntry entry)
		{
			Vector3 anchor = entry.anchor;
			Vector3 vector = camera.WorldToScreenPoint(anchor);
			vector.y = (float)Screen.height - vector.y;
			if (entry.popUp)
			{
				float num = entry.timer / entry.duration;
				vector.y -= (1f - num * num) * (float)entry.size * 1.5f;
			}
			_textStyle.normal.textColor = entry.color;
			_textStyle.fontSize = entry.size;
			GUI.Label(new Rect(vector, _textStyle.CalcSize(entry.content)), entry.content, _textStyle);
		}

		private void GUIAttachTextEntry(Camera camera, AttachTextEntry entry)
		{
			if (!(entry.transform == null))
			{
				Vector3 position = entry.transform.position + entry.offset;
				Vector3 vector = camera.WorldToScreenPoint(position);
				vector.y = (float)Screen.height - vector.y;
				_textStyle.normal.textColor = entry.color;
				_textStyle.fontSize = entry.size;
				GUI.Label(new Rect(vector, _textStyle.CalcSize(entry.content)), entry.content, _textStyle);
			}
		}
	}
}
