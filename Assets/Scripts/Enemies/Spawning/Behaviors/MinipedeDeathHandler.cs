using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class MinipedeDeathHandler : ILateTickable
	{
		private readonly HashSet<MinipedeController> _heads = new HashSet<MinipedeController>();
		private readonly List<SplitProcessor> _splitProcesses = new List<SplitProcessor>();

		public void Add( MinipedeController head )
		{
			_heads.Add( head );
		}

		public void LateTick()
		{
			if ( _heads.Count <= 0 )
			{
				return;
			}

			foreach ( var head in _heads )
			{
				bool isPrevDead = !head.IsAlive;
				int topMostDeadSegmentIndex = head.IsAlive
					? int.MaxValue
					: 0;

				SplitProcessor currentSplit = null;

				for ( int idx = 0; idx < head.Segments.Count; ++idx )
				{
					var segment = head.Segments[idx];

					if ( !segment.IsAlive )
					{
						isPrevDead = true;
						topMostDeadSegmentIndex = Mathf.Min( topMostDeadSegmentIndex, idx );
					}
					else
					{
						if ( isPrevDead )
						{
							var prevDeadSegment = idx - 1 >= 0
								? head.Segments[idx - 1]
								: head;

							currentSplit = new SplitProcessor( segment, prevDeadSegment, new List<MinipedeController>() );
							_splitProcesses.Add( currentSplit );
						}
						else if ( currentSplit != null )
						{
							currentSplit.Segments.Add( segment );
						}

						isPrevDead = false;
					}
				}

				head.RemoveSegments( topMostDeadSegmentIndex );

				for ( int idx = _splitProcesses.Count - 1; idx >= 0; --idx )
				{
					var process = _splitProcesses[idx];
					process.Split();

					_splitProcesses.RemoveAt( idx );
				}
			}

			_heads.Clear();
		}

		private class SplitProcessor
		{
			public MinipedeController Head { get; }
			public MinipedeController PrevDeadSegment { get; }
			public List<MinipedeController> Segments { get; }

			public SplitProcessor( MinipedeController head, 
				MinipedeController prevDeadSegment, 
				List<MinipedeController> segments )
			{
				Head = head;
				PrevDeadSegment = prevDeadSegment;
				Segments = segments;
			}

			public void Split()
			{
				Head.SetSegments( Segments );
				Head.StartSplitHeadBehavior( PrevDeadSegment.Body.position );
			}
		}
	}
}
