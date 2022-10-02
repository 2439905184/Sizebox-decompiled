public class GiantessCollider : EntityComponent
{
	public override void Initialize(EntityBase entity)
	{
		base.transform.SetParent(null, true);
	}
}
