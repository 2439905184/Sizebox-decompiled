using System.Collections.Generic;

public interface IMorphable
{
	bool MorphsInitialized { get; }

	List<EntityMorphData> Morphs { get; }

	void InitializeMorphs();

	void SetMorphValue(int index, float weight);

	void SetMorphValue(string morphName, float weight);
}
