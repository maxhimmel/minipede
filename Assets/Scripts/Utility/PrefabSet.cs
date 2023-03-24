using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Utility
{
	public class PrefabSet<TPrefab>
		where TPrefab : Component
	{
		public int Count => _settings.Prefabs.Length;
		public IEnumerable<TPrefab> Prefabs => _settings.Prefabs;

		private readonly Settings _settings;

		public PrefabSet( Settings settings )
		{
			_settings = settings;
		}

		public TPrefab GetRandomPrefab()
		{
			int randIdx = Random.Range( 0, Count );
			return GetPrefab( randIdx );
		}

		public TPrefab GetPrefab( int index )
		{
			return _settings.Prefabs[index];
		}

		[System.Serializable]
		public class Settings
		{
			[AssetSelector]
			public TPrefab[] Prefabs;
		}
	}
}