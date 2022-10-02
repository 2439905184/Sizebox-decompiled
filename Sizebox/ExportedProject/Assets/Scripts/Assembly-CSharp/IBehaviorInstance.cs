public interface IBehaviorInstance
{
	void Start();

	void Exit(bool abort = false);

	bool AutoFinish();
}
