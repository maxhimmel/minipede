using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Installers;

namespace Minipede.Cheats
{
	public class LevelGeneratorCheat : LevelGenerator
	{
		public LevelGeneratorCheat( LevelGenerationInstaller.Level settings, 
			LevelGraph levelGraph, 
			MushroomProvider mushroomProvider,
			LevelGenerator baseGenerator ) 
			: base( settings, levelGraph, mushroomProvider )
		{
		}

		public override UniTask GenerateLevel()
		{
			return UniTask.CompletedTask;
		}
	}
}