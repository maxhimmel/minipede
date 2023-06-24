using System.Threading;
using Cysharp.Threading.Tasks;

namespace Minipede.Gameplay.StartSequence
{
	public interface ILevelStartSequence
	{
		void CreateLighthouseMushrooms();

		UniTask Play( CancellationToken cancelToken );

		void Dispose();
	}
}