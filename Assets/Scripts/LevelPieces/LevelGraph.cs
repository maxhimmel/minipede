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
		private Vector2 _centerOffset;

		[Inject]
		public void Construct( GameplaySettings.Level settings,
			Block.Factory blockFactory )
		{
			_settings = settings;
			_blockFactory = blockFactory;

			CacheCenterOffset();

			_graph = new Graph<LevelCell>( settings.Graph.Dimensions.Row(), settings.Graph.Dimensions.Col(), CreateCellData );
		}

		private void CacheCenterOffset()
		{
			_centerOffset = 0.5f * new Vector2(
				_settings.Graph.Dimensions.Col() * _settings.Graph.Size.x,
				_settings.Graph.Dimensions.Row() * _settings.Graph.Size.y
			);
			_centerOffset -= _settings.Graph.Size * 0.5f;
			_centerOffset -= _settings.Graph.Offset;
		}

		private LevelCell CreateCellData( int row, int col )
		{
			return new LevelCell( GetCellCenter( row, col ) );
		}

		private Vector2 GetCellCenter( int row, int col )
		{
			Vector2 center = transform.position
				+ Vector3.up * row * _settings.Graph.Size.y
				+ Vector3.right * col * _settings.Graph.Size.x;

			return center - _centerOffset;
		}

		public async Task GenerateLevel()
		{
			_settings.RowGeneration.Init();

			// Go thru each row from top to bottom ...
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

					CreateBlock( cell.Item );

					await TaskHelpers.DelaySeconds( _settings.SpawnRate );
				}
			}
		}

		private Block CreateBlock( LevelCell data )
		{
			var newBlock = _blockFactory.Create();

			Vector3 blockScale = _settings.Graph.Size;
			blockScale.z = 1;
			newBlock.transform.localScale = blockScale;

			newBlock.transform.SetParent( transform );
			newBlock.transform.position = data.Center;

			return newBlock;
		}

#if UNITY_EDITOR
		[BoxGroup( "Tools" )]
		[SerializeField] private Color _gridColor = Color.red;
		[BoxGroup( "Tools" )]
		[Space, InfoBox( "These settings are not used at runtime. Please find the <b>GameplaySettings</b> asset.", InfoMessageType.Error )]
		[SerializeField] private Settings _editorSettings;

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = _gridColor;

			Vector2 center = transform.position;

			Vector2 centerOffset = 0.5f * new Vector2(
				_editorSettings.Dimensions.Col() * _editorSettings.Size.x,
				_editorSettings.Dimensions.Row() * _editorSettings.Size.y
			);
			centerOffset -= _editorSettings.Size * 0.5f;
			centerOffset -= _editorSettings.Offset;

			for ( int row = 0; row < _editorSettings.Dimensions.Row(); ++row )
			{
				for ( int col = 0; col < _editorSettings.Dimensions.Col(); ++col )
				{
					Vector2 pos = center - centerOffset
						+ Vector2.up * row * _editorSettings.Size.y
						+ Vector2.right * col * _editorSettings.Size.x;

					Gizmos.DrawWireCube( pos, _editorSettings.Size );
				}
			}
		}
#endif

		[System.Serializable]
		public struct Settings
		{
			[InfoBox( "X: Row | Y: Column" )]
			public Vector2Int Dimensions;
			public Vector2 Offset;
			public Vector2 Size;
		}
	}
}
