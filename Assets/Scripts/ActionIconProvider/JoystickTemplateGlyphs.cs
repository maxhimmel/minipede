﻿using System.Collections.Generic;
using Rewired;
using UnityEngine;

namespace Minipede
{
	[CreateAssetMenu( menuName = "Tools/Input/Joystick Template Glyph Asset" )]
	public class JoystickTemplateGlyphs : JoystickGlyphs
	{
		private readonly List<ControllerTemplateElementTarget> _tempTemplateElementTargets = new List<ControllerTemplateElementTarget>();

		protected override int GetElementId( Request request )
		{
			var elementMaps = request.Input
				.controllers
				.maps
				.ElementMapsWithAction( _controllerType, request.ActionId, skipDisabledMaps: true );

			var action = base.SelectElement( elementMaps, request.ElementType, request.AxisRange );

			action
				.controllerMap
				.controller
				.Templates[0]
				.GetElementTargets( action, _tempTemplateElementTargets );

			return _tempTemplateElementTargets[0].element.id;
		}
	}
}