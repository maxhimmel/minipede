using System.Linq;
using Minipede.Gameplay.Audio;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Minipede.Editor
{
	public class AudioEventReferenceDrawer : OdinValueDrawer<AudioEventReference>
	{
		private AudioEventSelector _eventSelector;
		private GUIStyle _buttonStyle;

		protected override void Initialize()
		{
			_eventSelector = new AudioEventSelector();
			_eventSelector.EnableSingleClickToSelect();
			_eventSelector.SelectionConfirmed += ( selection ) =>
			{
				ValueEntry.SmartValue.EventName = selection.FirstOrDefault();
			};

			_buttonStyle = SirenixGUIStyles.DropDownMiniButton;
			_buttonStyle.alignment = TextAnchor.MiddleLeft;
		}

		protected override void DrawPropertyLayout( GUIContent label )
		{
			using ( new EditorGUILayout.HorizontalScope() )
			{
				if ( label != null )
				{
					EditorGUILayout.PrefixLabel( label );
				}
				if ( GUILayout.Button( ValueEntry.SmartValue.EventName, _buttonStyle ) )
				{
					_eventSelector.ShowInPopup();
				}
			}
		}
	}

	public class AudioEventSelector : OdinSelector<string>
	{
		protected override void BuildSelectionTree( OdinMenuTree tree )
		{
			tree.Selection.SupportsMultiSelect = false;

			string[] assetGuids = AssetDatabase.FindAssets( "t:AudioBank" );
			foreach ( string guid in assetGuids )
			{
				string assetPath = AssetDatabase.GUIDToAssetPath( guid );
				AudioBank bank = AssetDatabase.LoadAssetAtPath<AudioBank>( assetPath );

				foreach ( var data in bank.Events )
				{
					string key = bank.ExportKey( data );
					tree.Add( key, key );
				}
			}
		}
	}
}