using System.Threading;

namespace Minipede.Gameplay.Player
{
    public interface IPlayerLifetimeHandler
    {
        CancellationToken PlayerDiedCancelToken { get; }
    }
}
