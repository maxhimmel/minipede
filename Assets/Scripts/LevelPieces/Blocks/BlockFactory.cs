using Minipede.Installers;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public partial class Block
	{
		public enum Type
		{
			Regular,
			Poison,
			Flower
		}

		public class Factory : PlaceholderFactory<Type, Vector2, Quaternion, Block> { }

		public class CustomFactory : IFactory<Type, Vector2, Quaternion, Block>
		{
			private readonly DiContainer _container;
			private readonly IBlockProvider _blockProvider;
			private readonly GameplaySettings.Level _settings;

			public CustomFactory( DiContainer container,
				IBlockProvider blockProvider,
				GameplaySettings.Level settings )
			{
				_container = container;
				_blockProvider = blockProvider;
				_settings = settings;
			}

			public Block Create( Type type, Vector2 position, Quaternion rotation )
			{
				var prefab = _blockProvider.GetAsset( type );
				var newBlock = _container.InstantiatePrefabForComponent<Block>( 
					prefab,
					position,
					rotation,
					null 
				);

				newBlock.name = prefab.name;
				newBlock.transform.localScale = new Vector3( 
					_settings.Graph.Size.x, 
					_settings.Graph.Size.y, 
					1 
				);

				return newBlock;
			}
		}
	}
}
