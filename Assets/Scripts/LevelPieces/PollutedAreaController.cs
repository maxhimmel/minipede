using System.Collections.Generic;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class PollutedAreaController : IInitializable
	{
		public float PollutionPercentage => Mathf.Clamp01( (_cleansedCells.Count / (float)_pollutionArea) / _pollutionWinPercentage.PollutionWinPercentage );

		private readonly IPollutionWinPercentage _pollutionWinPercentage;
		private readonly LevelGraph _levelGraph;
		private readonly SignalBus _signalBus;
		private readonly BoundsInt _worldSpaceBounds;
		private readonly HashSet<LevelCell> _cleansedCells;
		private readonly int _pollutionArea;

		public PollutedAreaController( Settings settings,
			IPollutionWinPercentage pollutionWinPercentage,
			LevelGraph levelGraph,
			SignalBus signalBus )
		{
			_pollutionWinPercentage = pollutionWinPercentage;
			_levelGraph = levelGraph;
			_signalBus = signalBus;
			_worldSpaceBounds = CalculateWorldSpaceBounds( settings );

			_cleansedCells = new HashSet<LevelCell>();
			_pollutionArea = settings.Size.x * settings.Size.y;
		}

		private BoundsInt CalculateWorldSpaceBounds( Settings settings )
		{
			Vector3 position = new Vector3( settings.CellCoord.Col(), settings.CellCoord.Row() );

			return new BoundsInt(
				position:	_levelGraph.transform.TransformPoint( position ).RoundToVector3Int(),
				size:		new Vector3Int( settings.Size.x, settings.Size.y, 10 )
			);
		}

		public virtual void Cleanse( Bounds bounds )
		{
			BoundsInt roundedBounds = new BoundsInt(
				Mathf.RoundToInt( bounds.min.x ),
				Mathf.RoundToInt( bounds.min.y ),
				1,
				Mathf.RoundToInt( bounds.size.x ),
				Mathf.RoundToInt( bounds.size.y ),
				1
			);

			foreach ( var pos in roundedBounds.allPositionsWithin )
			{
				if ( !_worldSpaceBounds.Contains( pos ) )
				{
					continue;
				}

				Vector3 centeredPos = pos + new Vector3( 0.5f, 0.5f );
				if ( _levelGraph.TryGetCellData( centeredPos, out var cell ) )
				{
					_cleansedCells.Add( cell );
				}
			}

			_signalBus.AbstractFire( new PollutionLevelChangedSignal()
			{
				CanWin = PollutionPercentage >= 1,
				NormalizedLevel = PollutionPercentage
			} );
		}

		public void Initialize()
		{
			_signalBus.AbstractFire( new PollutionLevelChangedSignal()
			{
				CanWin = PollutionPercentage >= 1,
				NormalizedLevel = PollutionPercentage 
			} );
		}

		[System.Serializable]
		public struct Settings : IPollutionWinPercentage
		{
			public float PollutionWinPercentage => WinPercentage;

			[PropertyRange( 0, 1 )]
			public float WinPercentage;

			[BoxGroup, InfoBox( "The lower left position of the polluted boundary. (X: Row | Y: Column)", InfoMessageType.None )]
			public Vector2Int CellCoord;
			[BoxGroup]
			public Vector2Int Size;
		}
	}
}