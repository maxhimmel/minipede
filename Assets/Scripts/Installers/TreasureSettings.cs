using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "Treasure" )]
    public class TreasureSettings : ScriptableObjectInstaller
    {
        [SerializeField] private Treasure.Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInstance( _settings );
		}
	}
}
