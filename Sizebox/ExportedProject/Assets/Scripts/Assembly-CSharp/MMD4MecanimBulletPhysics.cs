using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using MMD4MecanimInternal.Bullet;
using UnityEngine;

public class MMD4MecanimBulletPhysics : MonoBehaviour
{
	public class MMD4MecanimBulletBridge : IBridge
	{
		public class CachedThreadQueue
		{
			private class Queue
			{
				public Action function;

				public uint queueID;

				public uint uniqueID;

				public bool processingWaitEnd;

				public ManualResetEvent processedEvent;

				public Queue(Action function)
				{
					this.function = function;
					queueID = 0u;
					uniqueID = 0u;
					processedEvent = new ManualResetEvent(false);
				}

				public void Unuse()
				{
					function = null;
					queueID++;
				}

				public void Reuse(Action function)
				{
					this.function = function;
					processedEvent.Reset();
				}
			}

			private Mutex _lockMutex = new Mutex();

			private ManualResetEvent _invokeEvent = new ManualResetEvent(false);

			private ArrayList _threads = new ArrayList();

			private int _maxThreads;

			private bool _isFinalized;

			private uint _uniqueID;

			private ArrayList _processingQueues = new ArrayList();

			private ArrayList _reservedQueues = new ArrayList();

			private ArrayList _unusedQueues = new ArrayList();

			private static Queue _FindQueue(ArrayList queues, ref ThreadQueueHandle queueHandle)
			{
				if (queues != null)
				{
					for (int i = 0; i != queues.Count; i++)
					{
						Queue queue = (Queue)queues[i];
						if (queue == queueHandle.queuePtr && queue.queueID == queueHandle.queueID)
						{
							return queue;
						}
					}
				}
				return null;
			}

			public CachedThreadQueue()
			{
				_maxThreads = GetProcessCount();
			}

			public CachedThreadQueue(int maxThreads)
			{
				_maxThreads = maxThreads;
				if (_maxThreads <= 0)
				{
					_maxThreads = Mathf.Max(GetProcessCount(), 1);
				}
			}

			~CachedThreadQueue()
			{
				if (_threads.Count != 0)
				{
					_Finalize();
				}
			}

			public void _Finalize()
			{
				_lockMutex.WaitOne();
				bool isFinalized = _isFinalized;
				_isFinalized = true;
				if (!isFinalized)
				{
					_invokeEvent.Set();
				}
				_lockMutex.ReleaseMutex();
				if (!isFinalized)
				{
					for (int i = 0; i != _threads.Count; i++)
					{
						((Thread)_threads[i]).Join();
					}
					_threads.Clear();
					_lockMutex.WaitOne();
					_isFinalized = false;
					_lockMutex.ReleaseMutex();
				}
			}

			public ThreadQueueHandle Invoke(Action function)
			{
				ThreadQueueHandle result = default(ThreadQueueHandle);
				if (function == null)
				{
					return result;
				}
				bool flag = false;
				_lockMutex.WaitOne();
				flag = _isFinalized;
				if (!flag)
				{
					if (_processingQueues.Count == _threads.Count && _threads.Count < _maxThreads)
					{
						Thread thread = new Thread(_Run);
						_threads.Add(thread);
						thread.Start();
					}
					Queue queue = null;
					for (int num = _unusedQueues.Count - 1; num >= 0; num--)
					{
						queue = (Queue)_unusedQueues[num];
						if (!queue.processingWaitEnd)
						{
							_unusedQueues.RemoveAt(num);
							queue.Reuse(function);
							break;
						}
						queue = null;
					}
					if (queue == null)
					{
						queue = new Queue(function);
					}
					queue.uniqueID = _uniqueID;
					_uniqueID++;
					_reservedQueues.Add(queue);
					result.queuePtr = queue;
					result.queueID = queue.queueID;
					result.uniqueID = queue.uniqueID;
					queue = null;
					_invokeEvent.Set();
				}
				_lockMutex.ReleaseMutex();
				if (flag)
				{
					function();
				}
				return result;
			}

			public void WaitEnd(ref ThreadQueueHandle queueHandle)
			{
				if (queueHandle.queuePtr == null)
				{
					return;
				}
				Queue queue = null;
				_lockMutex.WaitOne();
				queue = _FindQueue(_processingQueues, ref queueHandle);
				if (queue == null)
				{
					queue = _FindQueue(_reservedQueues, ref queueHandle);
				}
				if (queue != null)
				{
					queue.processingWaitEnd = true;
				}
				_lockMutex.ReleaseMutex();
				if (queue == null)
				{
					queueHandle.Reset();
					return;
				}
				do
				{
					InstantSleep();
					queue.processedEvent.WaitOne();
					_lockMutex.WaitOne();
					if (queue.queueID != queueHandle.queueID)
					{
						queue.processingWaitEnd = false;
						queue = null;
					}
					_lockMutex.ReleaseMutex();
				}
				while (queue != null);
				queueHandle.Reset();
			}

			private void _Run()
			{
				while (true)
				{
					Queue queue = null;
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					_invokeEvent.WaitOne();
					_lockMutex.WaitOne();
					if (_reservedQueues.Count != 0)
					{
						queue = (Queue)_reservedQueues[0];
						_reservedQueues.RemoveAt(0);
						_processingQueues.Add(queue);
					}
					flag = queue != null;
					flag2 = _isFinalized;
					flag3 = _processingQueues.Count == 0 && _reservedQueues.Count == 0;
					_lockMutex.ReleaseMutex();
					if (queue != null)
					{
						if (queue.function != null)
						{
							queue.function();
						}
						_lockMutex.WaitOne();
						queue.Unuse();
						_processingQueues.Remove(queue);
						_unusedQueues.Add(queue);
						queue.processedEvent.Set();
						queue = null;
						flag2 = _isFinalized;
						flag3 = _processingQueues.Count == 0 && _reservedQueues.Count == 0;
						if (flag3)
						{
							_invokeEvent.Reset();
						}
						_lockMutex.ReleaseMutex();
					}
					if (!(flag3 && flag2))
					{
						if (!flag)
						{
							InstantSleep();
						}
						continue;
					}
					break;
				}
			}
		}

		public class CachedPararellThreadQueue
		{
			private class Queue
			{
				public PararellFunction function;

				public int length;

				public int processingThreads;

				public int processedThreads;

				public uint queueID;

				public uint uniqueID;

				public bool processingWaitEnd;

				public ManualResetEvent processedEvent;

				public Queue(PararellFunction function, int length)
				{
					this.function = function;
					this.length = length;
					processingThreads = 0;
					processedThreads = 0;
					queueID = 0u;
					uniqueID = 0u;
					processedEvent = new ManualResetEvent(false);
				}

				public void Unuse()
				{
					function = null;
					length = 0;
					processingThreads = 0;
					processedThreads = 0;
					queueID++;
				}

				public void Reuse(PararellFunction function, int length)
				{
					this.function = function;
					this.length = length;
					processedEvent.Reset();
				}
			}

			private Mutex _lockMutex = new Mutex();

			private ManualResetEvent _invokeEvent = new ManualResetEvent(false);

			private Thread[] _threads;

			private int _maxThreads;

			private bool _isFinalized;

			private uint _uniqueID;

			private ArrayList _processedQueues = new ArrayList();

			private Queue _processingQueue;

			private ArrayList _reservedQueues = new ArrayList();

			private ArrayList _unusedQueues = new ArrayList();

			private static bool _IsEqualQueue(Queue queue, ref ThreadQueueHandle queueHandle)
			{
				if (queue != null && queue == queueHandle.queuePtr && queue.queueID == queueHandle.queueID)
				{
					return true;
				}
				return false;
			}

			private static Queue _FindQueue(ArrayList queues, ref ThreadQueueHandle queueHandle)
			{
				if (queues != null)
				{
					for (int i = 0; i != queues.Count; i++)
					{
						Queue queue = (Queue)queues[i];
						if (queue == queueHandle.queuePtr && queue.queueID == queueHandle.queueID)
						{
							return queue;
						}
					}
				}
				return null;
			}

			private void _AwakeThread()
			{
				if (_threads == null)
				{
					_threads = new Thread[_maxThreads];
					for (int i = 0; i != _maxThreads; i++)
					{
						Thread thread = new Thread(_Run);
						_threads[i] = thread;
						thread.Start();
					}
				}
			}

			public CachedPararellThreadQueue()
			{
				_maxThreads = GetProcessCount();
			}

			public CachedPararellThreadQueue(int maxThreads)
			{
				_maxThreads = maxThreads;
				if (_maxThreads <= 0)
				{
					_maxThreads = Mathf.Max(GetProcessCount(), 1);
				}
			}

			~CachedPararellThreadQueue()
			{
				if (_threads != null)
				{
					_Finalize();
				}
			}

			public void _Finalize()
			{
				_lockMutex.WaitOne();
				bool isFinalized = _isFinalized;
				_isFinalized = true;
				if (!isFinalized)
				{
					_invokeEvent.Set();
				}
				_lockMutex.ReleaseMutex();
				if (isFinalized)
				{
					return;
				}
				if (_threads != null)
				{
					for (int i = 0; i != _threads.Length; i++)
					{
						_threads[i].Join();
					}
					_threads = null;
				}
				_lockMutex.WaitOne();
				_isFinalized = false;
				_lockMutex.ReleaseMutex();
			}

			public ThreadQueueHandle Invoke(PararellFunction function, int length)
			{
				ThreadQueueHandle result = default(ThreadQueueHandle);
				if (function == null)
				{
					return result;
				}
				bool flag = false;
				_lockMutex.WaitOne();
				flag = _isFinalized;
				if (!flag)
				{
					_AwakeThread();
					Queue queue = null;
					for (int num = _unusedQueues.Count - 1; num >= 0; num--)
					{
						queue = (Queue)_unusedQueues[num];
						if (!queue.processingWaitEnd)
						{
							_unusedQueues.RemoveAt(num);
							queue.Reuse(function, length);
							break;
						}
						queue = null;
					}
					if (queue == null)
					{
						queue = new Queue(function, length);
					}
					queue.uniqueID = _uniqueID;
					_uniqueID++;
					_reservedQueues.Add(queue);
					result.queuePtr = queue;
					result.queueID = queue.queueID;
					result.uniqueID = queue.uniqueID;
					queue = null;
					_invokeEvent.Set();
				}
				_lockMutex.ReleaseMutex();
				if (flag)
				{
					function(0, length);
				}
				return result;
			}

			public void WaitEnd(ref ThreadQueueHandle queueHandle)
			{
				if (queueHandle.queuePtr == null)
				{
					return;
				}
				Queue queue = null;
				_lockMutex.WaitOne();
				queue = _FindQueue(_processedQueues, ref queueHandle);
				if (queue == null)
				{
					queue = ((!_IsEqualQueue(_processingQueue, ref queueHandle)) ? _FindQueue(_reservedQueues, ref queueHandle) : _processingQueue);
				}
				if (queue != null)
				{
					queue.processingWaitEnd = true;
				}
				_lockMutex.ReleaseMutex();
				if (queue == null)
				{
					queueHandle.Reset();
					return;
				}
				do
				{
					InstantSleep();
					queue.processedEvent.WaitOne();
					_lockMutex.WaitOne();
					if (queue.queueID != queueHandle.queueID)
					{
						queue.processingWaitEnd = false;
						queue = null;
					}
					_lockMutex.ReleaseMutex();
				}
				while (queue != null);
				queueHandle.Reset();
			}

