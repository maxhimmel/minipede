using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
	public class ResourceTypeInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private ResourceType _resource;

		public override void InstallBindings()
		{
			Container.BindInstance( _resource )
				.AsCached();
		}
	}
}