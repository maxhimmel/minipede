using System;
using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class KnockbackProcessor : IFireEndProcessor
	{
		private readonly Settings _settings;
		private readonly IPushable _pushable;
		private readonly Transform _owner;

		public KnockbackProcessor( Settings settings,
			IPushable pushable,
			Transform owner )
		{
			_settings = settings;
			_pushable = pushable;
			_owner = owner;
		}

		public void FireEnding()
		{
			_pushable.Push( -_owner.up * _settings.Impulse );
		}

		[System.Serializable]
		public class Settings : IGunModule
		{
			public Type ModuleType => typeof( KnockbackProcessor );

			public float Impulse;
		}
	}
}