using Zenject;

namespace Minipede.Gameplay.Treasures
{
    public class Treasure : Collectable<Treasure>
    {
		public ResourceType Resource { get; private set; }

		[Inject]
		public void Construct( ResourceType resource )
		{
			Resource = resource;
		}

		protected override Treasure GetCollectable()
		{
			return this;
		}
	}
}
