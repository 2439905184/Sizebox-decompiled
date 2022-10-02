namespace BulletXNA
{
	public interface IProfileManager
	{
		void Start_Profile(string name);

		void Stop_Profile();

		void CleanupMemory();

		void Reset();

		void Increment_Frame_Counter();

		int Get_Frame_Count_Since_Reset();

		float Get_Time_Since_Reset();

		void DumpRecursive(IProfileIterator profileIterator, int spacing);

		void DumpAll();

		IProfileIterator getIterator();
	}
}
