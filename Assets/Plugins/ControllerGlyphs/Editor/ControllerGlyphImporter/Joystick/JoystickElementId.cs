using System;
using ControllerGlyph.Core;
using Sirenix.OdinInspector;

namespace ControllerGlyph.Editor
{
	[HideReferenceObjectPicker]
    [System.Serializable]
    public class JoystickElementId
    {
        [NonSerialized]
        public ControllerIdentifier ControllerIdentifier;

        [HideLabel]
        public ControllerElementId Element;

        [PropertySpace]
        public string PositiveName;
        public string NegativeName;
    }
}
