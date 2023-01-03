using System.Collections.Generic;
using System.Linq;

namespace Minipede.Gameplay.Player
{
	public class InteractionHandlerBus<TOwner>
	{
		private readonly Dictionary<System.Type, IInteractionHandler<TOwner>> _handlers;

		public InteractionHandlerBus( List<IInteractionHandler<TOwner>> handlers )
		{
			_handlers = handlers.ToDictionary( handler => handler.InteractType );
		}

		public bool Handle<TInteractable>( TOwner owner, TInteractable interactable )
		{
			if ( !_handlers.TryGetValue( interactable.GetType(), out var handler ) )
			{
				throw new System.NotSupportedException( 
					$"No handler matches <b>{interactable.GetType()}</b> interact type.\n" +
					$"Check where the <b>{nameof( InteractionHandlerBus<TOwner> )}</b> is being installed." 
				);
			}

			return handler.Handle( owner, interactable );
		}
	}

	public interface IInteractionHandler<TOwner>
	{
		System.Type InteractType { get; }

		bool Handle<TInteractable>( TOwner owner, TInteractable interactable );
	}

	public abstract class InteractionHandler<TOwner, TInteractable> : IInteractionHandler<TOwner>
	{
		public System.Type InteractType => typeof( TInteractable );

		public bool Handle<TBaseInteractable>( TOwner owner, TBaseInteractable interactable )
		{
			if ( interactable is TInteractable castedInteractable )
			{
				return Handle( owner, castedInteractable );
			}

			return false;
		}

		protected abstract bool Handle( TOwner owner, TInteractable interactable );
	}
}