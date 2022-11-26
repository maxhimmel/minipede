using System;
using System.Collections.Generic;
using Zenject;

namespace Minipede.Gameplay.Fx
{
	public class FxListener :
		IInitializable,
		IDisposable
	{
		private readonly SignalBus _signalBus;
		private readonly string _vfxId;
		private readonly List<IFxAnimator> _vfxAnimators;

		public FxListener( SignalBus signalBus,
			string vfxId,
			FxAnimatorResolver animatorResolver )
		{
			_signalBus = signalBus;
			_vfxId = vfxId;
			_vfxAnimators = animatorResolver.GetAnimators( vfxId );
		}

		public void Initialize()
		{
			_signalBus.SubscribeId<FxSignal>( _vfxId, OnVfxFired );
		}

		public void Dispose()
		{
			_signalBus.UnsubscribeId<FxSignal>( _vfxId, OnVfxFired );
		}

		private void OnVfxFired( FxSignal vfxSignal )
		{
			foreach ( var vfx in _vfxAnimators )
			{
				vfx.Play( vfxSignal );
			}
		}
	}
}
