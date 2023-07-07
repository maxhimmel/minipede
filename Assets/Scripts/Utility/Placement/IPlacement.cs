using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
	public interface IPlacement
	{
		void Move( Vector2 position );

		public class Factory : PlaceholderFactory<Object, IPlacement> { }
	}
}