			private void _Run()
			{
				while (true)
				{
					Queue queue = null;
					int num = 0;
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					_invokeEvent.WaitOne();
					_lockMutex.WaitOne();
					if (_processingQueue != null)
					{
						queue = _processingQueue;
					}
					else if (_reservedQueues.Count != 0)
					{
						queue = (Queue)_reservedQueues[0];
						_reservedQueues.RemoveAt(0);
						_processingQueue = queue;
					}
					if (queue != null)
					{
						num = queue.processingThreads;
						queue.processingThreads++;
						if (queue.processingThreads == _maxThreads)
						{
							_processingQueue = null;
							_processedQueues.Add(queue);
						}
					}
					flag = queue != null;
					flag2 = _isFinalized;
					flag3 = _processingQueue == null && _reservedQueues.Count == 0;
					_lockMutex.ReleaseMutex();
					if (queue != null)
					{
						int num2 = (queue.length + _maxThreads - 1) / _maxThreads;
						int num3 = num * num2;
						if (num3 < queue.length)
						{
							if (num3 + num2 > queue.length)
							{
								num2 = queue.length - num3;
							}
							if (queue.function != null)
							{
								queue.function(num3, num2);
							}
						}
						_lockMutex.WaitOne();
						if (++queue.processedThreads == _maxThreads)
						{
							queue.Unuse();
							_processedQueues.Remove(queue);
							_unusedQueues.Add(queue);
							queue.processedEvent.Set();
							queue = null;
							flag2 = _isFinalized;
							flag3 = _processingQueue == null && _reservedQueues.Count == 0;
							if (flag3)
							{
								_invokeEvent.Reset();
							}
						}
						_lockMutex.ReleaseMutex();
					}
					if (!(flag3 && flag2))
					{
						if (!flag)
						{
							InstantSleep();
						}
						continue;
					}
					break;
				}
			}
		}

		object IBridge.CreateCachedThreadQueue(int maxThreads)
		{
			return new CachedThreadQueue(maxThreads);
		}

		ThreadQueueHandle IBridge.InvokeCachedThreadQueue(object cachedThreadQueue, Action action)
		{
			if (cachedThreadQueue != null)
			{
				return ((CachedThreadQueue)cachedThreadQueue).Invoke(action);
			}
			return default(ThreadQueueHandle);
		}

		void IBridge.WaitEndCachedThreadQueue(object cachedThreadQueue, ref ThreadQueueHandle threadQueueHandle)
		{
			if (cachedThreadQueue != null)
			{
				((CachedThreadQueue)cachedThreadQueue).WaitEnd(ref threadQueueHandle);
			}
		}

		object IBridge.CreatePararellCachedThreadQueue(int maxThreads)
		{
			return new CachedPararellThreadQueue(maxThreads);
		}

		ThreadQueueHandle IBridge.InvokeCachedPararellThreadQueue(object cachedPararellThreadQueue, PararellFunction function, int length)
		{
			if (cachedPararellThreadQueue != null)
			{
				return ((CachedPararellThreadQueue)cachedPararellThreadQueue).Invoke(function, length);
			}
			return default(ThreadQueueHandle);
		}

		void IBridge.WaitEndCachedPararellThreadQueue(object cachedPararellThreadQueue, ref ThreadQueueHandle threadQueueHandle)
		{
			if (cachedPararellThreadQueue != null)
			{
				((CachedPararellThreadQueue)cachedPararellThreadQueue).WaitEnd(ref threadQueueHandle);
			}
		}

		public static void InstantSleep()
		{
			Thread.Sleep(0);
		}

		public static void ShortlySleep()
		{
			Thread.Sleep(1);
		}

		public static int GetProcessCount()
		{
			return 4;
		}
	}

	public class World
	{
		public WorldProperty worldProperty;

		public IntPtr worldPtr;

		public PhysicsWorld bulletPhysicsWorld;

		private float _gravityScaleCached = 10f;

		private float _gravityNoiseCached;

		private Vector3 _gravityDirectionCached = new Vector3(0f, -1f, 0f);

		private bool _isDirtyProperty = true;

		private MMD4MecanimCommon.PropertyWriter _updatePropertyWriter = new MMD4MecanimCommon.PropertyWriter();

		private WorldUpdateProperty _worldUpdateProperty = new WorldUpdateProperty();

		public bool isExpired
		{
			get
			{
				if (bulletPhysicsWorld != null)
				{
					return false;
				}
				return worldPtr == IntPtr.Zero;
			}
		}

		~World()
		{
			Destroy();
		}

		public bool Create()
		{
			return Create(null);
		}

		public bool Create(WorldProperty worldProperty)
		{
			Destroy();
			if (worldProperty != null)
			{
				this.worldProperty = worldProperty;
			}
			else
			{
				this.worldProperty = new WorldProperty();
			}
			_gravityScaleCached = this.worldProperty.gravityScale;
			_gravityNoiseCached = this.worldProperty.gravityNoise;
			_gravityDirectionCached = this.worldProperty.gravityDirection;
			if (_isUseBulletXNA)
			{
				bulletPhysicsWorld = new PhysicsWorld();
				if (!bulletPhysicsWorld.Create(this.worldProperty))
				{
					bulletPhysicsWorld.Destroy();
					bulletPhysicsWorld = null;
					return false;
				}
				return true;
			}
			if (this.worldProperty != null)
			{
				Vector3 gravityDirection = this.worldProperty.gravityDirection;
				gravityDirection.z = 0f - gravityDirection.z;
				MMD4MecanimCommon.PropertyWriter propertyWriter = new MMD4MecanimCommon.PropertyWriter();
				propertyWriter.Write("accurateStep", this.worldProperty.accurateStep);
				propertyWriter.Write("optimizeSettings", this.worldProperty.optimizeSettings);
				propertyWriter.Write("multiThreading", this.worldProperty.multiThreading);
				propertyWriter.Write("parallelDispatcher", this.worldProperty.parallelDispatcher);
				propertyWriter.Write("parallelSolver", this.worldProperty.parallelSolver);
				propertyWriter.Write("framePerSecond", this.worldProperty.framePerSecond);
				propertyWriter.Write("resetFrameRate", this.worldProperty.resetFrameRate);
				propertyWriter.Write("limitDeltaFrames", this.worldProperty.limitDeltaFrames);
				propertyWriter.Write("axisSweepDistance", this.worldProperty.axisSweepDistance);
				propertyWriter.Write("gravityScale", this.worldProperty.gravityScale);
				propertyWriter.Write("gravityNoise", this.worldProperty.gravityNoise);
				propertyWriter.Write("gravityDirection", gravityDirection);
				propertyWriter.Write("vertexScale", this.worldProperty.vertexScale);
				propertyWriter.Write("importScale", this.worldProperty.importScale);
				propertyWriter.Write("worldSolverInfoNumIterations", this.worldProperty.worldSolverInfoNumIterations);
				propertyWriter.Write("worldSolverInfoSplitImpulse", this.worldProperty.worldSolverInfoSplitImpulse);
				propertyWriter.Write("worldAddFloorPlane", this.worldProperty.worldAddFloorPlane);
				propertyWriter.Lock();
				worldPtr = _CreateWorld(propertyWriter.iValuesPtr, propertyWriter.iValueLength, propertyWriter.fValuesPtr, propertyWriter.fValueLength);
				propertyWriter.Unlock();
			}
			else
			{
				worldPtr = _CreateWorld(IntPtr.Zero, 0, IntPtr.Zero, 0);
			}
			DebugLog();
			return worldPtr != IntPtr.Zero;
		}

		public void SetGravity(float gravityScale, float gravityNoise, Vector3 gravityDirection)
		{
			if (_gravityScaleCached != gravityScale || _gravityNoiseCached != gravityNoise || _gravityDirectionCached != gravityDirection)
			{
				_gravityScaleCached = gravityScale;
				_gravityNoiseCached = gravityNoise;
				_gravityDirectionCached = gravityDirection;
				_isDirtyProperty = true;
				worldProperty.gravityScale = gravityScale;
				worldProperty.gravityNoise = gravityNoise;
				worldProperty.gravityDirection = gravityDirection;
			}
		}

		public void Destroy()
		{
			if (bulletPhysicsWorld != null)
			{
				bulletPhysicsWorld.Destroy();
				bulletPhysicsWorld = null;
			}
			if (worldPtr != IntPtr.Zero)
			{
				IntPtr intPtr = worldPtr;
				worldPtr = IntPtr.Zero;
				_DestroyWorld(intPtr);
			}
			worldProperty = null;
		}

		public void Update(float deltaTime)
		{
			if (worldProperty.gravityScale != _gravityScaleCached || worldProperty.gravityNoise != _gravityNoiseCached || worldProperty.gravityDirection != _gravityDirectionCached)
			{
				_gravityScaleCached = worldProperty.gravityScale;
				_gravityNoiseCached = worldProperty.gravityNoise;
				_gravityDirectionCached = worldProperty.gravityDirection;
				_isDirtyProperty = true;
			}
			if (bulletPhysicsWorld != null)
			{
				if (_isDirtyProperty)
				{
					_isDirtyProperty = false;
					_worldUpdateProperty.gravityScale = _gravityScaleCached;
					_worldUpdateProperty.gravityNoise = _gravityNoiseCached;
					_worldUpdateProperty.gravityDirection = _gravityDirectionCached;
					bulletPhysicsWorld.Update(deltaTime, _worldUpdateProperty);
				}
				else
				{
					bulletPhysicsWorld.Update(deltaTime, null);
				}
			}
			else if (worldPtr != IntPtr.Zero)
			{
				if (_isDirtyProperty)
				{
					_isDirtyProperty = false;
					_updatePropertyWriter.Clear();
					_updatePropertyWriter.Write("gravityScale", _gravityScaleCached);
					_updatePropertyWriter.Write("gravityNoise", _gravityNoiseCached);
					_updatePropertyWriter.Write("gravityDirection", _gravityDirectionCached);
					_updatePropertyWriter.Lock();
					_UpdateWorld(worldPtr, deltaTime, _updatePropertyWriter.iValuesPtr, _updatePropertyWriter.iValueLength, _updatePropertyWriter.fValuesPtr, _updatePropertyWriter.fValueLength);
					_updatePropertyWriter.Unlock();
				}
				else
				{
					_UpdateWorld(worldPtr, deltaTime, IntPtr.Zero, 0, IntPtr.Zero, 0);
				}
			}
		}
	}

	public class RigidBody
	{
		[Flags]
		public enum UpdateFlags
		{
			Freezed = 1
		}

		public MMD4MecanimRigidBody rigidBody;

		public IntPtr rigidBodyPtr;

		public MMD4MecanimInternal.Bullet.RigidBody bulletRigidBody;

		private float[] fValues = new float[7];

		private SphereCollider _sphereCollider;

		private BoxCollider _boxCollider;

		private CapsuleCollider _capsuleCollider;

		private bool ignoreBulletPhysics = true;

		private Vector3 _center
		{
			get
			{
				if (_sphereCollider != null)
				{
					return _sphereCollider.center;
				}
				if (_boxCollider != null)
				{
					return _boxCollider.center;
				}
				if (_capsuleCollider != null)
				{
					return _capsuleCollider.center;
				}
				return Vector3.zero;
			}
		}

		public bool isExpired
		{
			get
			{
				if (bulletRigidBody != null)
				{
					return false;
				}
				return rigidBodyPtr == IntPtr.Zero;
			}
		}

		~RigidBody()
		{
			Destroy();
		}

