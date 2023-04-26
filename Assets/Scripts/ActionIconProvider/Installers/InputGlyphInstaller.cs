using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "Tools/Input/Glyph Installer" )]
	public class InputGlyphInstaller : ScriptableObjectInstaller
	{
		[HideLabel]
		[SerializeField] private InputGlyphBus.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<InputGlyphBus>()
				.AsSingle()
				.WithArguments( _settings );
		}
	}
}
