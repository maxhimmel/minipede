using UnityEngine;

namespace Minipede.Utility
{
	public class SimpleRotationConstraint : MonoBehaviour
    {
		[SerializeField] private Vector3 _worldEuler;

		private void LateUpdate()
		{
			transform.rotation = Quaternion.Euler( _worldEuler );
		}
	}
}
