using System.Linq;
using ControllerGlyph;
using Minipede.Utility;
using Rewired;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class ActionGlyphPrompt : MonoBehaviour
    {
        private static readonly ControllerType[] _computerControllers = new ControllerType[2] { 
            ControllerType.Keyboard, ControllerType.Mouse 
        };

        [BoxGroup( "Widget" )]
        [SerializeField] private string _promptFormat = "{0} Action";
        [BoxGroup( "Widget" )]
        [SerializeField] private TMP_Text _promptElement;

        [BoxGroup( "Input" )]
        [ActionIdProperty( typeof( ReConsts.Action ) )]
        [SerializeField] private int _actionId;
        [BoxGroup( "Input" )]
        [SerializeField] private ControllerElementType _type;
        [BoxGroup( "Input" )]
        [SerializeField] private AxisRange _axisRange;

        [FoldoutGroup( "Input/Computers" ), ValueDropdown( "_computerControllers" ), LabelText( "Priority" )]
        [SerializeField] private ControllerType _computerPriority;

        private ControllerModel _model;
        private ControllerGlyphBus _glyphBus;

        [Inject]
		public void Construct( ControllerModel model,
            ControllerGlyphBus glyphBus )
		{
            _model = model;
            _glyphBus = glyphBus;
		}

		private void Start()
        {
			_model.Changed += OnControllerChanged;

            OnControllerChanged( _model );
		}

		private void OnDestroy()
        {
            _model.Changed -= OnControllerChanged;
        }

        private void OnControllerChanged( ControllerModel model )
        {
            var request = new ControllerGlyphRequest()
            {
                ActionId = _actionId,
                AxisRange = _axisRange,
                ElementType = _type
            };

            bool isComputerController = false;

            switch ( model.ControllerType )
            {
                default:
                case ControllerType.Joystick:
                    request.Controller = ControllerType.Joystick;
                    break;

                case ControllerType.Keyboard:
                case ControllerType.Mouse:
                    isComputerController = true;
                    request.Controller = _computerPriority;
                    break;
            }

            if ( !_glyphBus.TryGetGlyph( request, out var glyph ) )
            {
                if ( isComputerController )
				{
                    request.Controller = _computerControllers.First( type => type != _computerPriority );
                    _glyphBus.TryGetGlyph( request, out glyph );
				}
            }

            _promptElement.text = string.Format( _promptFormat, glyph );
        }
	}
}
