using Minipede.Utility;

namespace Minipede.Gameplay.LevelPieces
{
	public class CleansedAreaSetFactory
	{
		private readonly ICleansedAreaProvider _provider;
		private readonly CleansedArea.Factory _factory;

		public CleansedAreaSetFactory( ICleansedAreaProvider provider,
			CleansedArea.Factory factory )
		{
			_provider = provider;
			_factory = factory;
		}

		public CleansedArea Create( IOrientation placement )
		{
			var prefab = _provider.GetAsset();
			return _factory.Create( prefab, placement );
		}
	}
}