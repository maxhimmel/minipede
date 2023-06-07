using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Utility
{
	public class DashedLineScroller : MonoBehaviour
    {
        [SerializeField] private Line _line;

		[BoxGroup]
		[SerializeField] private float _speed;
		[BoxGroup]
		[SerializeField] private bool _useUnscaledTime;

		private void Update()
		{
			float delta = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
			_line.DashOffset += _speed * delta;
		}
	}
}
