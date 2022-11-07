using System.Collections.Generic;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace Minipede.Gameplay.Weapons
{
	public class FireSpread : IFireSpread
	{
		private readonly Settings _settings;
		private readonly ShotSpot _shotSpot;

		public FireSpread( Settings settings,
			ShotSpot shotSpot )
		{
			_settings = settings;
			_shotSpot = shotSpot;
		}

		public IEnumerable<IOrientation> GetSpread()
		{
			if ( _settings.Spread <= 1 )
			{
				yield return new Orientation(
					_shotSpot.Position,
					Quaternion.LookRotation( Vector3.forward, _shotSpot.Facing ),
					null
				);
			}
			else
			{
				Vector2 facing = _shotSpot.Facing;
				Vector2 tangent = _shotSpot.Tangent;

				float angleStep = _settings.Angle / (_settings.Spread - 1);
				float radianStep = Mathf.Deg2Rad * angleStep;
				float radianOffset = Mathf.Deg2Rad * (_settings.Angle / 2f);

				for ( int idx = 0; idx < _settings.Spread; ++idx )
				{
					float radian = radianStep * idx - radianOffset;
					Vector2 direction = Mathf.Cos( radian ) * facing + Mathf.Sin( radian ) * tangent;

					yield return new Orientation(
						_shotSpot.Position,
						Quaternion.LookRotation( Vector3.forward, direction ),
						null
					);
				}
			}
		}

		[System.Serializable]
		public struct Settings
		{
			[MinValue( 1 ), OnValueChanged( "OnSpreadChanged" ), Delayed]
			[InlineButton( "IncrementSpread", Label = ">" ), InlineButton( "DecrementSpread", Label = "<" )]
			public int Spread;
			[PropertyRange( 0, "@360f - 360f / Spread" ), OnInspectorGUI( Append = "DrawSpread" )]
			public float Angle;

#if UNITY_EDITOR
			private void IncrementSpread()
			{
				++Spread;
				OnSpreadChanged();
			}
			private void DecrementSpread()
			{
				--Spread;
				OnSpreadChanged();
			}

			private void OnSpreadChanged()
			{
				float maxAngle = 360f - 360f / Spread;
				if ( Angle > maxAngle )
				{
					Angle = maxAngle;
				}
			}

			private void DrawSpread()
			{
				float size = 150f + GUIHelper.CurrentIndentAmount;
				float lineLength = size * 0.9f / 2f;
				float gridSize = 10;

				Rect rect = EditorGUILayout.GetControlRect( false, GUILayout.Height( size ) );
				rect = EditorGUI.IndentedRect( rect );
				GUI.Box( rect, GUIContent.none, SirenixGUIStyles.BoxContainer );

				using ( new GUI.ClipScope( rect ) )
				{
					Handles.color = Color.white.SetAlpha( 0.1f );
					DrawGrid( rect, gridSize );

					Handles.color = Color.white;
					DrawActor( rect, gridSize );

					Handles.color = Color.white;
					Vector2 center = rect.size / 2f;
					if ( Spread <= 1 )
					{
						Handles.DrawLine( center, center + Vector2.down * lineLength );
					}
					else
					{
						float angleStep = Angle / (Spread - 1);
						float radianStep = Mathf.Deg2Rad * angleStep;
						float radianOffset = Mathf.Deg2Rad * (Angle / 2f);

						for ( int idx = 0; idx < Spread; ++idx )
						{
							float radian = radianStep * idx - radianOffset;
							Vector2 direction = Mathf.Cos( radian ) * Vector2.down + Mathf.Sin( radian ) * Vector2.right;
							Handles.DrawAAPolyLine( center, center + direction * lineLength );
						}
					}
				}
			}

			private void DrawGrid( Rect rect, float gridSize )
			{
				const float gridPadding = 10;

				int columnSteps = Mathf.CeilToInt( rect.width / gridSize );
				for ( int col = 1; col < columnSteps - 1; ++col )
				{
					Vector2 start = new Vector2( col * columnSteps, gridPadding );
					Vector2 end = new Vector2( col * columnSteps, rect.height - gridPadding );
					Handles.DrawDottedLine( start, end, 3 );
				}

				int rowSteps = Mathf.CeilToInt( rect.height / gridSize );
				for ( int row = 1; row < rowSteps - 1; ++row )
				{
					Vector2 start = new Vector2( gridPadding, row * columnSteps );
					Vector2 end = new Vector2( rect.width - gridPadding, row * columnSteps );
					Handles.DrawDottedLine( start, end, 6 );
				}
			}

			private void DrawActor( Rect rect, float gridSize )
			{
				Vector2 center = rect.size / 2f;
				float actorSize = rect.width / gridSize;

				Vector2 actorPos = center + Vector2.left * actorSize / 2f;
				Rect drawPos = new Rect( actorPos, Vector2.one * actorSize );

				Color color = Color.white.SetAlpha( 0.15f );
				Handles.DrawSolidRectangleWithOutline( drawPos, color, color.SetAlpha( 1 ) );
			}
#endif
		}
	}
}
