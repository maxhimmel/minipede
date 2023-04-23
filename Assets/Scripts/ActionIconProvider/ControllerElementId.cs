using System;
using Rewired;
using Sirenix.OdinInspector;

namespace Minipede
{
    [HideReferenceObjectPicker]
    [System.Serializable]
    public class ControllerElementId
    {
        [NonSerialized] public string EditorControllerName;
        [NonSerialized] public string ControllerName;
        [NonSerialized] public string ControllerGuid;

        public int ElementId;
        public string ElementIdName;
        public string ElementIdPositiveName;
        public string ElementIdNegativeName;
        public ControllerElementType ElementIdType;
    }
}
