using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
    public class EnterExitTriggerFilter<TComponent> : MonoBehaviour
		where TComponent : Component
    {
		public event System.Action<TComponent> Entered;
		public event System.Action<TComponent> Exited;

		private LayerMask _filter;
		private Collider2D _collider;

		[Inject]
		public void Construct( LayerMask filter,
			Collider2D collider )
		{
			_filter = filter;
			_collider = collider;
		}

		public void Activate()
		{
			_collider.enabled = true;
		}

		public void Deactivate()
		{
			_collider.enabled = false;
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			if ( TryValidate( collision, out var other ) )
			{
				OnEntered( other );
			}
		}

		protected virtual void OnEntered( TComponent other )
		{
			Entered?.Invoke( other );
		}

		private void OnTriggerExit2D( Collider2D collision )
		{
			if ( TryValidate( collision, out var other ) )
			{
				OnExited( other );
			}
		}

		protected virtual void OnExited( TComponent other )
		{
			Exited?.Invoke( other );
		}

		private bool TryValidate( Collider2D other, out TComponent result )
		{
			var otherBody = other.attachedRigidbody;
			if ( otherBody == null )
			{
				result = null;
				return false;
			}

			if ( !otherBody.gameObject.CanCollide( _filter ) )
			{
				result = null;
				return false;
			}

			result = otherBody.GetComponent<TComponent>();
			return true;
		}
	}
}
