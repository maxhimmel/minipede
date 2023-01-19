using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
    public class WaveItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Slider _progression;

        public void SetIcon( Color color )
		{
            _icon.color = color;
		}

        public void SetProgression( float progress )
		{
            _progression.value = progress;
		}

        public void SetProgressActive( bool isActive )
		{
            _progression.gameObject.SetActive( isActive );
		}
    }
}
