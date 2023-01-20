using Cysharp.Threading.Tasks;

namespace Minipede.Gameplay.Waves
{
    public interface IWave
    {
        string Id { get; }
        bool IsRunning { get; }

        UniTask<Result> Play();

        void Interrupt();

        public enum Result
        {
            Success,
            Interrupted,
            Restart
        }
    }
}
