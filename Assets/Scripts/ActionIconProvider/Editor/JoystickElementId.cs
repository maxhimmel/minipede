using System;
using Sirenix.OdinInspector;

namespace Minipede.Editor
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
