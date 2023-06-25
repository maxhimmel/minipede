using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.StartSequence;

namespace Minipede.Cheats
{
	public class LevelStartSequenceSkipCheat : ILevelStartSequence
	{
		private readonly ILevelStartSequence _baseController;

		public LevelStartSequenceSkipCheat( ILevelStartSequence baseController )
		{
			_baseController = baseController;
		}

		public void CreateLighthouseMushrooms()
		{
			_baseController.CreateLighthouseMushrooms();
		}

		public UniTask Play( CancellationToken cancelToken )
		{
			Dispose();

			return UniTask.CompletedTask;
		}

		public void Dispose()
		{
			_baseController.Dispose();
		}
	}
}