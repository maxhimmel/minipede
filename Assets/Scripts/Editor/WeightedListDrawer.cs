using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Minipede.Utility;

namespace Minipede.Editor
{
	[CustomPropertyDrawer( typeof( WeightedList ), true )]
	public class WeightedListDrawer : PropertyDrawer
	{
		private const string _itemsPropertyName = "_items";
		private const string _allowEmptyRollsName = "_allowEmptyRolls";

		private Dictionary<string, ReorderablePropertyData> _propertyPathToReorderableProperties = null;

		public WeightedListDrawer()
		{
			_propertyPathToReorderableProperties = new Dictionary<string, ReorderablePropertyData>();
		}

		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
		{
			ReorderablePropertyData reorderableProperty = GetReorderablePropertyData( property );

			position = OnGUI_AllowEmptyRolls( position, property );
			position = OnGUI_Reinitialize( position, property );
			OnGUI_ItemsList( position, property.displayName, reorderableProperty );

			if ( property.serializedObject.hasModifiedProperties )
			{
				NormalizeWeights( reorderableProperty.Property );

				property.serializedObject.ApplyModifiedProperties();
			}
		}

		private Rect OnGUI_AllowEmptyRolls( Rect position, SerializedProperty property )
		{
			float prevPosHeight = position.height;
			position.height = EditorGUIUtility.singleLineHeight;

			SerializedProperty allowEmptyRolls = property.FindPropertyRelative( _allowEmptyRollsName );
			GUIContent content = new GUIContent( allowEmptyRolls.displayName, allowEmptyRolls.tooltip );
			allowEmptyRolls.boolValue = EditorGUI.ToggleLeft( position, content, allowEmptyRolls.boolValue );

			position.height = prevPosHeight;
			position.y += EditorGUIUtility.singleLineHeight;
			return position;
		}

		private Rect OnGUI_Reinitialize( Rect position, SerializedProperty property )
		{
			float prevPosHeight = position.height;
			position.height = EditorGUIUtility.singleLineHeight;

			using ( new EditorGUI.DisabledGroupScope( !EditorApplication.isPlaying ) )
			{
				if ( GUI.Button( position, "Reinitialize" ) )
				{
					var tree = new PropertyTree<Object>( property.serializedObject );
					var treeProperty = tree.GetPropertyAtUnityPath( property.propertyPath );
					var instance = treeProperty.ValueEntry.WeakSmartValue as WeightedList;
					instance.Init( forceReinitialize: true );

					Debug.Log( $"Reinitialized successfully @ '{treeProperty.ParentType.FullName}'" );
				}
			}

			position.height = prevPosHeight;
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			return position;
		}

		private void OnGUI_ItemsList( Rect position, string headerName, ReorderablePropertyData reorderableProperty )
		{
			ReorderableList reorderable = reorderableProperty.Reorderable;
			SerializedProperty listProperty = reorderableProperty.Property;

			reorderable.drawHeaderCallback = ( Rect headerPos ) =>
			{
				EditorGUI.LabelField( headerPos, headerName );
			};

			reorderable.drawElementCallback = ( Rect elementPos, int index, bool isActive, bool isFocused ) =>
			{
				SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex( index );
				elementPos.height = EditorGUI.GetPropertyHeight( elementProperty );
				elementPos.y += EditorGUIUtility.standardVerticalSpacing;

				EditorGUI.PropertyField( elementPos, elementProperty );
			};

			reorderable.elementHeightCallback = ( int index ) =>
			{
				SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex( index );
				float height = EditorGUI.GetPropertyHeight( elementProperty );

				return height + EditorGUIUtility.singleLineHeight / 2f;
			};

			reorderable.DoList( position );
		}

		private ReorderablePropertyData GetReorderablePropertyData( SerializedProperty property )
		{
			if ( _propertyPathToReorderableProperties.TryGetValue( property.propertyPath, out ReorderablePropertyData data ) )
			{
				return data;
			}

			data = new ReorderablePropertyData( property, _itemsPropertyName );
			_propertyPathToReorderableProperties.Add( property.propertyPath, data );

			return data;
		}

		protected void NormalizeWeights( SerializedProperty itemsProperty )
		{
			int weightSum = 0;
			for ( int idx = 0; idx < itemsProperty.arraySize; ++idx )
			{
				SerializedProperty elementProperty = itemsProperty.GetArrayElementAtIndex( idx );
				SerializedProperty elementWeightProperty = elementProperty.FindPropertyRelative( WeightedNodeDrawer.NormalizedWeightPropertyName );

				weightSum += elementWeightProperty.intValue;
			}

			if ( weightSum <= 0 ) { return; }

			for ( int idx = 0; idx < itemsProperty.arraySize; ++idx )
			{
				SerializedProperty elementProperty = itemsProperty.GetArrayElementAtIndex( idx );
				SerializedProperty elementWeightProperty = elementProperty.FindPropertyRelative( WeightedNodeDrawer.NormalizedWeightPropertyName );

				float influence = elementWeightProperty.intValue / (float)weightSum;
				int normalizedWeight = Mathf.RoundToInt( influence * WeightedList.MaxWeight );

				elementWeightProperty.intValue = normalizedWeight;
			}
		}

		public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
		{
			float reinitializeButtonHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			ReorderablePropertyData reorderableProperty = GetReorderablePropertyData( property );
			return EditorGUIUtility.singleLineHeight + reorderableProperty.Reorderable.GetHeight() + reinitializeButtonHeight;
		}
	}

	public class ReorderablePropertyData
	{
		public ReorderableList Reorderable;
		public SerializedProperty Property;

		public ReorderablePropertyData( SerializedProperty property, string listPropertyName )
		{
			Property = property.FindPropertyRelative( listPropertyName );
			Reorderable = new ReorderableList( property.serializedObject, Property, true, true, true, true );
		}
	}

	[CustomPropertyDrawer( typeof( WeightedNode ), true )]
	public class WeightedNodeDrawer : PropertyDrawer
	{
		public const string NormalizedWeightPropertyName = "_normalizedWeight";

		private const string _weightPropertyName = "Weight";
		private const string _itemPropertyName = "Item";

		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
		{
			EditorGUI.DrawRect( position, new Color( 0, 0, 0, 0.2f ) );

			position.height = EditorGUIUtility.singleLineHeight;

			SerializedProperty weightProperty = property.FindPropertyRelative( NormalizedWeightPropertyName );
			EditorGUI.PropertyField( position, weightProperty, new GUIContent( _weightPropertyName ) );
			position.y += EditorGUI.GetPropertyHeight( weightProperty );

			position.y += EditorGUIUtility.standardVerticalSpacing;

			SerializedProperty itemProperty = property.FindPropertyRelative( _itemPropertyName );
			if ( itemProperty != null )
			{
				EditorGUI.PropertyField( position, itemProperty, true );
			}
			else
			{
				EditorGUI.LabelField( position, "Non-serialized property in use" );
			}
		}

		public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
		{
			int numProperties = 1; // "Weight" property ...

			SerializedProperty itemProperty = property.FindPropertyRelative( _itemPropertyName );
			numProperties += itemProperty != null
				? itemProperty.CountInProperty()
				: 1; // "Non-serialized property in use" warning ...

			return numProperties * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
		}
	}
}