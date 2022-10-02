namespace MMD4MecanimInternal.Bullet
{
	public struct ThreadQueueHandle
	{
		public object queuePtr;

		public uint queueID;

		public uint uniqueID;

		public ThreadQueueHandle(object queuePtr, uint queueID, uint uniqueID)
		{
			this.queuePtr = queuePtr;
			this.queueID = queueID;
			this.uniqueID = uniqueID;
		}

		public void Reset()
		{
			queuePtr = null;
			queueID = 0u;
			uniqueID = 0u;
		}

		public static implicit operator bool(ThreadQueueHandle rhs)
		{
			return rhs.queuePtr != null;
		}
	}
}
