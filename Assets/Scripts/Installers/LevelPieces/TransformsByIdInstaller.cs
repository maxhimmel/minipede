using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class TransformsByIdInstaller : MonoInstaller
    {
        [SerializeField] private string[] _identifiers;

		public override void InstallBindings()
		{
			foreach ( var id in _identifiers )
			{
				Container.Bind<Transform>()
					.WithId( id )
					.FromNewComponentOnNewGameObject()
					.WithGameObjectName( id )
					.UnderTransform( this.transform )
					.AsCached();
			}
		}
	}
}
