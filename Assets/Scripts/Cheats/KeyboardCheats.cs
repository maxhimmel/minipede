using Minipede.Gameplay;
using Minipede.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Cheats
{
	public class KeyboardCheats : ITickable
	{
		private readonly Action[] _settings;

		public KeyboardCheats( Action[] settings )
		{
			_settings = settings;
		}

		public void Tick()
		{
			foreach ( var cheat in _settings )
			{
				if ( Input.GetKeyDown( cheat.Key ) )
				{
					cheat.PerformAction();
				}
			}
		}

		[System.Serializable]
		public abstract class Action
		{
			public KeyCode Key;

			[EnableIf( "@UnityEngine.Application.isPlaying" )]
			[Button( ButtonSizes.Large, Name = "@GetActionName()" )]
			public abstract void PerformAction();

			protected abstract string GetActionName();
		}
	}

	[System.Serializable]
	public class KillShipAction : KeyboardCheats.Action
	{
		public override void PerformAction()
		{
			var ship = GameObject.FindObjectOfType<Ship>();
			if ( ship != null )
			{
				ship.TakeDamage( ship.transform, ship.transform, new KillInvoker.Settings() );
			}
		}

		protected override string GetActionName()
		{
			return "Kill Ship";
		}
	}
}
