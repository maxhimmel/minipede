using Rewired;

namespace Minipede.Utility
{
	public class ControllerModel
	{
		public event System.Action<ControllerModel> Changed;

		public ControllerType ControllerType { get; private set; }

		public ControllerModel( Settings settings )
		{
			ControllerType = settings.DefaultController;
		}

		public void SetControllerType( ControllerType type )
		{
			if ( ControllerType != type )
			{
				ControllerType = type;
				Changed?.Invoke( this );
			}
		}

		[System.Serializable]
		public class Settings
		{
			public ControllerType DefaultController;
		}
	}
}