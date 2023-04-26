using System.Collections.Generic;
using System.Linq;
using Rewired;
using Sirenix.OdinInspector;

namespace Minipede
{
	public class InputGlyphBus
	{
		private static readonly List<ControllerTemplateElementTarget> _tempTemplateElementTargets = new List<ControllerTemplateElementTarget>();

		private readonly Settings _settings;
		private readonly Player _input;

		public InputGlyphBus( Settings settings,
			Player input )
		{
			_settings = settings;
			_input = input;
		}

		public string GetGlyph( ControllerType controllerType, int actionId, AxisRange axisRange = AxisRange.Full )
		{
			if ( controllerType == ControllerType.Custom )
			{
				throw new System.NotImplementedException();
			}
			else if ( controllerType == ControllerType.Keyboard )
			{
				int elementId = GetElementId( controllerType, actionId, axisRange );
				return _settings.Keyboard.GetGlyph( elementId, axisRange );
			}
			else if ( controllerType == ControllerType.Mouse )
			{
				int elementId = GetElementId( controllerType, actionId, axisRange );
				return _settings.Mouse.GetGlyph( elementId, axisRange );
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
				bool hasJoystickGlyph = joystickGlyph != null;

				var elementId = hasJoystickGlyph
						? GetElementId( controllerType, actionId, axisRange )
						: GetTemplateElementId( actionId );

				return hasJoystickGlyph
					? joystickGlyph.GetGlyph( elementId, axisRange )
					: _settings.JoystickFallback.GetGlyph( elementId, axisRange );
			}
		}

		private int GetElementId( ControllerType controllerType, int actionId, AxisRange axisRange = AxisRange.Full )
		{
			//if ( controllerType != ControllerType.Keyboard )
			//{
			//	var elementMaps = _input
			//		.controllers
			//		.maps
			//		.ElementMapsWithAction( controllerType, actionId, skipDisabledMaps: true );

			//	var action = elementMaps.FirstOrDefault( e => e.elementType == ControllerElementType.Axis )
			//		?? elementMaps.FirstOrDefault( e => e.elementType == ControllerElementType.Button );

			//	return action.elementIdentifierId;
			//}
			//else
			{
				var elementMaps = _input
					.controllers
					.maps
					.ElementMapsWithAction( controllerType, actionId, skipDisabledMaps: true );

				var action = elementMaps
					.FirstOrDefault( aem =>
					{
						if ( aem.axisContribution == Pole.Positive && axisRange == AxisRange.Positive )
						{
							return true;
						}
						else if ( aem.axisContribution == Pole.Negative && axisRange == AxisRange.Negative )
						{
							return true;
						}
						else if ( axisRange == AxisRange.Full )
						{
							return true;
						}

						return false;
					} );

				return action != null
					? action.elementIdentifierId
					: -1;
			}
		}

		private int GetTemplateElementId( int actionId )
		{
			var elementMaps = _input
				.controllers
				.maps
				.ElementMapsWithAction( ControllerType.Joystick, actionId, skipDisabledMaps: true );

			var action = elementMaps.FirstOrDefault( e => e.elementType == ControllerElementType.Axis )
				?? elementMaps.FirstOrDefault( e => e.elementType == ControllerElementType.Button );

			action
				.controllerMap
				.controller
				.Templates[0]
				.GetElementTargets( action, _tempTemplateElementTargets );

			return _tempTemplateElementTargets[0].element.id;
		}

		//public string GetGlyph( ControllerType controllerType, int actionId, AxisRange axisRange = AxisRange.Full )
		//{
		//	// these work ...
		//	//var c = _input.controllers.GetFirstControllerWithTemplate( new System.Guid( _settings.Fallback.InputGuid ) );

		//	var controller = _input.controllers.GetLastActiveController( controllerType )
		//		?? _input.controllers.Controllers.First( c => c.type == controllerType );

		//	string controllerGuid = controller != null
		//		? _controllerElementResolver.GetControllerGuid( controller )
		//		: _settings.JoystickFallback.InputGuid;

		//	int elementId = _controllerElementResolver.GetElementId( _input, controllerType, actionId );

		//	if ( !_joystickGlyphs.TryGetValue( controllerGuid, out var glyph ) )
		//	{
		//		glyph = _settings.JoystickFallback;
		//		elementId = _controllerElementResolver.Fallback.GetElementId( _input, controllerType, actionId );
		//	}

		//	return glyph.GetGlyph( elementId, axisRange );
		//}

