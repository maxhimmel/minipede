using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Minipede.Editor
{
	public class InputGlyphWindow : OdinEditorWindow
	{
		[HideLabel, TabGroup( "Controllers", Icon = SdfIconType.Controller )]
		public ControllerGlyphImporter ControllerImporter = new ControllerGlyphImporter();

		[HideLabel, TabGroup( "Keyboard", Icon = SdfIconType.Keyboard )]
		public KeyboardGlyphImporter KeyboardImporter = new KeyboardGlyphImporter();

		[HideLabel, TabGroup( "Mouse", Icon = SdfIconType.Mouse2 )]
		public MouseGlyphImporter MouseImporter = new MouseGlyphImporter();

		[MenuItem( "Tools/Input/Glyph Importer" )]
		private static void OpenWindow()
		{
			var window = GetWindow<InputGlyphWindow>();
			window.titleContent = new GUIContent( "Glyph Importer" );
			window.Show();
		}

		protected override void Initialize()
		{
			base.Initialize();

			ControllerImporter.Ingest();
			KeyboardImporter.Ingest();
			MouseImporter.Ingest();
		}
	}
}