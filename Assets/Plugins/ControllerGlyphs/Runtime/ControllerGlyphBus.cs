using System.Linq;
using Rewired;
using Sirenix.OdinInspector;

namespace ControllerGlyph
{
	public class ControllerGlyphBus
	{
		private readonly Settings _settings;

		private Player _input;

		public ControllerGlyphBus( Settings settings )
		{
			_settings = settings;
		}

		public void Initialize( Player input )
		{
			_input = input;
		}

		public bool TryGetGlyph( Request request, out string glyph )
		{
			var controllerType = request.Controller;
			var controllerRequest = request.ToControllerRequest( _input );

			if ( controllerType == ControllerType.Custom )
			{
				throw new System.NotImplementedException();
			}
			else if ( controllerType == ControllerType.Keyboard )
			{
				return _settings.Keyboard.TryGetGlyph( controllerRequest, out glyph );
			}
			else if ( controllerType == ControllerType.Mouse )
			{
				return _settings.Mouse.TryGetGlyph( controllerRequest, out glyph );
			}
			else
			{
				var controller = _input.controllers.GetLastActiveController( controllerType )
					?? _input.controllers.Controllers.FirstOrDefault( c => c.type == controllerType );

				if ( controller == null )
				{
					glyph = string.Empty;
					return false;
				}

				var joystickGlyph = _settings.Joysticks.FirstOrDefault( j => j.InputGuid == controller.hardwareTypeGuid.ToString() );

				return joystickGlyph != null
					? joystickGlyph.TryGetGlyph( controllerRequest, out glyph )
					: _settings.JoystickFallback.TryGetGlyph( controllerRequest, out glyph );
			}
		}

		[System.Serializable]
		public class Settings
		{
			[BoxGroup( "Computer" )]
			public KeyboardGlyphs Keyboard;
			[BoxGroup( "Computer" )]
			public MouseGlyphs Mouse;

			[BoxGroup( "Joysticks" )]
			public JoystickGlyphs JoystickFallback;
			[BoxGroup( "Joysticks" )]
			public JoystickGlyphs[] Joysticks;
		}

		public abstract class Request
		{
			[BoxGroup]
			[PropertyOrder( -99 ), EnumToggleButtons, HideLabel]
			public ControllerType Controller = ControllerType.Joystick;

			[BoxGroup]
			public ControllerElementType ElementType = ControllerElementType.Button;

			[BoxGroup]
			public AxisRange AxisRange = AxisRange.Full;

			[BoxGroup]
			public bool SkipDisabledMaps = false;

			public abstract int GetActionId();

			internal ControllerGlyphs.Request ToControllerRequest( Player input )
			{
				return new ControllerGlyphs.Request( input,
					GetActionId(),
					ElementType,
					AxisRange,
					SkipDisabledMaps );
			}
		}
	}
}