namespace Minipede.Gameplay.Treasures
{
    public interface ICollector<TCollectable>
	{
		/// <returns>True if collected.</returns>
		bool Collect( TCollectable collectable );
	}
}