		public bool Create(MMD4MecanimRigidBody rigidBody)
		{
			Destroy();
			if (rigidBody == null)
			{
				return false;
			}
			World world = null;
			if (instance != null)
			{
				world = instance.globalWorld;
			}
			if (world == null)
			{
				return false;
			}
			MMD4MecanimInternal.Bullet.RigidBody.CreateProperty createProperty = default(MMD4MecanimInternal.Bullet.RigidBody.CreateProperty);
			bool isUseBulletXNA = _isUseBulletXNA;
			MMD4MecanimCommon.PropertyWriter propertyWriter = (isUseBulletXNA ? null : new MMD4MecanimCommon.PropertyWriter());
			Matrix4x4 matrix = rigidBody.transform.localToWorldMatrix;
			Vector3 vector = rigidBody.transform.position;
			Quaternion rotation = rigidBody.transform.rotation;
			Vector3 vector2 = MMD4MecanimCommon.ComputeMatrixScale(ref matrix);
			Vector3 center = _center;
			if (center != Vector3.zero)
			{
				vector = matrix.MultiplyPoint3x4(center);
			}
			SphereCollider component = rigidBody.gameObject.GetComponent<SphereCollider>();
			if (component != null)
			{
				float radius = component.radius;
				radius *= Mathf.Max(Mathf.Max(vector2.x, vector2.y), vector2.z);
				if (propertyWriter != null)
				{
					propertyWriter.Write("shapeType", 0);
					propertyWriter.Write("shapeSize", new Vector3(radius, 0f, 0f));
				}
				if (isUseBulletXNA)
				{
					createProperty.shapeType = 0;
					createProperty.shapeSize = new Vector3(radius, 0f, 0f);
				}
			}
			BoxCollider component2 = rigidBody.gameObject.GetComponent<BoxCollider>();
			if (component2 != null)
			{
				Vector3 size = component2.size;
				size.x *= vector2.x;
				size.y *= vector2.y;
				size.z *= vector2.z;
				if (propertyWriter != null)
				{
					propertyWriter.Write("shapeType", 1);
					propertyWriter.Write("shapeSize", size * 0.5f);
				}
				if (isUseBulletXNA)
				{
					createProperty.shapeType = 1;
					createProperty.shapeSize = size * 0.5f;
				}
			}
			CapsuleCollider component3 = rigidBody.gameObject.GetComponent<CapsuleCollider>();
			if (component3 != null)
			{
				Vector3 vector3 = new Vector3(component3.radius, component3.height, 0f);
				vector3.x *= Mathf.Max(vector2.x, vector2.z);
				vector3.y *= vector2.y;
				vector3.y -= component3.radius * 2f;
				if (propertyWriter != null)
				{
					propertyWriter.Write("shapeType", 2);
					propertyWriter.Write("shapeSize", vector3);
				}
				if (isUseBulletXNA)
				{
					createProperty.shapeType = 2;
					createProperty.shapeSize = vector3;
				}
			}
			_sphereCollider = component;
			_boxCollider = component2;
			_capsuleCollider = component3;
			if (component3 != null)
			{
				if (component3.direction == 0)
				{
					rotation *= rotateQuaternionX;
				}
				else if (component3.direction == 2)
				{
					rotation *= rotateQuaternionZ;
				}
			}
			if (world.worldProperty != null)
			{
				if (propertyWriter != null)
				{
					propertyWriter.Write("worldScale", world.worldProperty.worldScale);
				}
				if (isUseBulletXNA)
				{
					createProperty.unityScale = world.worldProperty.worldScale;
				}
			}
			else
			{
				if (propertyWriter != null)
				{
					propertyWriter.Write("worldScale", 1f);
				}
				if (isUseBulletXNA)
				{
					createProperty.unityScale = 1f;
				}
			}
			if (propertyWriter != null)
			{
				propertyWriter.Write("position", vector);
				propertyWriter.Write("rotation", rotation);
			}
			if (isUseBulletXNA)
			{
				createProperty.position = vector;
				createProperty.rotation = rotation;
			}
			int num = 0;
			if (rigidBody.bulletPhysicsRigidBodyProperty != null)
			{
				if (rigidBody.bulletPhysicsRigidBodyProperty.isKinematic)
				{
					num |= 1;
				}
				if (rigidBody.bulletPhysicsRigidBodyProperty.isAdditionalDamping)
				{
					num |= 2;
				}
				if (isUseBulletXNA)
				{
					createProperty.isKinematic = rigidBody.bulletPhysicsRigidBodyProperty.isKinematic;
					createProperty.isAdditionalDamping = rigidBody.bulletPhysicsRigidBodyProperty.isAdditionalDamping;
				}
				float num2 = rigidBody.bulletPhysicsRigidBodyProperty.mass;
				if (rigidBody.bulletPhysicsRigidBodyProperty.isKinematic)
				{
					num2 = 0f;
				}
				if (propertyWriter != null)
				{
					propertyWriter.Write("mass", num2);
					propertyWriter.Write("linearDamping", rigidBody.bulletPhysicsRigidBodyProperty.linearDamping);
					propertyWriter.Write("angularDamping", rigidBody.bulletPhysicsRigidBodyProperty.angularDamping);
					propertyWriter.Write("restitution", rigidBody.bulletPhysicsRigidBodyProperty.restitution);
					propertyWriter.Write("friction", rigidBody.bulletPhysicsRigidBodyProperty.friction);
				}
				if (isUseBulletXNA)
				{
					createProperty.mass = num2;
					createProperty.linearDamping = rigidBody.bulletPhysicsRigidBodyProperty.linearDamping;
					createProperty.angularDamping = rigidBody.bulletPhysicsRigidBodyProperty.angularDamping;
					createProperty.restitution = rigidBody.bulletPhysicsRigidBodyProperty.restitution;
					createProperty.friction = rigidBody.bulletPhysicsRigidBodyProperty.friction;
				}
			}
			if (propertyWriter != null)
			{
				propertyWriter.Write("flags", num);
				propertyWriter.Write("group", 65535);
				propertyWriter.Write("mask", 65535);
			}
			if (isUseBulletXNA)
			{
				createProperty.group = 65535;
				createProperty.mask = 65535;
			}
			if (isUseBulletXNA)
			{
				bulletRigidBody = new MMD4MecanimInternal.Bullet.RigidBody();
				if (!bulletRigidBody.Create(ref createProperty))
				{
					bulletRigidBody.Destroy();
					bulletRigidBody = null;
					return false;
				}
				if (world.bulletPhysicsWorld != null)
				{
					world.bulletPhysicsWorld.JoinWorld(bulletRigidBody);
				}
				this.rigidBody = rigidBody;
				return true;
			}
			propertyWriter.Lock();
			IntPtr intPtr = _CreateRigidBody(propertyWriter.iValuesPtr, propertyWriter.iValueLength, propertyWriter.fValuesPtr, propertyWriter.fValueLength);
			propertyWriter.Unlock();
			if (intPtr != IntPtr.Zero)
			{
				_JoinWorldRigidBody(world.worldPtr, intPtr);
				DebugLog();
				this.rigidBody = rigidBody;
				rigidBodyPtr = intPtr;
				return true;
			}
			DebugLog();
			return false;
		}

		public void Update()
		{
			if (!(rigidBody != null) || rigidBody.bulletPhysicsRigidBodyProperty == null)
			{
				return;
			}
			bool isKinematic = rigidBody.bulletPhysicsRigidBodyProperty.isKinematic;
			bool isFreezed = rigidBody.bulletPhysicsRigidBodyProperty.isFreezed;
			if (isKinematic || isFreezed)
			{
				int num = 0;
				if (isFreezed)
				{
					num |= 1;
				}
				Vector3 position = rigidBody.transform.position;
				Quaternion rotation = rigidBody.transform.rotation;
				Vector3 center = _center;
				if (center != Vector3.zero)
				{
					position = rigidBody.transform.localToWorldMatrix.MultiplyPoint3x4(center);
				}
				if (_capsuleCollider != null)
				{
					if (_capsuleCollider.direction == 0)
					{
						rotation *= rotateQuaternionX;
					}
					else if (_capsuleCollider.direction == 2)
					{
						rotation *= rotateQuaternionZ;
					}
				}
				if (bulletRigidBody != null)
				{
					bulletRigidBody.Update(num, ref position, ref rotation);
				}
				if (rigidBodyPtr != IntPtr.Zero)
				{
					fValues[0] = position.x;
					fValues[1] = position.y;
					fValues[2] = position.z;
					fValues[3] = rotation.x;
					fValues[4] = rotation.y;
					fValues[5] = rotation.z;
					fValues[6] = rotation.w;
					GCHandle gCHandle = GCHandle.Alloc(fValues, GCHandleType.Pinned);
					_UpdateRigidBody(rigidBodyPtr, num, IntPtr.Zero, 0, gCHandle.AddrOfPinnedObject(), fValues.Length);
					gCHandle.Free();
				}
			}
			else
			{
				if (bulletRigidBody != null)
				{
					bulletRigidBody.Update(0);
				}
				if (rigidBodyPtr != IntPtr.Zero)
				{
					GCHandle gCHandle2 = GCHandle.Alloc(fValues, GCHandleType.Pinned);
					_UpdateRigidBody(rigidBodyPtr, 0, IntPtr.Zero, 0, IntPtr.Zero, 0);
					gCHandle2.Free();
				}
			}
		}

		public void LateUpdate()
		{
			if (ignoreBulletPhysics || !(rigidBody != null) || rigidBody.bulletPhysicsRigidBodyProperty == null)
			{
				return;
			}
			bool isKinematic = rigidBody.bulletPhysicsRigidBodyProperty.isKinematic;
			bool isFreezed = rigidBody.bulletPhysicsRigidBodyProperty.isFreezed;
			if (isKinematic || isFreezed)
			{
				return;
			}
			Vector3 position = Vector3.one;
			Quaternion rotation = Quaternion.identity;
			int num = 0;
			if (bulletRigidBody != null)
			{
				num = bulletRigidBody.LateUpdate(ref position, ref rotation);
			}
			if (rigidBodyPtr != IntPtr.Zero)
			{
				GCHandle gCHandle = GCHandle.Alloc(fValues, GCHandleType.Pinned);
				num = _LateUpdateRigidBody(rigidBodyPtr, IntPtr.Zero, 0, gCHandle.AddrOfPinnedObject(), fValues.Length);
				gCHandle.Free();
				position = new Vector3(fValues[0], fValues[1], fValues[2]);
				rotation = new Quaternion(fValues[3], fValues[4], fValues[5], fValues[6]);
			}
			if (num == 0)
			{
				return;
			}
			if (_capsuleCollider != null)
			{
				if (_capsuleCollider.direction == 0)
				{
					rotation *= rotateQuaternionXInv;
				}
				else if (_capsuleCollider.direction == 2)
				{
					rotation *= rotateQuaternionZInv;
				}
			}
			rigidBody.gameObject.transform.position = position;
			rigidBody.gameObject.transform.rotation = rotation;
			Vector3 center = _center;
			if (center != Vector3.zero)
			{
				Vector3 localPosition = rigidBody.gameObject.transform.localPosition;
				localPosition -= center;
				rigidBody.gameObject.transform.localPosition = localPosition;
			}
		}

		public void Destroy()
		{
			if (bulletRigidBody != null)
			{
				bulletRigidBody.Destroy();
				bulletRigidBody = null;
			}
			if (rigidBodyPtr != IntPtr.Zero)
			{
				IntPtr intPtr = rigidBodyPtr;
				rigidBodyPtr = IntPtr.Zero;
				_DestroyRigidBody(intPtr);
			}
			_sphereCollider = null;
			_boxCollider = null;
			_capsuleCollider = null;
			rigidBody = null;
		}
	}

	public class MMDModel
	{
		public enum UpdateIValues
		{
			AnimationHashName = 0,
			Max = 1
		}

		public enum UpdateFValues
		{
			AnimationTime = 0,
			PPHShoulderFixRate = 1,
			Max = 2
		}

		[Flags]
		public enum LateUpdateMeshFlags
		{
			Vertices = 1,
			Normals = 2
		}

		[Flags]
		public enum UpdateFlags
		{
			IKEnabled = 1,
			BoneInherenceEnabled = 2,
			BoneMorphEnabled = 4,
			PPHShoulderEnabled = 8,
			FullBodyIKEnabled = 0x10
		}

		[Flags]
		public enum UpdateBoneFlags
		{
			WorldTransform = 1,
			Position = 2,
			Rotation = 4,
			UserPosition = 0x80,
			UserRotation = 0x100,
			SkeletonMask = -16777216,
			SkeletonLeftShoulder = 0x1000000,
			SkeletonLeftUpperArm = 0x2000000,
			SkeletonRightShoulder = 0x3000000,
			SkeletonRightUpperArm = 0x4000000,
			UserTransform = 0x180
		}

		[Flags]
		public enum UpdateRigidBodyFlags
		{
			Freezed = 1
		}

