using System;
using System.Collections.Generic;
using Minipede.Gameplay.VFX;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Vfx
{
    public class VfxListener<TSignal> : 
		IInitializable,
		IDisposable
		where TSignal : IVfxSignal
    {
		private readonly SignalBus _signalBus;
		private readonly IVfxAnimator[] _vfxAnimators;

		public VfxListener( SignalBus signalBus,
			IVfxAnimator[] vfxAnimators )
		{
			_signalBus = signalBus;
			_vfxAnimators = vfxAnimators;
		}

		public void Initialize()
		{
			_signalBus.Subscribe( typeof( TSignal ), OnVfxFired );
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe( typeof( TSignal ), OnVfxFired );
		}

		private void OnVfxFired( object signal )
		{
			IVfxSignal vfxSignal = signal as IVfxSignal;
			Debug.Assert( vfxSignal != null, $"Cannot play VFX. Must be of type {nameof( IVfxSignal )}." );

			foreach ( var vfx in _vfxAnimators )
			{
				vfx.Play( vfxSignal );
			}
		}
    }

	public class VfxListener :
		IInitializable,
		IDisposable
	{
		private readonly SignalBus _signalBus;
		private readonly string _vfxId;
		private readonly List<IVfxAnimator> _vfxAnimators;

		public VfxListener( SignalBus signalBus,
			string vfxId,
			VfxAnimatorResolver animatorResolver )
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
