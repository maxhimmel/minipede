using Minipede.Gameplay.Treasures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class GemCountItem<TTreasure> : MonoBehaviour
		where TTreasure : Treasure
    {
		[SerializeField] private string _format = "x{0}";

		[Space]
        [SerializeField] private Image _border;
        [SerializeField] private TMP_Text _count;
		[SerializeField] private Button _button;

		private GemCountModel _model;

		[Inject]
		public void Construct( GemCountModel model )
		{
			_model = model;
		}

		private void OnEnable()
		{
			_model.Listen( OnCollectedTreasure );
		}

		private void OnDisable()
		{
			_model.Cleanup();
		}

		private void OnCollectedTreasure( CollectedTreasureSignal signal )
		{
			if ( signal.TreasureType == typeof( TTreasure ) )
			{
				_count.text = string.Format( _format, signal.TotalAmount );
			}
		}
	}

	public class GemCountModel : Model<CollectedTreasureSignal>
	{
		public GemCountModel( SignalBus signalBus ) : base( signalBus )
		{
		}
	}
}