		[Flags]
		public enum LateUpdateFlags
		{
			Bone = 1,
			Mesh = 2
		}

		[Flags]
		public enum LateUpdateBoneFlags
		{
			LateUpdated = 1,
			Position = 2,
			Rotation = 4
		}

		public World localWorld;

		public MMD4MecanimModel model;

		public MMD4MecanimInternal.Bullet.MMDModel bulletMMDModel;

		public IntPtr mmdModelPtr;

		private Transform _modelTransform;

		private bool _physicsEnabled;

		private Transform[] _boneTransformList;

		private int[] _updateRigidBodyFlagsList;

		private float[] _updateIKWeightList;

		private float[] _updateMorphWeightList;

		private float[] _ikWeightList;

		private bool[] _ikDisabledList;

		private int[] _fullBodyIKBoneIDList;

		private Vector3[] _fullBodyIKPositionList;

		private Quaternion[] _fullBodyIKRotationList;

		private Vector3 _lossyScale = Vector3.one;

		private int[] _updateIValues;

		private float[] _updateFValues;

		private int[] _updateBoneFlagsList;

		private Matrix4x4[] _updateBoneTransformList;

		private Vector3[] _updateBonePositionList;

		private Quaternion[] _updateBoneRotationList;

		private Vector3[] _updateUserPositionList;

		private Quaternion[] _updateUserRotationList;

		private uint[] _updateUserPositionIsValidList;

		private uint[] _updateUserRotationIsValidList;

		private int[] _updateFullBodyIKFlagsList;

		private float[] _updateFullBodyIKTransformList;

		private float[] _updateFullBodyIKWeightList;

		private int[] _lateUpdateBoneFlagsList;

		private Vector3[] _lateUpdateBonePositionList;

		private Quaternion[] _lateUpdateBoneRotationList;

		private int[] _lateUpdateMeshFlagsList;

		private bool _isDirtyPreUpdate = true;

		public const int FullBodyIKTargetMax = 36;

		public int[] fullBodyIKBoneIDList
		{
			get
			{
				return _fullBodyIKBoneIDList;
			}
		}

		public Vector3[] fullBodyIKPositionList
		{
			get
			{
				return _fullBodyIKPositionList;
			}
		}

		public Quaternion[] fullBodyIKRotationList
		{
			get
			{
				return _fullBodyIKRotationList;
			}
		}

		public int[] updateFullBodyIKFlagsList
		{
			get
			{
				return _updateFullBodyIKFlagsList;
			}
		}

		public float[] updateFullBodyIKTransformList
		{
			get
			{
				return _updateFullBodyIKTransformList;
			}
		}

		public float[] updateFullBodyIKWeightList
		{
			get
			{
				return _updateFullBodyIKWeightList;
			}
		}

		public bool isExpired
		{
			get
			{
				if (bulletMMDModel != null)
				{
					return false;
				}
				if (mmdModelPtr == IntPtr.Zero)
				{
					return localWorld == null;
				}
				return false;
			}
		}

		~MMDModel()
		{
			Destroy();
		}

		private bool _Prepare(MMD4MecanimModel model)
		{
			if (model == null)
			{
				return false;
			}
			this.model = model;
			_modelTransform = model.transform;
			MMD4MecanimData.ModelData modelData = model.modelData;
			if (modelData == null || modelData.boneDataList == null || model.boneList == null || model.boneList.Length != modelData.boneDataList.Length)
			{
				UnityEngine.Debug.LogError("_Prepare: Failed.");
				return false;
			}
			if (modelData.boneDataList != null)
			{
				int num = modelData.boneDataList.Length;
				_boneTransformList = new Transform[num];
				for (int i = 0; i < num; i++)
				{
					if (model.boneList[i] != null && model.boneList[i].gameObject != null)
					{
						_boneTransformList[i] = model.boneList[i].gameObject.transform;
					}
				}
			}
			else
			{
				_boneTransformList = new Transform[0];
			}
			if (modelData.ikDataList != null)
			{
				int num2 = modelData.ikDataList.Length;
				_ikWeightList = new float[num2];
				_ikDisabledList = new bool[num2];
				_updateIKWeightList = new float[num2];
				for (int j = 0; j < num2; j++)
				{
					_ikWeightList[j] = 1f;
					_updateIKWeightList[j] = 1f;
				}
			}
			else
			{
				_ikWeightList = new float[0];
				_ikDisabledList = new bool[0];
				_updateIKWeightList = new float[0];
			}
			if (modelData.morphDataList != null)
			{
				int num3 = modelData.morphDataList.Length;
				_updateMorphWeightList = new float[num3];
			}
			else
			{
				_updateMorphWeightList = new float[0];
			}
			if (modelData.rigidBodyDataList != null)
			{
				int num4 = modelData.rigidBodyDataList.Length;
				_updateRigidBodyFlagsList = new int[num4];
				for (int k = 0; k < num4; k++)
				{
					if (modelData.rigidBodyDataList[k].isFreezed)
					{
						_updateRigidBodyFlagsList[k] |= 1;
					}
				}
			}
			else
			{
				_updateRigidBodyFlagsList = new int[0];
			}
			_physicsEnabled = model.physicsEngine != MMD4MecanimModel.PhysicsEngine.None;
			_isDirtyPreUpdate = true;
			_PrepareWork(model);
			return true;
		}

		public bool Create(MMD4MecanimModel model)
		{
			if (model == null)
			{
				return false;
			}
			byte[] modelFileBytes = model.modelFileBytes;
			if (modelFileBytes == null)
			{
				UnityEngine.Debug.LogError("");
				return false;
			}
			if (!_Prepare(model))
			{
				UnityEngine.Debug.LogError("");
				return false;
			}
			if (_modelTransform != null)
			{
				_lossyScale = _modelTransform.transform.lossyScale;
			}
			else
			{
				_lossyScale = Vector3.one;
			}
			bool flag = true;
			bool flag2 = true;
			bool flag3 = false;
			bool flag4 = true;
			float num = 0f;
			float num2 = 0f;
			MMDModelProperty mMDModelProperty = null;
			WorldProperty worldProperty = null;
			if (model.bulletPhysics != null)
			{
				mMDModelProperty = model.bulletPhysics.mmdModelProperty;
				worldProperty = model.bulletPhysics.worldProperty;
				flag = model.bulletPhysics.joinLocalWorld;
				flag2 = model.bulletPhysics.useOriginalScale;
				flag3 = model.bulletPhysics.useCustomResetTime;
				num = model.bulletPhysics.resetMorphTime;
				num2 = model.bulletPhysics.resetWaitTime;
			}
			float num3 = 0f;
			float importScale = model.importScale;
			World world = null;
			World world2 = null;
			if (importScale == 0f)
			{
				importScale = model.modelData.importScale;
			}
			if (flag)
			{
				if (worldProperty == null)
				{
					UnityEngine.Debug.LogError("localWorldProperty is null.");
					return false;
				}
				world2 = new World();
				world = world2;
				if (!world2.Create(worldProperty))
				{
					UnityEngine.Debug.LogError("");
					return false;
				}
				num3 = ((!flag2) ? worldProperty.worldScale : (model.modelData.vertexScale * importScale));
				flag4 = worldProperty.optimizeSettings;
			}
			else
			{
				if (instance != null)
				{
					world = instance.globalWorld;
				}
				if (world == null)
				{
					UnityEngine.Debug.LogError("");
					return false;
				}
				if (world.worldProperty == null)
				{
					UnityEngine.Debug.LogError("worldProperty is null.");
					return false;
				}
				num3 = world.worldProperty.worldScale;
				flag4 = world.worldProperty.optimizeSettings;
			}
			bool xdefEnabled = model.xdefEnabled;
			bool vertexMorphEnabled = model.vertexMorphEnabled;
			bool flag5 = model.blendShapesEnabled;
			byte[] array = null;
			byte[] array2 = null;
			int[] array3 = null;
			if (model.skinningEnabled)
			{
				if (vertexMorphEnabled || xdefEnabled)
				{
					bool blendShapesAnything = false;
					array3 = model._PrepareMeshFlags(out blendShapesAnything);
					flag5 = vertexMorphEnabled && flag5 && blendShapesAnything;
					if ((vertexMorphEnabled && !flag5) || xdefEnabled)
					{
						array = model.indexFileBytes;
						if (array != null)
						{
							array2 = model.vertexFileBytes;
						}
					}
				}
				else
				{
					array3 = model._PrepareMeshFlags();
				}
			}
			if (_isUseBulletXNA)
			{
				MMD4MecanimInternal.Bullet.MMDModel.ImportProperty importProperty = default(MMD4MecanimInternal.Bullet.MMDModel.ImportProperty);
				importProperty.isPhysicsEnabled = _physicsEnabled;
				importProperty.isJoinedLocalWorld = flag;
				importProperty.isVertexMorphEnabled = vertexMorphEnabled;
				importProperty.isBlendShapesEnabled = flag5;
				importProperty.isXDEFEnabled = xdefEnabled;
				importProperty.isXDEFNormalEnabled = model.xdefNormalEnabled;
				importProperty.useCustomResetTime = flag3;
				importProperty.resetMorphTime = num;
				importProperty.resetWaitTime = num2;
				importProperty.mmdModelProperty = mMDModelProperty;
				importProperty.optimizeSettings = flag4;
				if (importProperty.mmdModelProperty != null)
				{
					importProperty.mmdModelProperty.worldScale = num3;
					importProperty.mmdModelProperty.importScale = importScale;
					importProperty.mmdModelProperty.lossyScale = _lossyScale;
				}
				bulletMMDModel = new MMD4MecanimInternal.Bullet.MMDModel();
				if (!bulletMMDModel.Import(modelFileBytes, array, array2, array3, ref importProperty))
				{
					UnityEngine.Debug.LogError("");
					bulletMMDModel.Destroy();
					if (world2 != null)
					{
						world2.Destroy();
					}
					return false;
				}
				if (world != null && world.bulletPhysicsWorld != null)
				{
					world.bulletPhysicsWorld.JoinWorld(bulletMMDModel);
				}
				localWorld = world2;
				_UploadMesh(model, array3);
				return true;
			}
			MMD4MecanimCommon.PropertyWriter propertyWriter = new MMD4MecanimCommon.PropertyWriter();
			propertyWriter.Write("worldScale", num3);
			propertyWriter.Write("importScale", importScale);
			propertyWriter.Write("lossyScale", _lossyScale);
			propertyWriter.Write("optimizeSettings", flag4);
			if (flag3)
			{
				propertyWriter.Write("resetMorphTime", num);
				propertyWriter.Write("resetWaitTime", num2);
			}
			if (mMDModelProperty != null)
			{
				propertyWriter.Write("isPhysicsEnabled", _physicsEnabled);
				propertyWriter.Write("isJoinedLocalWorld", flag);
				propertyWriter.Write("isVertexMorphEnabled", vertexMorphEnabled);
				propertyWriter.Write("isBlendShapesEnabled", flag5);
				propertyWriter.Write("isXDEFEnabled", xdefEnabled);
				propertyWriter.Write("isXDEFNormalEnabled", model.xdefNormalEnabled);
				propertyWriter.Write("rigidBodyIsAdditionalDamping", mMDModelProperty.rigidBodyIsAdditionalDamping);
				propertyWriter.Write("rigidBodyIsEnableSleeping", mMDModelProperty.rigidBodyIsEnableSleeping);
				propertyWriter.Write("rigidBodyIsUseCcd", mMDModelProperty.rigidBodyIsUseCcd);
				propertyWriter.Write("rigidBodyCcdMotionThreshold", mMDModelProperty.rigidBodyCcdMotionThreshold);
				propertyWriter.Write("rigidBodyShapeScale", mMDModelProperty.rigidBodyShapeScale);
				propertyWriter.Write("rigidBodyMassRate", mMDModelProperty.rigidBodyMassRate);
				propertyWriter.Write("rigidBodyLinearDampingRate", mMDModelProperty.rigidBodyLinearDampingRate);
				propertyWriter.Write("rigidBodyAngularDampingRate", mMDModelProperty.rigidBodyAngularDampingRate);
				propertyWriter.Write("rigidBodyRestitutionRate", mMDModelProperty.rigidBodyRestitutionRate);
				propertyWriter.Write("rigidBodyFrictionRate", mMDModelProperty.rigidBodyFrictionRate);
				propertyWriter.Write("rigidBodyAntiJitterRate", mMDModelProperty.rigidBodyAntiJitterRate);
				propertyWriter.Write("rigidBodyAntiJitterRateOnKinematic", mMDModelProperty.rigidBodyAntiJitterRateOnKinematic);
				propertyWriter.Write("rigidBodyPreBoneAlignmentLimitLength", mMDModelProperty.rigidBodyPreBoneAlignmentLimitLength);
				propertyWriter.Write("rigidBodyPreBoneAlignmentLossRate", mMDModelProperty.rigidBodyPreBoneAlignmentLossRate);
				propertyWriter.Write("rigidBodyPostBoneAlignmentLimitLength", mMDModelProperty.rigidBodyPostBoneAlignmentLimitLength);
				propertyWriter.Write("rigidBodyPostBoneAlignmentLossRate", mMDModelProperty.rigidBodyPostBoneAlignmentLossRate);
				propertyWriter.Write("rigidBodyLinearDampingLossRate", mMDModelProperty.rigidBodyLinearDampingLossRate);
				propertyWriter.Write("rigidBodyAngularDampingLossRate", mMDModelProperty.rigidBodyAngularDampingLossRate);
				propertyWriter.Write("rigidBodyLinearVelocityLimit", mMDModelProperty.rigidBodyLinearVelocityLimit);
				propertyWriter.Write("rigidBodyAngularVelocityLimit", mMDModelProperty.rigidBodyAngularVelocityLimit);
				propertyWriter.Write("rigidBodyIsUseForceAngularVelocityLimit", mMDModelProperty.rigidBodyIsUseForceAngularVelocityLimit);
				propertyWriter.Write("rigidBodyIsUseForceAngularAccelerationLimit", mMDModelProperty.rigidBodyIsUseForceAngularAccelerationLimit);
				propertyWriter.Write("rigidBodyForceAngularVelocityLimit", mMDModelProperty.rigidBodyForceAngularVelocityLimit);
				propertyWriter.Write("rigidBodyIsAdditionalCollider", mMDModelProperty.rigidBodyIsAdditionalCollider);
				propertyWriter.Write("rigidBodyAdditionalColliderBias", mMDModelProperty.rigidBodyAdditionalColliderBias);
				propertyWriter.Write("rigidBodyIsForceTranslate", mMDModelProperty.rigidBodyIsForceTranslate);
				propertyWriter.Write("jointRootAdditionalLimitAngle", mMDModelProperty.jointRootAdditionalLimitAngle);
			}
			MMD4MecanimCommon.GCHValues<byte> gCHValues = MMD4MecanimCommon.MakeGCHValues(modelFileBytes);
			MMD4MecanimCommon.GCHValues<byte> gCHValues2 = MMD4MecanimCommon.MakeGCHValues(array);
			MMD4MecanimCommon.GCHValues<byte> gCHValues3 = MMD4MecanimCommon.MakeGCHValues(array2);
			MMD4MecanimCommon.GCHValues<int> gCHValues4 = MMD4MecanimCommon.MakeGCHValues(array3);
			propertyWriter.Lock();
			IntPtr intPtr = _CreateMMDModel(gCHValues, gCHValues.length, gCHValues2, gCHValues2.length, gCHValues3, gCHValues3.length, gCHValues4, gCHValues4.length, propertyWriter.iValuesPtr, propertyWriter.iValueLength, propertyWriter.fValuesPtr, propertyWriter.fValueLength);
			propertyWriter.Unlock();
			gCHValues4.Free();
			gCHValues3.Free();
			gCHValues2.Free();
			gCHValues.Free();
			if (intPtr != IntPtr.Zero)
			{
				_JoinWorldMMDModel(world.worldPtr, intPtr);
				DebugLog();
				localWorld = world2;
				mmdModelPtr = intPtr;
				_UploadMesh(model, array3);
				return true;
			}
			if (world2 != null)
			{
				world2.Destroy();
			}
			DebugLog();
			UnityEngine.Debug.LogError("");
			return false;
		}

