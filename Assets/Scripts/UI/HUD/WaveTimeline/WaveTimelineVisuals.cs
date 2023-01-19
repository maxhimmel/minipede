using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class WaveTimelineVisuals
	{
		private readonly Dictionary<string, WaveVisual> _visuals;

		public WaveTimelineVisuals( Settings settings )
		{
			_visuals = settings.Visuals.ToDictionary( visual => visual.Id );
		}

		public WaveVisual GetVisual( string id )
		{
			return _visuals[id];
		}

		[System.Serializable]
		public struct Settings
		{
			[TableList]
			public WaveVisual[] Visuals;
		}

		[System.Serializable]
		public struct WaveVisual
		{
			public string Id;
			public Color Color;
		}
	}
}