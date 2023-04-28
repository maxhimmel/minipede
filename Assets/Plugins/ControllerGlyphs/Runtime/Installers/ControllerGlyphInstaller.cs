using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ControllerGlyph.Installers
{
	[CreateAssetMenu( menuName = "Tools/Input/Glyph Installer" )]
	public class ControllerGlyphInstaller : ScriptableObjectInstaller
	{
		[HideLabel]
		[SerializeField] private ControllerGlyphBus.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<ControllerGlyphBus>()
				.AsSingle()
				.WithArguments( _settings );
		}
	}
}
