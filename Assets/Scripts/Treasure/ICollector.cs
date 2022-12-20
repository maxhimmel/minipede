namespace Minipede.Gameplay.Treasures
{
    public interface ICollector<TCollectable>
	{
		void Collect( TCollectable collectable );
	}
}
