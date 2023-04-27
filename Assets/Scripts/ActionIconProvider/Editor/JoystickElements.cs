using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Minipede.Editor
{
	[HideReferenceObjectPicker]
	[System.Serializable]
	public class JoystickElements : ISearchFilterable
	{
		private const string _glyphAssetPath = "Assets/Scripts/ActionIconProvider/Assets";

		[HideLabel]
		public ControllerIdentifier Identifier;

		[ListDrawerSettings( IsReadOnly = true, ShowPaging = false )]
		public List<JoystickElementId> Elements;

		bool ISearchFilterable.IsMatch( string searchString )
		{
			searchString = searchString.ToLower();
			return Identifier.ControllerName.ToLower().Contains( searchString ) 
				|| Identifier.EditorControllerName.ToLower().Contains( searchString );
		}

		[OnInspectorGUI, PropertyOrder( -1 )]
		private void CreateGlyphAsset()
		{
			if ( SirenixEditorGUI.MenuButton( EditorGUI.indentLevel, " Create Glyph Asset", false, EditorIcons.ImageCollection.Raw ) )
			{
				var savePath = EditorUtility.SaveFilePanelInProject(
					"Save Controller Glyph Asset",
					$"{Identifier.ControllerName}",
					"asset",
					"Please enter a file name to save the glyph asset to",
					 _glyphAssetPath
				);

				if ( !string.IsNullOrEmpty( savePath ) )
				{
					var newGlyphAsset = ScriptableObject.CreateInstance<JoystickGlyphs>();
					newGlyphAsset.Construct(
						Identifier,
						Elements.Select( e => (e.Element.Id, e.Element.Name) ).ToList()
					);

					AssetDatabase.CreateAsset( newGlyphAsset, savePath );
					AssetDatabase.Refresh();

					EditorUtility.FocusProjectWindow();
					EditorGUIUtility.PingObject( newGlyphAsset );
				}
			}
		}
	}
}