using System;
using Sirenix.OdinInspector;

namespace Minipede.Editor
{
	[HideReferenceObjectPicker]
    [System.Serializable]
    public class ControllerElementId
    {
        [NonSerialized]
        public ControllerIdentifier ControllerIdentifier;

        [HideLabel]
        public InputElementId Element;

        [PropertySpace]
        public string PositiveName;
        public string NegativeName;
    }
}