		private void _UploadMesh(MMD4MecanimModel model, int[] meshFlags)
		{
			if (model == null || meshFlags == null || meshFlags.Length == 0)
			{
				return;
			}
			model._InitializeCloneMesh(meshFlags);
			if (bulletMMDModel != null)
			{
				for (int i = 0; i != meshFlags.Length; i++)
				{
					if ((meshFlags[i] & 6) == 0)
					{
						continue;
					}
					MMD4MecanimModel.CloneMesh cloneMesh = model._GetCloneMesh(i);
					if (cloneMesh == null)
					{
						continue;
					}
					Vector3[] vertices = cloneMesh.vertices;
					Vector3[] normals = null;
					BoneWeight[] boneWeights = null;
					Matrix4x4[] bindposes = null;
					if (((uint)meshFlags[i] & 4u) != 0)
					{
						if (model.xdefNormalEnabled)
						{
							normals = cloneMesh.normals;
						}
						boneWeights = cloneMesh.boneWeights;
						bindposes = cloneMesh.bindposes;
					}
					bulletMMDModel.UploadMesh(i, vertices, normals, boneWeights, bindposes);
				}
			}
			if (mmdModelPtr != IntPtr.Zero)
			{
				for (int j = 0; j != meshFlags.Length; j++)
				{
					if ((meshFlags[j] & 6) == 0)
					{
						continue;
					}
					MMD4MecanimModel.CloneMesh cloneMesh2 = model._GetCloneMesh(j);
					if (cloneMesh2 == null)
					{
						continue;
					}
					Vector3[] vertices2 = cloneMesh2.vertices;
					Vector3[] values = null;
					BoneWeight[] values2 = null;
					Matrix4x4[] values3 = null;
					if (((uint)meshFlags[j] & 4u) != 0)
					{
						if (model.xdefNormalEnabled)
						{
							values = cloneMesh2.normals;
						}
						values2 = cloneMesh2.boneWeights;
						values3 = cloneMesh2.bindposes;
					}
					MMD4MecanimCommon.GCHValues<Vector3> gCHValues = MMD4MecanimCommon.MakeGCHValues(vertices2);
					MMD4MecanimCommon.GCHValues<Vector3> gCHValues2 = MMD4MecanimCommon.MakeGCHValues(values);
					MMD4MecanimCommon.GCHValues<BoneWeight> gCHValues3 = MMD4MecanimCommon.MakeGCHValues(values2);
					MMD4MecanimCommon.GCHValues<Matrix4x4> gCHValues4 = MMD4MecanimCommon.MakeGCHValues(values3);
					_UploadMeshMMDModel(mmdModelPtr, j, gCHValues, gCHValues2, gCHValues3, gCHValues.length, gCHValues4, gCHValues4.length);
					gCHValues3.Free();
					gCHValues2.Free();
					gCHValues.Free();
				}
				DebugLog();
			}
			model._CleanupCloneMesh();
		}

		private void _PrepareWork(MMD4MecanimModel model)
		{
			int num = 0;
			if (_boneTransformList != null)
			{
				num = _boneTransformList.Length;
			}
			_fullBodyIKBoneIDList = new int[36];
			_fullBodyIKPositionList = new Vector3[36];
			_fullBodyIKRotationList = new Quaternion[36];
			for (int i = 0; i != 36; i++)
			{
				_fullBodyIKBoneIDList[i] = -1;
				_fullBodyIKPositionList[i] = Vector3.zero;
				_fullBodyIKRotationList[i] = Quaternion.identity;
			}
			_updateIValues = new int[1];
			_updateFValues = new float[2];
			_updateBoneFlagsList = new int[num];
			_updateBoneTransformList = new Matrix4x4[num];
			_updateBonePositionList = new Vector3[num];
			_updateBoneRotationList = new Quaternion[num];
			_updateUserPositionList = new Vector3[num];
			_updateUserRotationList = new Quaternion[num];
			_updateUserPositionIsValidList = new uint[(num + 31) / 32];
			_updateUserRotationIsValidList = new uint[(num + 31) / 32];
			for (int j = 0; j != num; j++)
			{
				_updateUserPositionList[j] = Vector3.zero;
				_updateUserRotationList[j] = Quaternion.identity;
			}
			_updateFullBodyIKFlagsList = new int[36];
			_updateFullBodyIKTransformList = new float[144];
			_updateFullBodyIKWeightList = new float[36];
			for (int k = 0; k != 36; k++)
			{
				_updateFullBodyIKWeightList[k] = 1f;
			}
		}

		public void Destroy()
		{
			if (bulletMMDModel != null)
			{
				bulletMMDModel.Destroy();
				bulletMMDModel = null;
			}
			if (mmdModelPtr != IntPtr.Zero)
			{
				IntPtr intPtr = mmdModelPtr;
				mmdModelPtr = IntPtr.Zero;
				_DestroyMMDModel(intPtr);
			}
			if (localWorld != null)
			{
				localWorld.Destroy();
				localWorld = null;
			}
			_modelTransform = null;
			model = null;
		}

		public void SetRigidBodyFreezed(int rigidBodyID, bool isFreezed)
		{
			if (_updateRigidBodyFlagsList != null && (uint)rigidBodyID < (uint)_updateRigidBodyFlagsList.Length)
			{
				int num = _updateRigidBodyFlagsList[rigidBodyID];
				bool flag = (num & 1) != 0;
				if (isFreezed != flag)
				{
					_isDirtyPreUpdate = true;
					num = ((!isFreezed) ? (num & -2) : (num | 1));
					_updateRigidBodyFlagsList[rigidBodyID] = num;
				}
			}
		}

		public void SetIKProperty(int ikID, bool isEnabled, float ikWeight)
		{
			if (_ikWeightList != null && _ikDisabledList != null && _updateIKWeightList != null && (uint)ikID < (uint)_ikWeightList.Length && (_ikDisabledList[ikID] != !isEnabled || _ikWeightList[ikID] != ikWeight))
			{
				_isDirtyPreUpdate = true;
				_ikDisabledList[ikID] = !isEnabled;
				_ikWeightList[ikID] = ikWeight;
				if (isEnabled)
				{
					_updateIKWeightList[ikID] = ikWeight;
				}
				else
				{
					_updateIKWeightList[ikID] = 0f;
				}
			}
		}

		private static void _Swap<Type>(ref Type[] lhs, ref Type[] rhs)
		{
			Type[] array = lhs;
			lhs = rhs;
			rhs = array;
		}

