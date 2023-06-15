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
		public class Settings
		{
			[TableList]
			public WaveVisual[] Visuals;
		}

		[System.Serializable]
		public class WaveVisual
		{
			public string Id;
			public Color Color;

			[PreviewField( Alignment = ObjectFieldAlignment.Left, Height = 30 )]
			public Sprite Icon;
		}
	}
}