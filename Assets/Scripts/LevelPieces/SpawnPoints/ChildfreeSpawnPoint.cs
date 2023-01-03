using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
    public class ChildfreeSpawnPoint : MonoBehaviour,
        IOrientation
    {
        public Vector2 Position { 
            get { 
                return transform.position; 
            } 
            set { 
                transform.position = value; 
            } 
        }

		public Quaternion Rotation { 
            get { 
                return transform.rotation; 
            } 
            set { 
                transform.rotation = value; 
            } 
        }
        
        public Transform Parent { 
            get { return null; } 
            set { } 
        }
    }
}
