﻿using System.Collections.Generic;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Enemies;

namespace Minipede.Gameplay.Player
{
	public class CameraGroupDangerReaction : IDangerWarningReaction
	{
		private readonly TargetGroupAttachment.Settings _settings;
		private readonly TargetGroupAttachment.Factory _groupAttachmentFactory;
		private readonly Dictionary<EnemyController, TargetGroupAttachment> _attachments;

		public CameraGroupDangerReaction( TargetGroupAttachment.Settings settings,
			TargetGroupAttachment.Factory groupAttachmentFactory )
		{
			_settings = settings;
			_groupAttachmentFactory = groupAttachmentFactory;

			_attachments = new Dictionary<EnemyController, TargetGroupAttachment>();
		}

		public void React( EnemyController enemy )
		{
			_attachments.Add(
				enemy,
				_groupAttachmentFactory.Create( _settings, enemy.transform )
			);
		}

		public void Neglect( EnemyController enemy )
		{
			if ( _attachments.Remove( enemy, out var attachment ) )
			{
				attachment.Deactivate( canDispose: true );
			}
		}
	}
}