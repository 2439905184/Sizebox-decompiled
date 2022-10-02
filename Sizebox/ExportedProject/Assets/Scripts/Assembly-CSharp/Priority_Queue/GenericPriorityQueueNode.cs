namespace Priority_Queue
{
	public class GenericPriorityQueueNode<TPriority>
	{
		public TPriority Priority { get; protected internal set; }

		public int QueueIndex { get; internal set; }

		public long InsertionIndex { get; internal set; }
	}
}
