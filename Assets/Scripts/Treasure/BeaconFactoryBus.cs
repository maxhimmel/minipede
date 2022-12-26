using System.Collections.Generic;
using System.Linq;
using Minipede.Utility;
using Sirenix.OdinInspector;

namespace Minipede.Gameplay.Treasures
{
	public class BeaconFactoryBus
	{
		private readonly Dictionary<ResourceType, Beacon.Factory> _factories;

		public BeaconFactoryBus( List<Beacon.Factory> factories )
		{
			_factories = factories.ToDictionary( factory => factory.Resource );
		}

		public Beacon Create( ResourceType resource, IOrientation placement )
		{
			var factory = _factories[resource];
			return factory.Create( placement );
		}

		[InlineProperty]
		[System.Serializable]
		public struct Settings
		{
			public ResourceType ResourceType;
			public Beacon Prefab;
		}
	}
}