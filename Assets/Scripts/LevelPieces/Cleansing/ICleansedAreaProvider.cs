using Minipede.Utility;

namespace Minipede.Gameplay.LevelPieces
{
    public interface ICleansedAreaProvider : IProvider<CleansedArea>
    {
    }

	public class RandomCleansedAreaProvider : ICleansedAreaProvider
	{
		private readonly CleansedAreaSet _sampler;

		public RandomCleansedAreaProvider( CleansedAreaSet sampler )
		{
			_sampler = sampler;
		}

		public CleansedArea GetAsset()
		{
			return _sampler.Count > 0
				? _sampler.GetRandomPrefab()
				: null;
		}
	}
}
