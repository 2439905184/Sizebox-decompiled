public interface IPlayable : IEntity, IGameObject
{
	Player Player { get; }

	bool IsPlayerControlled { get; }

	bool StartPlayerControl(Player player);

	void OnPlayerControlEnd(Player player);
}