		public void Update()
		{
			if (_boneTransformList == null || _updateIValues == null || _updateFValues == null || _updateBoneFlagsList == null || _updateBoneTransformList == null || _updateBonePositionList == null || _updateBoneRotationList == null || _updateUserPositionList == null || _updateUserRotationList == null || _updateRigidBodyFlagsList == null || _updateIKWeightList == null || _updateMorphWeightList == null || _modelTransform == null)
			{
				return;
			}
			if (model != null)
			{
				if (localWorld != null)
				{
					localWorld.SetGravity(model.bulletPhysics.worldProperty.gravityScale, model.bulletPhysics.worldProperty.gravityNoise, model.bulletPhysics.worldProperty.gravityDirection);
				}
				if (model.ikList != null)
				{
					for (int i = 0; i != model.ikList.Length; i++)
					{
						if (model.ikList[i] != null)
						{
							SetIKProperty(i, model.ikList[i].ikEnabled, model.ikList[i].ikWeight);
						}
					}
				}
				if (model.morphList != null && model.morphList.Length == _updateMorphWeightList.Length)
				{
					for (int j = 0; j != model.morphList.Length; j++)
					{
						_updateMorphWeightList[j] = model.morphList[j]._updatedWeight;
					}
				}
				if (model.rigidBodyList != null)
				{
					for (int k = 0; k != model.rigidBodyList.Length; k++)
					{
						if (model.rigidBodyList[k] != null)
						{
							SetRigidBodyFreezed(k, model.rigidBodyList[k].freezed);
						}
					}
				}
			}
			int num = _boneTransformList.Length;
			int num2 = 0;
			num2 |= (model.ikEnabled ? 1 : 0);
			num2 |= (model.boneInherenceEnabled ? 2 : 0);
			num2 |= (model.boneMorphEnabled ? 4 : 0);
			num2 |= (model.fullBodyIKEnabled ? 16 : 0);
			if (model != null)
			{
				float pphShoulderFixRateImmediately = model.pphShoulderFixRateImmediately;
				if (pphShoulderFixRateImmediately > 0f)
				{
					num2 |= 8;
					if (_updateFValues[1] != pphShoulderFixRateImmediately)
					{
						_updateFValues[1] = pphShoulderFixRateImmediately;
						_isDirtyPreUpdate = true;
					}
				}
				MMD4MecanimBone[] boneList = model.boneList;
				if (boneList != null)
				{
					for (int l = 0; l != num; l++)
					{
						MMD4MecanimBone mMD4MecanimBone = boneList[l];
						if (!(mMD4MecanimBone != null))
						{
							continue;
						}
						bool flag = ((_updateUserPositionIsValidList[l >> 5] >> l) & 1) != 0;
						bool flag2 = ((_updateUserRotationIsValidList[l >> 5] >> l) & 1) != 0;
						if (!mMD4MecanimBone._userPositionIsZero != flag)
						{
							_isDirtyPreUpdate = true;
							if (mMD4MecanimBone._userPositionIsZero)
							{
								_updateUserPositionIsValidList[l >> 5] &= (uint)(~(1 << l));
								_updateUserPositionList[l] = Vector3.zero;
							}
							else
							{
								_updateUserPositionIsValidList[l >> 5] |= (uint)(1 << l);
								_updateUserPositionList[l] = mMD4MecanimBone._userPosition;
							}
						}
						else if (flag)
						{
							_updateUserPositionList[l] = mMD4MecanimBone._userPosition;
						}
						if (!mMD4MecanimBone._userRotationIsIdentity != flag2)
						{
							_isDirtyPreUpdate = true;
							if (mMD4MecanimBone._userRotationIsIdentity)
							{
								_updateUserRotationIsValidList[l >> 5] &= (uint)(~(1 << l));
								_updateUserRotationList[l] = Quaternion.identity;
							}
							else
							{
								_updateUserRotationIsValidList[l >> 5] |= (uint)(1 << l);
								_updateUserRotationList[l] = mMD4MecanimBone._userRotation;
							}
						}
						else if (flag2)
						{
							_updateUserRotationList[l] = mMD4MecanimBone._userRotation;
						}
					}
				}
			}
			if (_isDirtyPreUpdate)
			{
				for (int m = 0; m != num; m++)
				{
					_updateBoneFlagsList[m] = 0;
				}
				if (model != null)
				{
					MMD4MecanimBone[] boneList2 = model.boneList;
					if (boneList2 != null && boneList2.Length == num)
					{
						for (int n = 0; n != num; n++)
						{
							if (boneList2[n] != null)
							{
								switch (boneList2[n].humanBodyBones)
								{
								case 11:
									_updateBoneFlagsList[n] |= 16777216;
									break;
								case 13:
									_updateBoneFlagsList[n] |= 33554432;
									break;
								case 12:
									_updateBoneFlagsList[n] |= 50331648;
									break;
								case 14:
									_updateBoneFlagsList[n] |= 67108864;
									break;
								}
							}
							bool num3 = ((_updateUserPositionIsValidList[n >> 5] >> n) & 1) != 0;
							bool flag3 = ((_updateUserRotationIsValidList[n >> 5] >> n) & 1) != 0;
							if (num3)
							{
								_updateBoneFlagsList[n] |= 128;
							}
							if (flag3)
							{
								_updateBoneFlagsList[n] |= 256;
							}
						}
					}
				}
				bool flag4 = false;
				if (bulletMMDModel != null)
				{
					flag4 = true;
					bulletMMDModel.PreUpdate((uint)num2, _updateBoneFlagsList, _updateRigidBodyFlagsList, _updateIKWeightList, _updateMorphWeightList);
				}
				if (!flag4 && mmdModelPtr != IntPtr.Zero)
				{
					MMD4MecanimCommon.GCHValues<int> gCHValues = MMD4MecanimCommon.MakeGCHValues(_updateBoneFlagsList);
					MMD4MecanimCommon.GCHValues<int> gCHValues2 = MMD4MecanimCommon.MakeGCHValues(_updateRigidBodyFlagsList);
					MMD4MecanimCommon.GCHValues<float> gCHValues3 = MMD4MecanimCommon.MakeGCHValues(_updateIKWeightList);
					MMD4MecanimCommon.GCHValues<float> gCHValues4 = MMD4MecanimCommon.MakeGCHValues(_updateMorphWeightList);
					MMD4MecanimCommon.GCHValues<int> gCHValues5 = MMD4MecanimCommon.MakeGCHValues(_updateFullBodyIKFlagsList);
					MMD4MecanimCommon.GCHValues<float> gCHValues6 = MMD4MecanimCommon.MakeGCHValues(_updateFullBodyIKWeightList);
					_PreUpdateMMDModel(mmdModelPtr, num2, gCHValues, gCHValues.length, gCHValues2, gCHValues2.length, gCHValues3, gCHValues3.length, gCHValues4, gCHValues4.length, gCHValues5, gCHValues6, gCHValues6.length);
					gCHValues6.Free();
					gCHValues5.Free();
					gCHValues4.Free();
					gCHValues3.Free();
					gCHValues2.Free();
					gCHValues.Free();
				}
			}
			for (int num4 = 0; num4 != num; num4++)
			{
				Transform transform = _boneTransformList[num4];
				if (transform != null)
				{
					if (((uint)_updateBoneFlagsList[num4] & (true ? 1u : 0u)) != 0)
					{
						_updateBoneTransformList[num4] = transform.localToWorldMatrix;
					}
					for (int num5 = 0; num5 < 16; num5++)
					{
						_updateBoneTransformList[num4][num5] /= model.transform.localScale.x;
					}
					if (((uint)_updateBoneFlagsList[num4] & 2u) != 0)
					{
						_updateBonePositionList[num4] = transform.localPosition;
					}
					if (((uint)_updateBoneFlagsList[num4] & 4u) != 0)
					{
						_updateBoneRotationList[num4] = transform.localRotation;
					}
				}
			}
			Matrix4x4 modelTransform = _modelTransform.localToWorldMatrix;
			if (bulletMMDModel != null)
			{
				bulletMMDModel.Update((uint)num2, _updateIValues, _updateFValues, ref modelTransform, _updateBoneFlagsList, _updateBoneTransformList, _updateBonePositionList, _updateBoneRotationList, _updateUserPositionList, _updateUserRotationList, _updateRigidBodyFlagsList, _updateIKWeightList, _updateMorphWeightList);
			}
			else if (mmdModelPtr != IntPtr.Zero)
			{
				MMD4MecanimCommon.GCHValues<int> gCHValues7 = MMD4MecanimCommon.MakeGCHValues(_updateIValues);
				MMD4MecanimCommon.GCHValues<float> gCHValues8 = MMD4MecanimCommon.MakeGCHValues(_updateFValues);
				MMD4MecanimCommon.GCHValue<Matrix4x4> gCHValue = MMD4MecanimCommon.MakeGCHValue(ref modelTransform);
				MMD4MecanimCommon.GCHValues<int> gCHValues9 = MMD4MecanimCommon.MakeGCHValues(_updateBoneFlagsList);
				MMD4MecanimCommon.GCHValues<Matrix4x4> gCHValues10 = MMD4MecanimCommon.MakeGCHValues(_updateBoneTransformList);
				MMD4MecanimCommon.GCHValues<Vector3> gCHValues11 = MMD4MecanimCommon.MakeGCHValues(_updateBonePositionList);
				MMD4MecanimCommon.GCHValues<Quaternion> gCHValues12 = MMD4MecanimCommon.MakeGCHValues(_updateBoneRotationList);
				MMD4MecanimCommon.GCHValues<Vector3> gCHValues13 = MMD4MecanimCommon.MakeGCHValues(_updateUserPositionList);
				MMD4MecanimCommon.GCHValues<Quaternion> gCHValues14 = MMD4MecanimCommon.MakeGCHValues(_updateUserRotationList);
				MMD4MecanimCommon.GCHValues<int> gCHValues15 = MMD4MecanimCommon.MakeGCHValues(_updateRigidBodyFlagsList);
				MMD4MecanimCommon.GCHValues<float> gCHValues16 = MMD4MecanimCommon.MakeGCHValues(_updateIKWeightList);
				MMD4MecanimCommon.GCHValues<float> gCHValues17 = MMD4MecanimCommon.MakeGCHValues(_updateMorphWeightList);
				MMD4MecanimCommon.GCHValues<int> gCHValues18 = MMD4MecanimCommon.MakeGCHValues(_updateFullBodyIKFlagsList);
				MMD4MecanimCommon.GCHValues<float> gCHValues19 = MMD4MecanimCommon.MakeGCHValues(_updateFullBodyIKTransformList);
				MMD4MecanimCommon.GCHValues<float> gCHValues20 = MMD4MecanimCommon.MakeGCHValues(_updateFullBodyIKWeightList);
				_UpdateMMDModel(mmdModelPtr, num2, gCHValues7, gCHValues7.length, gCHValues8, gCHValues8.length, gCHValue, gCHValues9, gCHValues10, gCHValues11, gCHValues12, gCHValues13, gCHValues14, gCHValues9.length, gCHValues15, gCHValues15.length, gCHValues16, gCHValues16.length, gCHValues17, gCHValues17.length, gCHValues18, gCHValues19, gCHValues20, gCHValues20.length);
				gCHValues20.Free();
				gCHValues19.Free();
				gCHValues18.Free();
				gCHValues17.Free();
				gCHValues16.Free();
				gCHValues15.Free();
				gCHValues14.Free();
				gCHValues13.Free();
				gCHValues12.Free();
				gCHValues11.Free();
				gCHValues10.Free();
				gCHValues9.Free();
				gCHValue.Free();
				gCHValues8.Free();
				gCHValues7.Free();
			}
		}

