using Zenject;

namespace Minipede.Gameplay.Player
{
	public class CraftingController : ILateTickable
	{
		private readonly CraftingModel _model;

		public CraftingController( CraftingModel model )
		{
			_model = model;
		}

		public void LateTick()
		{
			_model.SelectBeacon();
			_model.UpdateCraftingTimer();
		}
	}
}