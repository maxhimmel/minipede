using System.Collections.Generic;
using System.Linq;
using Rewired;

namespace Minipede
{
	public class InputGlyphBus
	{
		private readonly Settings _settings;
		private readonly Player _input;
		private readonly Dictionary<string, InputGlyphs> _glyphs;
		private readonly ControllerElementResolver _controllerElementResolver;

		public InputGlyphBus( Settings settings,
			Player input )
		{
			_settings = settings;
			_input = input;
			
			_glyphs = settings.Glyphs.ToDictionary( g => g.InputGuid );
			_controllerElementResolver = new ControllerElementResolver();
		}

		public string GetGlyph( int actionId, AxisRange axisRange = AxisRange.Full )
		{
			var controller = _input.controllers.GetLastActiveController() ?? _input.controllers.Keyboard;

			string controllerGuid = _controllerElementResolver.GetControllerGuid( controller );
			int elementId = _controllerElementResolver.GetElementId( _input, controller, actionId );

			if ( !_glyphs.TryGetValue( controllerGuid, out var glyph ) )
			{
				glyph = _settings.Fallback;
				elementId = _controllerElementResolver.Fallback.GetElementId( _input, controller, actionId );
			}

			return glyph.GetGlyph( elementId, axisRange );//actionElementMap.axisRange );
		}

		[System.Serializable]
		public class Settings
		{
			public InputGlyphs Fallback;
			public InputGlyphs[] Glyphs;
		}

		public class ControllerElementResolver
		{
			public IControllerElementProvider Fallback { get; }

			private readonly Dictionary<ControllerType, IControllerElementProvider> _elementProviders;

			public ControllerElementResolver()
			{
				Fallback = new TemplateElementProvider();

				_elementProviders = new Dictionary<ControllerType, IControllerElementProvider>()
				{
					{ ControllerType.Joystick, new ControllerElementProvider() },
					{ ControllerType.Keyboard, new KeyboardElementProvider() },
					{ ControllerType.Mouse, new MouseElementProvider() },
				};
			}

			public string GetControllerGuid( Controller controller )
			{
				return _elementProviders[controller.type]
					.GetControllerGuid( controller );
			}

			public int GetElementId( Player input, Controller controller, int actionId )
			{
				return _elementProviders[controller.type]
					.GetElementId( input, controller, actionId );
			}
		}

		public interface IControllerElementProvider
		{
			string GetControllerGuid( Controller controller );
			int GetElementId( Player input, Controller controller, int actionId );
		}

		public class KeyboardElementProvider : IControllerElementProvider
		{
			public string GetControllerGuid( Controller controller )
			{
				return ControllerType.Keyboard.ToString();
			}

			public int GetElementId( Player input, Controller controller, int actionId )
			{
				var actionElementMap = input
					.controllers
					.maps
					.GetFirstElementMapWithAction( controller, actionId, skipDisabledMaps: true );

				return actionElementMap.elementIdentifierId;
			}
		}

		public class MouseElementProvider : IControllerElementProvider
		{
			public string GetControllerGuid( Controller controller )
			{
				return ControllerType.Mouse.ToString();
			}

			public int GetElementId( Player input, Controller controller, int actionId )
			{
				var actionElementMap = input
					.controllers
					.maps
					.GetFirstElementMapWithAction( controller, actionId, skipDisabledMaps: true );

				return actionElementMap.elementIdentifierId;
			}
		}

		public class ControllerElementProvider : IControllerElementProvider
		{
			public string GetControllerGuid( Controller controller )
			{
				return controller.hardwareTypeGuid.ToString();
			}

			public int GetElementId( Player input, Controller controller, int actionId )
			{
				var actionElementMap = input
					.controllers
					.maps
					.GetFirstElementMapWithAction( controller, actionId, skipDisabledMaps: true );

				return actionElementMap.elementIdentifierId;
			}
		}

		public class TemplateElementProvider : IControllerElementProvider
		{
			private static readonly List<ControllerTemplateElementTarget> _tempTemplateElementTargets = new List<ControllerTemplateElementTarget>();

			public string GetControllerGuid( Controller controller )
			{
				return controller.hardwareTypeGuid.ToString();
			}

			public int GetElementId( Player input, Controller controller, int actionId )
			{
				var actionElementMap = input
					.controllers
					.maps
					.GetFirstElementMapWithAction( controller, actionId, skipDisabledMaps: true );

				controller.Templates[0].GetElementTargets( actionElementMap, _tempTemplateElementTargets );

				return _tempTemplateElementTargets[0].element.id;
			}
		}
	}
}