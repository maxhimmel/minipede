using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class CompositeInstaller : MonoInstaller
    {
		[SerializeField] private MonoInstaller[] _installers;

		public override void InstallBindings()
		{
			foreach ( var installer in _installers )
			{
				Container.Inject( installer );
				installer.InstallBindings();
			}
		}
	}
}