		[System.Serializable]
		public class Settings
		{
			[BoxGroup( "Computer" )]
			public KeyboardGlyphs Keyboard;
			[BoxGroup( "Computer" )]
			public MouseGlyphs Mouse;

			[BoxGroup( "Joysticks" )]
			public ControllerGlyphs JoystickFallback;
			[BoxGroup( "Joysticks" )]
			public ControllerGlyphs[] Joysticks;
		}

		//public class ControllerElementResolver
		//{
		//	public IControllerElementProvider Fallback { get; }

		//	private readonly Dictionary<ControllerType, IControllerElementProvider> _elementProviders;

		//	public ControllerElementResolver()
		//	{
		//		Fallback = new TemplateElementProvider();

		//		_elementProviders = new Dictionary<ControllerType, IControllerElementProvider>()
		//		{
		//			{ ControllerType.Joystick, new ControllerElementProvider() },
		//			{ ControllerType.Keyboard, new KeyboardElementProvider() },
		//			{ ControllerType.Mouse, new MouseElementProvider() },
		//		};
		//	}

		//	public string GetControllerGuid( Controller controller )
		//	{
		//		return _elementProviders[controller.type]
		//			.GetControllerGuid( controller );
		//	}

		//	public int GetElementId( Player input, ControllerType controllerType, int actionId )
		//	{
		//		return _elementProviders[controllerType]
		//			.GetElementId( input, controllerType, actionId );
		//	}
		//}

		//public interface IControllerElementProvider
		//{
		//	string GetControllerGuid( Controller controller );
		//	int GetElementId( Player input, ControllerType controllerType, int actionId );
		//}

		//public class ControllerElementProvider : IControllerElementProvider
		//{
		//	public virtual string GetControllerGuid( Controller controller )
		//	{
		//		return controller.hardwareTypeGuid.ToString();
		//	}

		//	public virtual int GetElementId( Player input, ControllerType controllerType, int actionId )
		//	{
		//		var elementMaps = input
		//			.controllers
		//			.maps
		//			.ElementMapsWithAction( controllerType, actionId, skipDisabledMaps: true );

		//		var action = elementMaps.FirstOrDefault( e => e.elementType == ControllerElementType.Axis )
		//			?? elementMaps.FirstOrDefault( e => e.elementType == ControllerElementType.Button );

		//		return action.elementIdentifierId;
		//		//var actionElementMap = input
		//		//	.controllers
		//		//	.maps
		//		//	.GetFirstElementMapWithAction( controllerType, actionId, skipDisabledMaps: true );

		//		//return actionElementMap.elementIdentifierId;
		//	}
		//}

		//public class KeyboardElementProvider : ControllerElementProvider
		//{
		//	public override string GetControllerGuid( Controller controller )
		//	{
		//		return ControllerType.Keyboard.ToString();
		//	}
		//}

		//public class MouseElementProvider : ControllerElementProvider
		//{
		//	public override string GetControllerGuid( Controller controller )
		//	{
		//		return ControllerType.Mouse.ToString();
		//	}
		//}

		//public class TemplateElementProvider : ControllerElementProvider
		//{
		//	private static readonly List<ControllerTemplateElementTarget> _tempTemplateElementTargets = new List<ControllerTemplateElementTarget>();

		//	public override int GetElementId( Player input, ControllerType controllerType, int actionId )
		//	{
		//		var elementMaps = input
		//			.controllers
		//			.maps
		//			.ElementMapsWithAction( controllerType, actionId, skipDisabledMaps: true );

		//		var action = elementMaps.FirstOrDefault( e => e.elementType == ControllerElementType.Axis )
		//			?? elementMaps.FirstOrDefault( e => e.elementType == ControllerElementType.Button );

		//		action
		//			.controllerMap
		//			.controller
		//			.Templates[0]
		//			.GetElementTargets( action, _tempTemplateElementTargets );

		//		return _tempTemplateElementTargets[0].element.id;

		//		//var actionElementMap = input
		//		//	.controllers
		//		//	.maps
		//		//	.GetFirstElementMapWithAction( controllerType, actionId, skipDisabledMaps: true );

		//		//actionElementMap
		//		//	.controllerMap
		//		//	.controller
		//		//	.Templates[0]
		//		//	.GetElementTargets( actionElementMap, _tempTemplateElementTargets );

		//		//return _tempTemplateElementTargets[0].element.id;
		//	}
		//}
	}
}