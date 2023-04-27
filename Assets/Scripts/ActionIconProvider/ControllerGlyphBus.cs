﻿using System.Linq;
using Rewired;
using Sirenix.OdinInspector;

namespace Minipede
{
	public class ControllerGlyphBus
	{
		private readonly Settings _settings;
		private readonly Player _input;

		public ControllerGlyphBus( Settings settings,
			Player input )
		{
			_settings = settings;
			_input = input;
		}

		public string GetGlyph( Request request )
		{
			var controllerType = request.Controller;
			var controllerRequest = request.ToControllerRequest( _input );

			if ( controllerType == ControllerType.Custom )
			{
				throw new System.NotImplementedException();
			}
			else if ( controllerType == ControllerType.Keyboard )
			{
				return _settings.Keyboard.GetGlyph( controllerRequest );
			}
			else if ( controllerType == ControllerType.Mouse )
			{
				return _settings.Mouse.GetGlyph( controllerRequest );
			}
			else
			{
				var controller = _input.controllers.GetLastActiveController( controllerType )
					?? _input.controllers.Controllers.FirstOrDefault( c => c.type == controllerType );

				if ( controller == null )
				{
					return string.Empty;
				}

				var joystickGlyph = _settings.Joysticks.FirstOrDefault( j => j.InputGuid == controller.hardwareTypeGuid.ToString() );

				return joystickGlyph != null
					? joystickGlyph.GetGlyph( controllerRequest )
					: _settings.JoystickFallback.GetGlyph( controllerRequest );
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

			public abstract int GetActionId();

			internal ControllerGlyphs.Request ToControllerRequest( Player input )
			{
				return new ControllerGlyphs.Request( input,
					GetActionId(),
					ElementType,
					AxisRange );
			}
		}
	}
}