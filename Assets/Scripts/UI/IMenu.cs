using Zenject;

namespace Minipede.Gameplay.UI
{
	public interface IMenu : IInitializable
	{
		string Title { get; }

		void Show();
		void Hide();
	}
}