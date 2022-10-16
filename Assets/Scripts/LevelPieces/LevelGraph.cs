using System;
using System.Linq;
using System.Threading.Tasks;
using Minipede.Installers;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class LevelGraph : MonoBehaviour
    {
		private GameplaySettings.Level _settings;
		private Block.Factory _blockFactory;
		private Graph<LevelCell> _graph;

		private Vector2 _initialOrigin;
		private Vector2 _localOrigin;

		[Inject]
		public void Construct( GameplaySettings.Level settings,
			Block.Factory blockFactory )
		{
			_settings = settings;
			_blockFactory = blockFactory;

			TryUpdateLocalOriginCache( forceUpdate: true );

			_graph = new Graph<LevelCell>( settings.Graph.Dimensions.Row(), settings.Graph.Dimensions.Col(), CreateCellData );
		}

		private LevelCell CreateCellData( int row, int col )
		{
			return new LevelCell( GetCellCenter( row, col ) );
		}

		private Vector2 GetCellCenter( int row, int col )
		{
			Vector2 position = transform.position
				+ Vector3.up * row * _settings.Graph.Size.y
				+ Vector3.right * col * _settings.Graph.Size.x;

			return position 
				+ _settings.Graph.Size * 0.5f 
				+ _settings.Graph.Offset;
		}

		public async Task GenerateLevel()
		{
			_settings.RowGeneration.Init();

			// Go thru each row from top to bottom ...
			float secondsPerRow = _settings.SpawnRate / _settings.Graph.Dimensions.Row();
			for ( int row = _settings.Graph.Dimensions.Row() - 1; row >= 0; --row )
			{
				// Randomize the column indices ...
				int[] columnIndices = Enumerable.Range( 0, _settings.Graph.Dimensions.Col() ).ToArray();
				columnIndices.FisherYatesShuffle();

				// Create a block at a random cell ...
				int blockCount = _settings.RowGeneration.GetRandomItem();
				for ( int idx = 0; idx < blockCount; ++idx )
				{
					int randIdx = columnIndices[idx];
					var cell = _graph.GetCell( row, randIdx );

					CreateBlock( Block.Type.Regular, cell.Item );

					if ( idx + 1 >= blockCount && row <= 0 )
					{
						// No need to delay after the final block has been created ...
						break;
					}

					float secondsPerBlock = secondsPerRow / blockCount;
					await TaskHelpers.DelaySeconds( secondsPerBlock );
				}
			}
		}

		public Block CreateBlock( Block.Type type, LevelCell data )
		{
			var newBlock = _blockFactory.Create( type, data.Center, Quaternion.identity );
			newBlock.transform.SetParent( transform );

			data.Block = newBlock;

			return newBlock;
		}

		public bool TryGetCellData( Vector2 worldPosition, out LevelCell cellData )
		{
			TryUpdateLocalOriginCache();

			worldPosition.x /= _settings.Graph.Size.x;
			worldPosition.y /= _settings.Graph.Size.y;

			Vector2 localPos = worldPosition - _localOrigin;
			Vector2Int cellCoord = new Vector2Int()
			{
				x = Mathf.RoundToInt( localPos.y ), // rows
				y = Mathf.RoundToInt( localPos.x )	// columns
			};

			cellData = null;
			if ( cellCoord.Row() < 0 || cellCoord.Row() >= _settings.Graph.Dimensions.Row() )
			{
				return false;
			}
			if ( cellCoord.Col() < 0 || cellCoord.Col() >= _settings.Graph.Dimensions.Col() )
			{
				return false;
			}

			cellData = _graph.GetCell( cellCoord.Row(), cellCoord.Col() ).Item;
			return true;
		}

		private bool TryUpdateLocalOriginCache( bool forceUpdate = false )
		{
			if ( !forceUpdate && _initialOrigin.Approximately( transform.position ) )
			{
				return false;
			}

			_initialOrigin = transform.position;
			_localOrigin = transform.position;

			_localOrigin += _settings.Graph.Size * 0.5f;
			_localOrigin += _settings.Graph.Offset;

			_localOrigin.x /= _settings.Graph.Size.x;
			_localOrigin.y /= _settings.Graph.Size.y;

			return true;
		}

		[System.Serializable]
		public struct Settings
		{
			[InfoBox( "X: Row | Y: Column" )]
			public Vector2Int Dimensions;
			public Vector2 Size;
			public Vector2 Offset;
		}

#if UNITY_EDITOR
		[BoxGroup( "Tools" )]
		[SerializeField] private bool _drawGraph = true;
		[BoxGroup( "Tools" ), ShowIf("_drawGraph")]
		[SerializeField] private Color _gridColor = Color.red;
		[BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]
		[Space, InfoBox( "These settings are not used at runtime. Please find the <b>GameplaySettings</b> asset.", InfoMessageType.Error )]
		[SerializeField] private Settings _editorSettings;

		private void OnDrawGizmos()
		{
			if ( !_drawGraph )
			{
				return;
			}

			Gizmos.color = _gridColor;

			Vector2 center = transform.position;

			Vector2 centerOffset = _editorSettings.Size * 0.5f;
			centerOffset += _editorSettings.Offset;

			for ( int row = 0; row < _editorSettings.Dimensions.Row(); ++row )
			{
				for ( int col = 0; col < _editorSettings.Dimensions.Col(); ++col )
				{
					Vector2 pos = center + centerOffset
						+ Vector2.up * row * _editorSettings.Size.y
						+ Vector2.right * col * _editorSettings.Size.x;

					Gizmos.DrawWireCube( pos, _editorSettings.Size );
				}
			}
		}
#endif
	}
}
