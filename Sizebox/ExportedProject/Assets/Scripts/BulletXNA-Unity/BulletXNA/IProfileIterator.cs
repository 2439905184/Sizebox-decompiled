namespace BulletXNA
{
	public interface IProfileIterator
	{
		void First();

		void Next();

		bool Is_Done();

		bool Is_Root();

		void Enter_Child(int index);

		void Enter_Largest_Child();

		void Enter_Parent();

		string Get_Current_Name();

		int Get_Current_Total_Calls();

		float Get_Current_Total_Time();

		string Get_Current_Parent_Name();

		int Get_Current_Parent_Total_Calls();

		float Get_Current_Parent_Total_Time();
	}
}
