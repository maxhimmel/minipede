using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class CompositeTimedSpawner : ITimedSpawner
	{
		private readonly ITimedSpawner[] _childSpawners;

		public CompositeTimedSpawner( Settings settings,
			DiContainer container )
		{
			_childSpawners = new ITimedSpawner[settings.Children.Length];
			for ( int idx = 0; idx < settings.Children.Length; ++idx )
			{
				var child = settings.Children[idx];
				_childSpawners[idx] = container.Instantiate( child.SpawnerType, new object[1] { child } ) as ITimedSpawner;
			}
		}

		public void Initialize()
		{
			foreach ( var child in _childSpawners )
			{
				child.Initialize();
			}
		}

		public void Dispose()
		{
			foreach ( var child in _childSpawners )
			{
				child.Dispose();
			}
		}

		public void Play()
		{
			foreach ( var child in _childSpawners )
			{
				child.Play();
			}
		}

		public void Tick()
		{
			foreach ( var child in _childSpawners )
			{
				child.Tick();
			}
		}

		public UniTaskVoid HandleSpawning( CancellationToken cancelToken )
		{
			foreach ( var child in _childSpawners )
			{
				child.HandleSpawning( cancelToken ).Forget();
			}

			return new UniTaskVoid();
		}

		public void Stop()
		{
			foreach ( var child in _childSpawners )
			{
				child.Stop();
			}
		}

		[System.Serializable]
		public class Settings : ITimedSpawner.ISettings
		{
			public System.Type SpawnerType => typeof( CompositeTimedSpawner );
			public string Name => _name;

			[SerializeField] private string _name;

			[ListDrawerSettings( ListElementLabelName = "Name", DraggableItems = false )]
			[SerializeReference] public ITimedSpawner.ISettings[] Children = new ITimedSpawner.ISettings[0];
		}
	}
}