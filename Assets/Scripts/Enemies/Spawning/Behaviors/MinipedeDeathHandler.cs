using System.Collections.Generic;
using UnityEngine;

namespace Minipede.Gameplay.Enemies
{
    public class MinipedeDeathHandler : MonoBehaviour
	{
		private readonly Dictionary<MinipedeController, HashSet<MinipedeController>> _deaths = new Dictionary<MinipedeController, HashSet<MinipedeController>>();
		private readonly SortedList<int, MinipedeController> _headIndices = new SortedList<int, MinipedeController>();

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
						newHead.SetSegments( head._segments.GetRange( firstSegmentIndex, segmentCount ) );
					}

					int deadSegmentIndex = headIndex - 1;
					var deadSegment = deadSegmentIndex < 0
						? head
						: head._segments[deadSegmentIndex];

					newHead.StartSplitHeadBehavior( deadSegment.Body.position );
				}

				// Remove which segments the head should stop listening to ...
				if ( head.HasSegments )
				{
					head.RemoveSegments( topMostDeadSegmentIndex );
				}

				_headIndices.Clear();
			}

			_deaths.Clear();
		}
	}
}
