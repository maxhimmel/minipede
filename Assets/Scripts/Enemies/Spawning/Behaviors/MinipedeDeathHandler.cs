using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class MinipedeDeathHandler : MonoBehaviour
	{
		private LevelGraph _levelGraph;
		private readonly Dictionary<MinipedeController, HashSet<MinipedeController>> _deaths = new Dictionary<MinipedeController, HashSet<MinipedeController>>();
		private readonly SortedList<int, MinipedeController> _headIndices = new SortedList<int, MinipedeController>();

		//private CancellationTokenSource _cancelSource;

		[Inject]
		public void Construct( LevelGraph levelGraph )
		{
			_levelGraph = levelGraph;
		}

		public void Add( MinipedeController head, MinipedeController segment )
		{
			Add( head );
			_deaths[head].Add( segment );
		}

		public void Add( MinipedeController head )
		{
			if ( !_deaths.ContainsKey( head ) )
			{
				_deaths.Add( head, new HashSet<MinipedeController>() );
			}
		}

		//private void Start()
		//{
		//	_cancelSource = AppHelper.CreateLinkedCTS( this.GetCancellationTokenOnDestroy() );

		//	HandleDeaths()
		//		.Cancellable( _cancelSource.Token )
		//		.Forget();
		//}

		//private async UniTask HandleDeaths()
		//{
		//	while ( !_cancelSource.IsCancellationRequested )
		//	{
		//		while ( _deaths.Count <= 0 )
		//		{
		//			await UniTask.Yield( PlayerLoopTiming.LastFixedUpdate, _cancelSource.Token );
		//		}

		//		foreach ( var head in _deaths.Keys )
		//		{
		//			var deadSegments = _deaths[head];
		//			int topMostDeadSegmentIndex = int.MaxValue;

		//			// The head died ...
		//			if ( !head.IsAlive && head.HasSegments )
		//			{
		//				topMostDeadSegmentIndex = 0;

		//				for ( int idx = 0; idx < head._segments.Count; ++idx )
		//				{
		//					var segment = head._segments[idx];
		//					if ( segment.IsAlive )
		//					{
		//						_headIndices.Add( idx, segment );
		//						break;
		//					}
		//				}
		//			}

		//			// The head's segments died ...
		//			if ( deadSegments.Count > 0 )
		//			{
		//				foreach ( var segment in deadSegments )
		//				{
		//					int deadSegmentIndex = head._segments.FindIndex( otherSegment => otherSegment == segment );
		//					if ( deadSegmentIndex < 0 )
		//					{
		//						throw new System.NotSupportedException( "Head is listening to a segment's death it's not tracking." );
		//					}

		//					topMostDeadSegmentIndex = Mathf.Min( deadSegmentIndex, topMostDeadSegmentIndex );

		//					int headIndex = deadSegmentIndex + 1;
		//					if ( headIndex < head._segments.Count )
		//					{
		//						var potentialNewHead = head._segments[headIndex];
		//						if ( potentialNewHead.IsAlive )
		//						{
		//							_headIndices.TryAdd( headIndex, potentialNewHead );
		//						}
		//					}
		//				}
		//			}

		//			// Split minipede based on new head indices ...
		//			for ( int idx = 0; idx < _headIndices.Keys.Count; ++idx )
		//			{
		//				int headIndex = _headIndices.Keys[idx];
		//				MinipedeController newHead = _headIndices[headIndex];

		//				int firstSegmentIndex = headIndex + 1;
		//				if ( firstSegmentIndex < head._segments.Count )
		//				{
		//					int lastSegmentIndex = head._segments.Count - 1;
		//					if ( idx + 1 < _headIndices.Keys.Count )
		//					{
		//						lastSegmentIndex = _headIndices.Keys[idx + 1];
		//						lastSegmentIndex -= 2; // Subtracting one for the head and one for the previous dead segment.
		//					}

		//					int segmentCount = lastSegmentIndex - firstSegmentIndex + 1; // Adding one to include the end segment.
		//					if ( segmentCount < 0 )
		//					{
		//						Debug.LogError( "WTF" );
		//					}

		//					newHead.SetSegments( head._segments.GetRange( firstSegmentIndex, segmentCount ) );
		//				}


		//				int deadSegmentIndex = headIndex - 1;
		//				var deadSegment = deadSegmentIndex < 0
		//					? head
		//					: head._segments[deadSegmentIndex];


		//				//// TODO: We should probably be setting the column and row directions based on the previous segment - not THIS head.
		//				////var newHeadVelocity = newHead._motor.Velocity; 
		//				//	// Using this velocity is probably not dependable if the movement has been stopped.
		//				//newHead._columnDir.x = newHead._columnDir.x;
		//				//newHead._rowDir.y = newHead._rowDir.y;

		//				newHead._motor.StopMoving();
		//				newHead.UpdateSegmentMovement();

		//				newHead.StateStack.Insert( 0, "New Head" );

		//				var cellCoord = _levelGraph.WorldPosToCellCoord( deadSegment.Body.position );
		//				newHead._motor.SetDestination( cellCoord, newHead.OnDestroyCancelToken )
		//					.ContinueWith( newHead.UpdateRowTransition )
		//					.Forget();
		//			}

		//			// Remove which segments the head should stop listening to ...
		//			if ( head.HasSegments )
		//			{
		//				for ( int idx = head._segments.Count - 1; idx >= topMostDeadSegmentIndex; --idx )
		//				{
		//					var segment = head._segments[idx];
		//					segment.Died -= head.OnSegmentDied;

		//					head._segments.RemoveAt( idx );
		//				}
		//			}

		//			_headIndices.Clear();
		//		}

		//		_deaths.Clear();
		//	}
		//}

		private void LateUpdate()
		{
			if ( _deaths.Count <= 0 )
			{
				return;
			}

			foreach ( var head in _deaths.Keys )
			{
				var deadSegments = _deaths[head];
				int topMostDeadSegmentIndex = int.MaxValue;

				head.StateStack.Insert( 0, $"Handling Death\n" +
					$"Alive? ({head.IsAlive})\n" +
					$"Segments ({head._segments?.Count})" );

				// The head died ...
				if ( !head.IsAlive && head.HasSegments )
				{
					topMostDeadSegmentIndex = 0;

					for ( int idx = 0; idx < head._segments.Count; ++idx )
					{
						var segment = head._segments[idx];
						if ( segment.IsAlive )
						{
							_headIndices.Add( idx, segment );
							break;
						}
					}
				}

				// The head's segments died ...
				if ( deadSegments.Count > 0 )
				{
					foreach ( var segment in deadSegments )
					{
						int deadSegmentIndex = head._segments.FindIndex( otherSegment => otherSegment == segment );
						if ( deadSegmentIndex < 0 )
						{
							throw new System.NotSupportedException( "Head is listening to a segment's death it's not tracking." );
						}

						topMostDeadSegmentIndex = Mathf.Min( deadSegmentIndex, topMostDeadSegmentIndex );

						int headIndex = deadSegmentIndex + 1;
						if ( headIndex < head._segments.Count )
						{
							var potentialNewHead = head._segments[headIndex];
							if ( potentialNewHead.IsAlive )
							{
								_headIndices.TryAdd( headIndex, potentialNewHead );
							}
						}
					}
				}

				// Split minipede based on new head indices ...
				for ( int idx = 0; idx < _headIndices.Keys.Count; ++idx )
				{
					int headIndex = _headIndices.Keys[idx];
					MinipedeController newHead = _headIndices[headIndex];

					int firstSegmentIndex = headIndex + 1;
					if ( firstSegmentIndex < head._segments.Count )
					{
						int lastSegmentIndex = head._segments.Count - 1;
						if ( idx + 1 < _headIndices.Keys.Count )
						{
							lastSegmentIndex = _headIndices.Keys[idx + 1];
							lastSegmentIndex -= 2; // Subtracting one for the head and one for the previous dead segment.
						}

						int segmentCount = lastSegmentIndex - firstSegmentIndex + 1; // Adding one to include the end segment.
						if ( segmentCount < 0 )
						{
							Debug.LogError( "WTF" );
						}

						newHead.SetSegments( head._segments.GetRange( firstSegmentIndex, segmentCount ) );
					}


					int deadSegmentIndex = headIndex - 1;
					var deadSegment = deadSegmentIndex < 0
						? head
						: head._segments[deadSegmentIndex];


					//// TODO: We should probably be setting the column and row directions based on the previous segment - not THIS head.
					////var newHeadVelocity = newHead._motor.Velocity; 
					//	// Using this velocity is probably not dependable if the movement has been stopped.
					//newHead._columnDir.x = newHead._columnDir.x;
					//newHead._rowDir.y = newHead._rowDir.y;

					newHead._motor.StopMoving();
					newHead.UpdateSegmentMovement();

					newHead.StateStack.Insert( 0, $"New Head\n" +
						$"Segments ({newHead._segments?.Count})" );

					var cellCoord = _levelGraph.WorldPosToCellCoord( deadSegment.Body.position );
					newHead._motor.SetDestination( cellCoord, newHead.OnDestroyCancelToken )
						.ContinueWith( newHead.UpdateRowTransition )
						.Forget();
				}

				// Remove which segments the head should stop listening to ...
				if ( head.HasSegments )
				{
					for ( int idx = head._segments.Count - 1; idx >= topMostDeadSegmentIndex; --idx )
					{
						var segment = head._segments[idx];
						segment.Died -= head.OnSegmentDied;

						head._segments.RemoveAt( idx );
					}
				}

				head.StateStack.Insert( 0, $"Death Handled\n" +
					$"Top Dead Segment ({topMostDeadSegmentIndex})\n" +
					$"Segments ({head._segments?.Count})" );

				_headIndices.Clear();
			}

			_deaths.Clear();
		}
	}
}
