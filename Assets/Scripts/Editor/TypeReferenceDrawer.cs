using System.Linq;
using Minipede.Utility;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Minipede.Editor
{
	public class TypeReferenceDrawer : OdinAttributeDrawer<TypeReferenceAttribute, TypeReference>
	{
		private TypeSelector _typeSelector;
		private GUIStyle _buttonStyle;

		protected override void Initialize()
		{
			base.Initialize();

			_typeSelector = new TypeSelector( this.Attribute.GetTypes(), false );
			_typeSelector.FlattenTree = true;
			_typeSelector.EnableSingleClickToSelect();
			_typeSelector.SelectionConfirmed += ( selection ) =>
			{
				this.ValueEntry.SmartValue.Fullname = selection.FirstOrDefault().FullName;
			};

			_buttonStyle = SirenixGUIStyles.DropDownMiniButton;
			_buttonStyle.alignment = TextAnchor.MiddleLeft;
		}

		protected override void DrawPropertyLayout( GUIContent label )
		{
			SirenixEditorGUI.BeginHorizontalPropertyLayout( label );
			{
				string typeName = this.ValueEntry.SmartValue.IsValid ? this.ValueEntry.SmartValue.RefType.Name : "null";
				if ( GUILayout.Button( typeName, _buttonStyle ) )
				{
					_typeSelector.ShowInPopup();
				}
			}
			SirenixEditorGUI.EndHorizontalPropertyLayout();

			EditorGUILayout.Space( EditorGUIUtility.standardVerticalSpacing );
		}
	}
}