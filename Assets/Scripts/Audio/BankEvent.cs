using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Audio
{
	[System.Serializable]
	[InlineProperty]
	public class BankEvent : IComparable<BankEvent>
	{
		[HorizontalGroup, HideLabel]
		public string EventName;
		[HorizontalGroup, HideLabel]
		public AudioClip Clip;

		public int CompareTo( BankEvent other )
		{
			if ( string.IsNullOrEmpty( EventName ) )
			{
				return 1;
			}

			return EventName.CompareTo( other.EventName );
		}
	}
}