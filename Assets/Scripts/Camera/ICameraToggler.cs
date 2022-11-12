﻿namespace Minipede.Gameplay.Camera
{
	public interface ICameraToggler<TToggler>
	{
		void Activate( TToggler sender );
		void Deactivate( TToggler sender );
	}
}