using SaveDataStructures;

public interface ISavable : IGameObject
{
	SavableData Save();

	void Load(SavableData savedData, bool loadPosition = true);
}
