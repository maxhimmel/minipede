using ControllerGlyph;
using Zenject;

namespace Minipede.Utility
{
	public class ControllerGlyphInitializer : IInitializable
	{
		private readonly PlayerInputResolver _inputResolver;
		private readonly ControllerGlyphBus _glyphBus;

		public ControllerGlyphInitializer( PlayerInputResolver inputResolver,
			ControllerGlyphBus glyphBus )
		{
			_inputResolver = inputResolver;
			_glyphBus = glyphBus;
		}

		public void Initialize()
		{
			_glyphBus.Initialize( _inputResolver.GetInput() );
		}
	}
}