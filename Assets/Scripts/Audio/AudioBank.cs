using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Minipede.Gameplay.Audio
{
    [CreateAssetMenu( menuName = "Audio/Bank" )]
    public class AudioBank : ScriptableObject
    {
        public string Category => _category;
        public IEnumerable<BankEvent> Events => _events;

        [SerializeField] private string _category;

        [ListDrawerSettings( OnTitleBarGUI = "DrawSortButton" )]
        [OnValueChanged( "OnBankEntryChanged", IncludeChildren = true )]
        [SerializeField] private BankEvent[] _events;

        public string ExportKey( BankEvent data )
		{
            return $"{_category}/{data.EventName}";
		}

#if UNITY_EDITOR
        private void OnBankEntryChanged()
		{
            foreach ( var entry in _events )
			{
                if ( string.IsNullOrEmpty( entry.EventName ) && entry.Clip != null )
				{
                    entry.EventName = entry.Clip.name;
				}
			}
		}

        private void DrawSortButton()
        {
            if ( SirenixEditorGUI.ToolbarButton( new GUIContent( EditorIcons.TriangleUp.Active, "Sort" ) ) )
            {
                _events.Sort();
            }
        }
#endif
    }
}
