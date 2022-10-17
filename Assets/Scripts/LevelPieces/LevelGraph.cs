using Minipede.Installers;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class LevelGraph : MonoBehaviour
	{
		public Settings Data { get; private set; }

		private Vector2 _initialOrigin;
		private Vector2 _localOrigin;

		[Inject]
		public void Construct( GameplaySettings.Level settings )
		{
			Data = settings.Graph;

			TryUpdateLocalOriginCache( forceUpdate: true );
		}

		public Vector2 CellCoordToWorldPos( Vector2Int cellCoord )
		{
			Vector2 position = transform.position
				+ Vector3.up * cellCoord.Row() * Data.Size.y
				+ Vector3.right * cellCoord.Col() * Data.Size.x;

			return position
				+ Data.Size * 0.5f
				+ Data.Offset;
		}

		public Vector2Int WorldPosToClampedCellCoord( Vector2 worldPosition )
		{
			Vector2Int cellCoord = WorldPosToCellCoord( worldPosition );

			bool isRowValid = cellCoord.Row() >= 0 && cellCoord.Row() < Data.Dimensions.Row();
			bool isColumnValid = cellCoord.Col() >= 0 && cellCoord.Col() < Data.Dimensions.Col();
			if ( isRowValid && isColumnValid )
			{
				return cellCoord;
			}

			if ( !isRowValid )
			{
				cellCoord = cellCoord.Row() < 0
					? cellCoord.MoveRowUp()
					: cellCoord.MoveRowDown();
			}
			if ( !isColumnValid )
			{
				cellCoord = cellCoord.Col() < 0
					? cellCoord.MoveColumnRight()
					: cellCoord.MoveColumnLeft();
			}

			return cellCoord;
		}

		public Vector2Int WorldPosToCellCoord( Vector2 worldPosition )
		{
			TryUpdateLocalOriginCache();

			worldPosition.x /= Data.Size.x;
			worldPosition.y /= Data.Size.y;

			Vector2 localPos = worldPosition - _localOrigin;

			return new Vector2Int()
			{
				x = Mathf.RoundToInt( localPos.y ), // rows
				y = Mathf.RoundToInt( localPos.x )  // columns
			};
		}

		private bool TryUpdateLocalOriginCache( bool forceUpdate = false )
		{
			if ( !forceUpdate && _initialOrigin.Approximately( transform.position ) )
			{
				return false;
			}

			_initialOrigin = transform.position;
			_localOrigin = transform.position;

			_localOrigin += Data.Size * 0.5f;
			_localOrigin += Data.Offset;

			_localOrigin.x /= Data.Size.x;
			_localOrigin.y /= Data.Size.y;

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
		#region EDITOR
		[BoxGroup( "Tools" )]
		[SerializeField] private bool _drawGraph = true;
		[BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]
		[SerializeField] private Color _gridColor = Color.red;
		[BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]
		[SerializeField] private Color _playerColor = Color.white;
		[BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]
		[SerializeField] private Color _playerDepthColor = Color.yellow;
		[BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]

		[InfoBox( "These settings are not used at runtime. Please find the <b>GameplaySettings</b> asset.", InfoMessageType.Error )]
		[Space, BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]
		[SerializeField] private LevelBuilder.Settings _editorBuilder;
		[Space, BoxGroup( "Tools" ), ShowIf( "_drawGraph" )]
		[SerializeField] private Settings _editorGraph;

		private void OnDrawGizmos()
		{
			if ( !_drawGraph )
			{
				return;
			}

			// Player area ...
			Gizmos.color = _playerColor;
			DrawRowsAndColumns( 0, _editorBuilder.PlayerRows - _editorBuilder.PlayerRowDepth );

			// Level area ...
			Gizmos.color = _gridColor;
			DrawRowsAndColumns( _editorBuilder.PlayerRows, _editorGraph.Dimensions.Row() );

			// Player depth area ...
			Gizmos.color = _playerDepthColor;
			DrawRowsAndColumns( _editorBuilder.PlayerRowDepth, _editorBuilder.PlayerRows );
		}

		private void DrawRowsAndColumns( int startRow, int rowCount )
		{
			Vector2 center = transform.position;

			Vector2 centerOffset = _editorGraph.Size * 0.5f;
			centerOffset += _editorGraph.Offset;

			for ( int row = startRow; row < rowCount; ++row )
			{
				for ( int col = 0; col < _editorGraph.Dimensions.Col(); ++col )
				{
					Vector2 pos = center + centerOffset
						+ Vector2.up * row * _editorGraph.Size.y
						+ Vector2.right * col * _editorGraph.Size.x;

					Gizmos.DrawWireCube( pos, _editorGraph.Size );
				}
			}
		}
		#endregion EDITOR
#endif
	}
}
