using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class PrefabInstallerFactory : MonoInstaller
    {
		private readonly object _identifier = new object();

		[SerializeField] private GameObject[] _prefabs;

		public override void InstallBindings()
		{
			foreach ( var prefab in _prefabs )
			{
				Container.Bind<GameObject>()
					.WithId( _identifier )
					.FromMethod( () => Container.InstantiatePrefab( prefab, new GameObjectCreationParameters() 
					{ 
						Name = prefab.name 
					} ) )
					.AsCached()
					.NonLazy();
			}
        }
	}
}