		public void LateUpdate(float deltaTime)
		{
			if (_boneTransformList == null || _updateBoneTransformList == null || _updateBoneFlagsList == null || _updateBonePositionList == null || _updateBoneRotationList == null)
			{
				return;
			}
			if (localWorld != null)
			{
				localWorld.Update(deltaTime);
			}
			int num = 0;
			int[] array = null;
			Vector3[] array2 = null;
			Quaternion[] array3 = null;
			int[] array4 = null;
			MMD4MecanimInternal.Bullet.MMDModel.UpdateData updateData = null;
			if (bulletMMDModel != null)
			{
				updateData = bulletMMDModel.LateUpdate();
				if (updateData != null)
				{
					num = (int)updateData.lateUpdateFlags;
					array = updateData.lateUpdateBoneFlagsList;
					array2 = updateData.lateUpdateBonePositionList;
					array3 = updateData.lateUpdateBoneRotationList;
					array4 = updateData.lateUpdateMeshFlagsList;
				}
			}
			if (mmdModelPtr != IntPtr.Zero)
			{
				if (_lateUpdateBoneFlagsList == null || _lateUpdateBonePositionList == null || _lateUpdateBoneRotationList == null)
				{
					int num2 = ((_boneTransformList != null) ? _boneTransformList.Length : 0);
					_lateUpdateBoneFlagsList = new int[num2];
					_lateUpdateBonePositionList = new Vector3[num2];
					_lateUpdateBoneRotationList = new Quaternion[num2];
				}
				if (_lateUpdateMeshFlagsList == null)
				{
					int num3 = ((model != null) ? model._skinnedMeshCount : 0);
					_lateUpdateMeshFlagsList = new int[num3];
				}
				MMD4MecanimCommon.GCHValues<int> gCHValues = MMD4MecanimCommon.MakeGCHValues(_lateUpdateBoneFlagsList);
				MMD4MecanimCommon.GCHValues<Vector3> gCHValues2 = MMD4MecanimCommon.MakeGCHValues(_lateUpdateBonePositionList);
				MMD4MecanimCommon.GCHValues<Quaternion> gCHValues3 = MMD4MecanimCommon.MakeGCHValues(_lateUpdateBoneRotationList);
				MMD4MecanimCommon.GCHValues<int> gCHValues4 = MMD4MecanimCommon.MakeGCHValues(_lateUpdateMeshFlagsList);
				num = _LateUpdateMMDModel(mmdModelPtr, IntPtr.Zero, 0, IntPtr.Zero, 0, gCHValues, gCHValues2, gCHValues3, _lateUpdateBoneFlagsList.Length, gCHValues4, gCHValues4.length, IntPtr.Zero, 0);
				gCHValues4.Free();
				gCHValues3.Free();
				gCHValues2.Free();
				gCHValues.Free();
				array = _lateUpdateBoneFlagsList;
				array2 = _lateUpdateBonePositionList;
				array3 = _lateUpdateBoneRotationList;
				array4 = _lateUpdateMeshFlagsList;
			}
			if (((uint)num & (true ? 1u : 0u)) != 0 && array != null && array2 != null && array3 != null)
			{
				int num4 = _boneTransformList.Length;
				for (int i = 0; i != num4; i++)
				{
					Transform transform = _boneTransformList[i];
					if (transform != null && ((uint)array[i] & (true ? 1u : 0u)) != 0)
					{
						if (((uint)array[i] & 2u) != 0)
						{
							transform.localPosition = array2[i];
						}
						if (((uint)array[i] & 4u) != 0)
						{
							transform.localRotation = array3[i];
						}
					}
				}
			}
			if ((num & 2) == 0 || array4 == null)
			{
				return;
			}
			if (bulletMMDModel != null && updateData != null)
			{
				MMD4MecanimInternal.Bullet.MMDModel.LateUpdateMeshData[] lateUpdateMeshDataList = updateData.lateUpdateMeshDataList;
				int cloneMeshCount = model._cloneMeshCount;
				if (lateUpdateMeshDataList == null || lateUpdateMeshDataList.Length != cloneMeshCount || array4.Length != cloneMeshCount)
				{
					return;
				}
				for (int j = 0; j != cloneMeshCount; j++)
				{
					int num5 = array4[j];
					if ((num5 & 3) == 0)
					{
						continue;
					}
					Vector3[] vertices = null;
					Vector3[] normals = null;
					if (lateUpdateMeshDataList[j] != null)
					{
						if (((uint)num5 & (true ? 1u : 0u)) != 0)
						{
							vertices = lateUpdateMeshDataList[j].vertices;
						}
						if (((uint)num5 & 2u) != 0)
						{
							normals = lateUpdateMeshDataList[j].normals;
						}
						model._UploadMeshVertex(j, vertices, normals);
					}
				}
			}
			else
			{
				if (!(mmdModelPtr != IntPtr.Zero))
				{
					return;
				}
				int cloneMeshCount2 = model._cloneMeshCount;
				for (int k = 0; k != cloneMeshCount2; k++)
				{
					int num6 = array4[k];
					if ((num6 & 3) == 0)
					{
						continue;
					}
					MMD4MecanimModel.CloneMesh cloneMesh = model._GetCloneMesh(k);
					if (cloneMesh != null)
					{
						Vector3[] array5 = null;
						Vector3[] array6 = null;
						if (((uint)num6 & (true ? 1u : 0u)) != 0)
						{
							array5 = cloneMesh.vertices;
						}
						if (((uint)num6 & 2u) != 0)
						{
							array6 = cloneMesh.normals;
						}
						MMD4MecanimCommon.GCHValues<Vector3> gCHValues5 = MMD4MecanimCommon.MakeGCHValues(array5);
						MMD4MecanimCommon.GCHValues<Vector3> gCHValues6 = MMD4MecanimCommon.MakeGCHValues(array6);
						int num7 = _LateUpdateMeshMMDModel(mmdModelPtr, k, gCHValues5, gCHValues6, gCHValues5.length);
						gCHValues6.Free();
						gCHValues5.Free();
						if (num7 != 0)
						{
							model._UploadMeshVertex(k, array5, array6);
						}
					}
				}
			}
		}
	}

