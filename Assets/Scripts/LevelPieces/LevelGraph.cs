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
		public Settings GraphSettings => _settings.Graph;

		private GameplaySettings.Level _settings;
		private Block.Factory _blockFactory;
		private Graph<LevelCell> _graph;

		[Inject]
		public void Construct( GameplaySettings.Level settings,
			Block.Factory blockFactory )
		{
			_settings = settings;
			_blockFactory = blockFactory;

			_graph = new Graph<LevelCell>( settings.Graph.Dimensions.Row(), settings.Graph.Dimensions.Col(), CreateCellData );
		}

		private LevelCell CreateCellData( int row, int col )
		{
			return new LevelCell( CellCoordToWorldPosition( row, col ) );
		}

		public Vector2 CellCoordToWorldPosition( int row, int col )
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
			for ( int row = _settings.Graph.Dimensions.Row() - 1; row >= _settings.PlayerRowDepth; --row )
			{
				// Randomize the column indices ...
				int[] columnIndices = Enumerable.Range( 0, _settings.Graph.Dimensions.Col() ).ToArray();
				columnIndices.FisherYatesShuffle();

				// Create a block at a random cell ...
				int blockCount = _settings.RowGeneration.GetRandomItem();
				for ( int idx = 0; idx < blockCount; ++idx )
				{
					int randIdx = columnIndices[idx];
					var cell = GetCell( row, randIdx );

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

		public LevelCell GetCellData( int row, int col )
		{
			return GetCell( row, col ).Item;
		}

		private Graph<LevelCell>.Cell GetCell( int row, int col )
		{
			return _graph.GetCell( row, col );
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
		#region EDITOR
		[BoxGroup( "Tools" )]
		[SerializeField] private bool _drawGraph = true;
		[InfoBox( "These settings are not used at runtime. Please find the <b>GameplaySettings</b> asset.", InfoMessageType.Error )]
		[BoxGroup( "Tools" ), ShowIf("_drawGraph")]
		[SerializeField] private Color _gridColor = Color.red;
		[BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]
		[SerializeField] private Color _playerColor = Color.white;
		[BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]
		[SerializeField] private Color _playerDepthColor = Color.yellow;
		[BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]

		[Space, HorizontalGroup( "Tools/Rows" ), SerializeField] private int _playerRows = 8;
		[BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]
		[Space, HorizontalGroup( "Tools/Rows" ), SerializeField] private int _spawnPlayerDepth = 4;

		[Space, BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]
		[SerializeField] private Settings _editorSettings;

		private void OnDrawGizmos()
		{
			if ( !_drawGraph )
			{
				return;
			}

			// Player area ...
			Gizmos.color = _playerColor;
			DrawRowsAndColumns( 0, _playerRows - _spawnPlayerDepth );

			// Level area ...
			Gizmos.color = _gridColor;
			DrawRowsAndColumns( _playerRows, _editorSettings.Dimensions.Row() );

			// Player depth area ...
			Gizmos.color = _playerDepthColor;
			DrawRowsAndColumns( _spawnPlayerDepth, _playerRows );
		}

		private void DrawRowsAndColumns( int startRow, int rowCount )
		{
			Vector2 center = transform.position;

			Vector2 centerOffset = _editorSettings.Size * 0.5f;
			centerOffset += _editorSettings.Offset;

			for ( int row = startRow; row < rowCount; ++row )
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
		#endregion EDITOR
#endif
	}
}
