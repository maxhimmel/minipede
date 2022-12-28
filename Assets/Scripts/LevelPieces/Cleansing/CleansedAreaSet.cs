using System.Collections.Generic;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class CleansedAreaSet
	{
		public int Count => _settings.Prefabs.Length;
		public IEnumerable<CleansedArea> Prefabs => _settings.Prefabs;

		private readonly Settings _settings;

		public CleansedAreaSet( Settings settings )
		{
			_settings = settings;
		}

		public CleansedArea GetRandomPrefab()
		{
			int randIdx = Random.Range( 0, Count );
			return GetPrefab( randIdx );
		}

		public CleansedArea GetPrefab( int index )
		{
			return _settings.Prefabs[index];
		}

		[System.Serializable]
		public struct Settings
		{
			public CleansedArea[] Prefabs;
		}
	}
}