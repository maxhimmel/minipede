﻿using Sirenix.OdinInspector;

namespace ControllerGlyph.Editor
{
	[HideReferenceObjectPicker]
	[System.Serializable]
	public class MouseElementId
	{
		[HideLabel]
		public ControllerElementId Element;

		[PropertySpace]
		public string PositiveName;
		public string NegativeName;
	}
}