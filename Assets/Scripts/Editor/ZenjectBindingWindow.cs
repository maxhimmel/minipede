using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Minipede.Editor
{
    public class ZenjectBindingWindow : OdinEditorWindow
	{
		[HideInPlayMode]
		[LabelText( "Identifiers" )]
		[ListDrawerSettings( Expanded = true, IsReadOnly = true, OnTitleBarGUI = "RefreshBindings" )]
		[SerializeField] private List<BindingElement> _bindings;

		[HideInEditorMode]
		[InfoBox( "Only viewable outside play-mode.", InfoMessageType = InfoMessageType.Error )]
		[SerializeField, DisplayAsString, HideLabel] private string _placeholder = string.Empty;

		[MenuItem( "Minipede/Zenject Binding IDs" )]
		private static void OpenWindow()
		{
			GetWindow<ZenjectBindingWindow>( "Scene Binding IDs" ).Show();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			EditorApplication.playModeStateChanged -= OnEnterEditMode;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			EditorApplication.playModeStateChanged += OnEnterEditMode;

			SetupBindings();
		}

		private void OnEnterEditMode( PlayModeStateChange state )
		{
			if ( state != PlayModeStateChange.EnteredEditMode )
			{
				return;
			}

			SetupBindings();
		}

		private void RefreshBindings()
		{
			if ( SirenixEditorGUI.ToolbarButton( EditorIcons.Refresh ) )
			{
				SetupBindings();
			}
		}

		private void SetupBindings()
		{
			var lookup = new Dictionary<string, List<Component>>();

			var sceneBindings = FindObjectsOfType<ZenjectBinding>( includeInactive: true );
			foreach ( var zenjectBinding in sceneBindings )
			{
				if ( string.IsNullOrEmpty( zenjectBinding.Identifier ) )
				{
					continue;
				}

				if ( lookup.TryGetValue( zenjectBinding.Identifier, out var bindings ) )
				{
					foreach ( var component in zenjectBinding.Components )
					{
						bindings.Add( component );
					}
				}
				else
				{
					lookup.Add( zenjectBinding.Identifier, new List<Component>( zenjectBinding.Components ) );
				}
			}

			_bindings = new List<BindingElement>( lookup.Count );
			foreach ( var kvp in lookup )
			{
				_bindings.Add( new BindingElement()
				{
					Identifier = kvp.Key,
					Bindings = kvp.Value
				} );
			}
			_bindings.Sort();
		}

		[System.Serializable]
		[InlineProperty]
		private class BindingElement : IComparable<BindingElement>
		{
			[HorizontalGroup( 0.3f )]
			[DisplayAsString, HideLabel]
			public string Identifier;

			[HorizontalGroup]
			[ListDrawerSettings( Expanded = true, IsReadOnly = true )]
			public List<Component> Bindings;

			public int CompareTo( BindingElement other )
			{
				return Identifier.CompareTo( other.Identifier );
			}
		}
	}
}
