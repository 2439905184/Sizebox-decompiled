using System;

namespace MMD4MecanimInternal.Bullet
{
	public interface IBridge
	{
		object CreateCachedThreadQueue(int maxThreads);

		ThreadQueueHandle InvokeCachedThreadQueue(object cachedThreadQueue, Action action);

		void WaitEndCachedThreadQueue(object cachedThreadQueue, ref ThreadQueueHandle threadQueueHandle);

		object CreatePararellCachedThreadQueue(int maxThreads);

		ThreadQueueHandle InvokeCachedPararellThreadQueue(object cachedPararellThreadQueue, PararellFunction function, int length);

		void WaitEndCachedPararellThreadQueue(object cachedPararellThreadQueue, ref ThreadQueueHandle threadQueueHandle);
	}
}