	public static readonly Matrix4x4 rotateMatrixX = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);

	public static readonly Matrix4x4 rotateMatrixXInv = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f), Vector3.one);

	public static readonly Matrix4x4 rotateMatrixZ = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(90f, 0f, 0f), Vector3.one);

	public static readonly Matrix4x4 rotateMatrixZInv = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-90f, 0f, 0f), Vector3.one);

	public static readonly Quaternion rotateQuaternionX = Quaternion.Euler(0f, 0f, 90f);

	public static readonly Quaternion rotateQuaternionZ = Quaternion.Euler(90f, 0f, 0f);

	public static readonly Quaternion rotateQuaternionXInv = Quaternion.Euler(0f, 0f, -90f);

	public static readonly Quaternion rotateQuaternionZInv = Quaternion.Euler(-90f, 0f, 0f);

	public WorldProperty globalWorldProperty;

	private List<MMDModel> _mmdModelList = new List<MMDModel>();

	private List<RigidBody> _rigidBodyList = new List<RigidBody>();

	private bool _isAwaked;

	private World _globalWorld;

	private static MMD4MecanimBulletPhysics _instance;

	private bool _initialized;

	private static bool _isUseBulletXNA;

	private Process _process;

	private bool ignoreBulletPhysics = true;

	public const string DllName = "MMD4MecanimBulletPhysics";

	public World globalWorld
	{
		get
		{
			_ActivateGlobalWorld();
			return _globalWorld;
		}
	}

	public static bool isUseBulletXNA
	{
		get
		{
			return _isUseBulletXNA;
		}
	}

	public static MMD4MecanimBulletPhysics instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (MMD4MecanimBulletPhysics)UnityEngine.Object.FindObjectOfType(typeof(MMD4MecanimBulletPhysics));
				if (_instance == null)
				{
					MMD4MecanimBulletPhysics mMD4MecanimBulletPhysics = new GameObject("MMD4MecanimBulletPhysics").AddComponent<MMD4MecanimBulletPhysics>();
					if (_instance == null)
					{
						_instance = mMD4MecanimBulletPhysics;
					}
				}
				if (_instance != null)
				{
					_instance._Initialize();
				}
			}
			return _instance;
		}
	}

	private void _Initialize()
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		_isUseBulletXNA = false;
		if (_isUseBulletXNA)
		{
			if (Global.bridge == null)
			{
				Global.bridge = new MMD4MecanimBulletBridge();
			}
			UnityEngine.Debug.Log("MMD4MecanimBulletPhysics:Awake BulletXNA.");
		}
		else
		{
			UnityEngine.Debug.Log("MMD4MecanimBulletPhysics:Awake Native Plugin.");
			_InitializeEngine();
		}
		StartCoroutine(DelayedAwake());
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else if (_instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		_Initialize();
	}

	private void LateUpdate()
	{
		if (!ignoreBulletPhysics)
		{
			_InternalUpdate();
		}
	}

	private void _InternalUpdate()
	{
		if (_isAwaked)
		{
			for (int i = 0; i != _rigidBodyList.Count; i++)
			{
				_rigidBodyList[i].Update();
			}
			for (int j = 0; j != _mmdModelList.Count; j++)
			{
				_mmdModelList[j].Update();
			}
			World world = globalWorld;
			if (world != null)
			{
				world.Update(Time.deltaTime);
			}
			for (int k = 0; k != _rigidBodyList.Count; k++)
			{
				_rigidBodyList[k].LateUpdate();
			}
			for (int l = 0; l != _mmdModelList.Count; l++)
			{
				_mmdModelList[l].LateUpdate(Time.deltaTime);
			}
			DebugLog();
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i != _rigidBodyList.Count; i++)
		{
			_rigidBodyList[i].Destroy();
		}
		for (int j = 0; j != _mmdModelList.Count; j++)
		{
			_mmdModelList[j].Destroy();
		}
		_rigidBodyList.Clear();
		_mmdModelList.Clear();
		if (_globalWorld != null)
		{
			_globalWorld.Destroy();
			_globalWorld = null;
		}
		if (_instance == this)
		{
			_instance = null;
			if (!_isUseBulletXNA)
			{
				_FinalizeEngine();
			}
		}
	}

	private IEnumerator DelayedAwake()
	{
		yield return new WaitForEndOfFrame();
		_isAwaked = true;
	}

	private void _ActivateGlobalWorld()
	{
		if (_globalWorld == null)
		{
			_globalWorld = new World();
		}
		if (globalWorldProperty == null)
		{
			globalWorldProperty = new WorldProperty();
		}
		if (_globalWorld.isExpired)
		{
			_globalWorld.Create(globalWorldProperty);
		}
	}

	public static void DebugLog()
	{
		if (!_isUseBulletXNA)
		{
			IntPtr intPtr = _DebugLog(1);
			if (intPtr != IntPtr.Zero)
			{
				UnityEngine.Debug.Log(Marshal.PtrToStringUni(intPtr));
			}
		}
	}

	public MMDModel CreateMMDModel(MMD4MecanimModel model)
	{
		MMDModel mMDModel = new MMDModel();
		if (!mMDModel.Create(model))
		{
			UnityEngine.Debug.LogError("CreateMMDModel: Failed " + model.gameObject.name);
			return null;
		}
		_mmdModelList.Add(mMDModel);
		return mMDModel;
	}

	public void DestroyMMDModel(MMDModel mmdModel)
	{
		for (int i = 0; i < _mmdModelList.Count; i++)
		{
			if (_mmdModelList[i] == mmdModel)
			{
				mmdModel.Destroy();
				_mmdModelList.Remove(mmdModel);
				break;
			}
		}
	}

	public RigidBody CreateRigidBody(MMD4MecanimRigidBody rigidBody)
	{
		RigidBody rigidBody2 = new RigidBody();
		if (!rigidBody2.Create(rigidBody))
		{
			return null;
		}
		_rigidBodyList.Add(rigidBody2);
		return rigidBody2;
	}

	public void DestroyRigidBody(RigidBody rigidBody)
	{
		for (int i = 0; i < _rigidBodyList.Count; i++)
		{
			if (_rigidBodyList[i] == rigidBody)
			{
				rigidBody.Destroy();
				_rigidBodyList.Remove(rigidBody);
				break;
			}
		}
	}

	private static void _InitializeEngine()
	{
		MMD4MecanimBulletPhysicsInitialize();
	}

	private static void _FinalizeEngine()
	{
		MMD4MecanimBulletPhysicsFinalize();
	}

	private static IntPtr _DebugLog(int clanupFlag)
	{
		return MMD4MecanimBulletPhysicsDebugLog(clanupFlag);
	}

	private static IntPtr _CreateWorld(IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		return MMD4MecanimBulletPhysicsCreateWorld(iValues, iValueLength, fValues, fValueLength);
	}

	private static void _ConfigWorld(IntPtr worldPtr, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		MMD4MecanimBulletPhysicsConfigWorld(worldPtr, iValues, iValueLength, fValues, fValueLength);
	}

	private static void _DestroyWorld(IntPtr worldPtr)
	{
		MMD4MecanimBulletPhysicsDestroyWorld(worldPtr);
	}

	private static void _UpdateWorld(IntPtr worldPtr, float deltaTime, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		MMD4MecanimBulletPhysicsUpdateWorld(worldPtr, deltaTime, iValues, iValueLength, fValues, fValueLength);
	}

	private static IntPtr _CreateRigidBody(IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		return MMD4MecanimBulletPhysicsCreateRigidBody(iValues, iValueLength, fValues, fValueLength);
	}

	private static void _ConfigRigidBody(IntPtr rigidBodyPtr, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		MMD4MecanimBulletPhysicsConfigRigidBody(rigidBodyPtr, iValues, iValueLength, fValues, fValueLength);
	}

	private static void _DestroyRigidBody(IntPtr rigidBodyPtr)
	{
		MMD4MecanimBulletPhysicsDestroyRigidBody(rigidBodyPtr);
	}

	private static void _JoinWorldRigidBody(IntPtr worldPtr, IntPtr rigidBodyPtr)
	{
		MMD4MecanimBulletPhysicsJoinWorldRigidBody(worldPtr, rigidBodyPtr);
	}

	private static void _LeaveWorldRigidBody(IntPtr rigidBodyPtr)
	{
		MMD4MecanimBulletPhysicsLeaveWorldRigidBody(rigidBodyPtr);
	}

	private static void _ResetWorldRigidBody(IntPtr rigidBodyPtr)
	{
		MMD4MecanimBulletPhysicsResetWorldRigidBody(rigidBodyPtr);
	}

	private static void _UpdateRigidBody(IntPtr rigidBodyPtr, int updateFlags, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		MMD4MecanimBulletPhysicsUpdateRigidBody(rigidBodyPtr, updateFlags, iValues, iValueLength, fValues, fValueLength);
	}

	private static int _LateUpdateRigidBody(IntPtr rigidBodyPtr, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		return MMD4MecanimBulletPhysicsLateUpdateRigidBody(rigidBodyPtr, iValues, iValueLength, fValues, fValueLength);
	}

	private static IntPtr _CreateMMDModel(IntPtr mmdModelBytes, int mmdModelLength, IntPtr mmdIndexBytes, int mmdIndexLength, IntPtr mmdVertexBytes, int mmdVertexLength, IntPtr iMeshValues, int meshLength, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		return MMD4MecanimBulletPhysicsCreateMMDModel(mmdModelBytes, mmdModelLength, mmdIndexBytes, mmdIndexLength, mmdVertexBytes, mmdVertexLength, iMeshValues, meshLength, iValues, iValueLength, fValues, fValueLength);
	}

	private static int _GetFullBodyIKDataMMDModel(IntPtr mmdModelPtr, IntPtr fullBodyIKBoneID, IntPtr fullBodyIKPositions, IntPtr fullBodyIKRotations, int fullBodyIKLength)
	{
		return MMD4MecanimBulletPhysicsGetFullBodyIKDataMMDModel(mmdModelPtr, fullBodyIKBoneID, fullBodyIKPositions, fullBodyIKRotations, fullBodyIKLength);
	}

	private static int _UploadMeshMMDModel(IntPtr mmdModelPtr, int meshID, IntPtr vertices, IntPtr normals, IntPtr boneWeights, int vertexLength, IntPtr bindposes, int boneLength)
	{
		return MMD4MecanimBulletPhysicsUploadMeshMMDModel(mmdModelPtr, meshID, vertices, normals, boneWeights, vertexLength, bindposes, boneLength);
	}

	private static void _ConfigMMDModel(IntPtr mmdModelPtr, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		MMD4MecanimBulletPhysicsConfigMMDModel(mmdModelPtr, iValues, iValueLength, fValues, fValueLength);
	}

	private static void _DestroyMMDModel(IntPtr mmdModelPtr)
	{
		MMD4MecanimBulletPhysicsDestroyMMDModel(mmdModelPtr);
	}

	private static void _JoinWorldMMDModel(IntPtr worldPtr, IntPtr mmdModelPtr)
	{
		MMD4MecanimBulletPhysicsJoinWorldMMDModel(worldPtr, mmdModelPtr);
	}

	private static void _LeaveWorldMMDModel(IntPtr mmdModelPtr)
	{
		MMD4MecanimBulletPhysicsLeaveWorldMMDModel(mmdModelPtr);
	}

	private static void _ResetWorldMMDModel(IntPtr mmdModelPtr)
	{
		MMD4MecanimBulletPhysicsResetWorldMMDModel(mmdModelPtr);
	}

	private static int _PreUpdateMMDModel(IntPtr mmdModelPtr, int updateFlags, IntPtr iBoneValues, int boneLength, IntPtr iRigidBodyValues, int rigidBodyLength, IntPtr ikWeights, int ikLength, IntPtr morphWeights, int morphLength, IntPtr iFullBodyIKValues, IntPtr fullBodyIKWeights, int fullBodyIKLength)
	{
		return MMD4MecanimBulletPhysicsPreUpdateMMDModel(mmdModelPtr, updateFlags, iBoneValues, boneLength, iRigidBodyValues, rigidBodyLength, ikWeights, ikLength, morphWeights, morphLength, iFullBodyIKValues, fullBodyIKWeights, fullBodyIKLength);
	}

	private static void _UpdateMMDModel(IntPtr mmdModelPtr, int updateFlags, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength, IntPtr modelTransform, IntPtr iBoneValues, IntPtr boneTransforms, IntPtr boneLocalPositions, IntPtr boneLocalRotations, IntPtr boneUserPositions, IntPtr boneUserRotations, int boneLength, IntPtr iRigidBodyValues, int rigidBodyLength, IntPtr ikWeights, int ikLength, IntPtr morphWeights, int morphLength, IntPtr iFullBodyIKValues, IntPtr fullBodyIKTransforms, IntPtr fullBodyIKWeights, int fullBodyIKLength)
	{
		MMD4MecanimBulletPhysicsUpdateMMDModel(mmdModelPtr, updateFlags, iValues, iValueLength, fValues, fValueLength, modelTransform, iBoneValues, boneTransforms, boneLocalPositions, boneLocalRotations, boneUserPositions, boneUserRotations, boneLength, iRigidBodyValues, rigidBodyLength, ikWeights, ikLength, morphWeights, morphLength, iFullBodyIKValues, fullBodyIKTransforms, fullBodyIKWeights, fullBodyIKLength);
	}

	private static int _LateUpdateMMDModel(IntPtr mmdModelPtr, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength, IntPtr iBoneValues, IntPtr bonePositions, IntPtr boneRotations, int boneLength, IntPtr iMeshValues, int meshLength, IntPtr morphWeigts, int morphLength)
	{
		return MMD4MecanimBulletPhysicsLateUpdateMMDModel(mmdModelPtr, iValues, iValueLength, fValues, fValueLength, iBoneValues, bonePositions, boneRotations, boneLength, iMeshValues, meshLength, morphWeigts, morphLength);
	}

	private static int _LateUpdateMeshMMDModel(IntPtr mmdModelPtr, int meshID, IntPtr vertices, IntPtr normals, int vertexLength)
	{
		return MMD4MecanimBulletPhysicsLateUpdateMeshMMDModel(mmdModelPtr, meshID, vertices, normals, vertexLength);
	}

	private static void _ConfigBoneMMDModel(IntPtr mmdModelPtr, int boneID, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		MMD4MecanimBulletPhysicsConfigBoneMMDModel(mmdModelPtr, boneID, iValues, iValueLength, fValues, fValueLength);
	}

	private static void _ConfigRigidBodyMMDModel(IntPtr mmdModelPtr, int rigidBodyID, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		MMD4MecanimBulletPhysicsConfigRigidBodyMMDModel(mmdModelPtr, rigidBodyID, iValues, iValueLength, fValues, fValueLength);
	}

	private static void _ConfigJointMMDModel(IntPtr mmdModelPtr, int jointID, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength)
	{
		MMD4MecanimBulletPhysicsConfigJointMMDModel(mmdModelPtr, jointID, iValues, iValueLength, fValues, fValueLength);
	}

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsInitialize();

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsFinalize();

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern IntPtr MMD4MecanimBulletPhysicsDebugLog(int clanupFlag);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern IntPtr MMD4MecanimBulletPhysicsCreateWorld(IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsConfigWorld(IntPtr worldPtr, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsDestroyWorld(IntPtr worldPtr);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsUpdateWorld(IntPtr worldPtr, float deltaTime, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern IntPtr MMD4MecanimBulletPhysicsCreateRigidBody(IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsConfigRigidBody(IntPtr rigidBodyPtr, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsDestroyRigidBody(IntPtr rigidBodyPtr);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsJoinWorldRigidBody(IntPtr worldPtr, IntPtr rigidBodyPtr);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsLeaveWorldRigidBody(IntPtr rigidBodyPtr);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsResetWorldRigidBody(IntPtr rigidBodyPtr);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsUpdateRigidBody(IntPtr rigidBodyPtr, int updateFlags, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern int MMD4MecanimBulletPhysicsLateUpdateRigidBody(IntPtr rigidBodyPtr, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern IntPtr MMD4MecanimBulletPhysicsCreateMMDModel(IntPtr mmdModelBytes, int mmdModelLength, IntPtr mmdIndexBytes, int mmdIndexLength, IntPtr mmdVertexBytes, int mmdVertexLength, IntPtr iMeshValues, int meshLength, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern int MMD4MecanimBulletPhysicsGetFullBodyIKDataMMDModel(IntPtr mmdModelPtr, IntPtr fullBodyIKBoneID, IntPtr fullBodyIKPositions, IntPtr fullBodyIKRotations, int fullBodyIKLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern int MMD4MecanimBulletPhysicsUploadMeshMMDModel(IntPtr mmdModelPtr, int meshID, IntPtr vertices, IntPtr normals, IntPtr boneWeights, int vertexLength, IntPtr bindposes, int boneLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsConfigMMDModel(IntPtr mmdModelPtr, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsDestroyMMDModel(IntPtr mmdModelPtr);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsJoinWorldMMDModel(IntPtr worldPtr, IntPtr mmdModelPtr);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsLeaveWorldMMDModel(IntPtr mmdModelPtr);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsResetWorldMMDModel(IntPtr mmdModelPtr);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern int MMD4MecanimBulletPhysicsPreUpdateMMDModel(IntPtr mmdModelPtr, int updateFlags, IntPtr iBoneValues, int boneLength, IntPtr iRigidBodyValues, int rigidBodyLength, IntPtr ikWeights, int ikLength, IntPtr morphWeights, int morphLength, IntPtr iFullBodyIKValues, IntPtr fullBodyIKWeights, int fullBodyIKLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsUpdateMMDModel(IntPtr mmdModelPtr, int updateFlags, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength, IntPtr modelTransform, IntPtr iBoneValues, IntPtr boneTransforms, IntPtr boneLocalPositions, IntPtr boneLocalRotations, IntPtr boneUserPositions, IntPtr boneUserRotations, int boneLength, IntPtr iRigidBodyValues, int rigidBodyLength, IntPtr ikWeights, int ikLength, IntPtr morphWeights, int morphLength, IntPtr iFullBodyIKValues, IntPtr fullBodyIKTransforms, IntPtr fullBodyIKWeights, int fullBodyIKLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern int MMD4MecanimBulletPhysicsLateUpdateMMDModel(IntPtr mmdModelPtr, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength, IntPtr iBoneValues, IntPtr bonePositions, IntPtr boneRotations, int boneLength, IntPtr iMeshValues, int meshLength, IntPtr morphWeigts, int morphLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern int MMD4MecanimBulletPhysicsLateUpdateMeshMMDModel(IntPtr mmdModelPtr, int meshID, IntPtr vertices, IntPtr normals, int vertexLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsConfigBoneMMDModel(IntPtr mmdModelPtr, int boneID, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsConfigRigidBodyMMDModel(IntPtr mmdModelPtr, int rigidBodyID, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);

	[DllImport("MMD4MecanimBulletPhysics")]
	public static extern void MMD4MecanimBulletPhysicsConfigJointMMDModel(IntPtr mmdModelPtr, int jointID, IntPtr iValues, int iValueLength, IntPtr fValues, int fValueLength);
}
