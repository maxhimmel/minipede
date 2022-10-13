using System;
using UnityEngine;

namespace Minipede
{
	public interface IOnDestroyedNotify
	{
		event EventHandler Destroyed;
	}

	public class OnDestroyedNotify : MonoBehaviour,
		IOnDestroyedNotify
    {
		public event EventHandler Destroyed;

		private void OnDestroy()
		{
			if ( AppHelper.IsQuitting )
			{
				return;
			}

			Destroyed?.Invoke( gameObject, EventArgs.Empty );
		}
	}
}
