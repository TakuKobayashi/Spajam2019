// ReSharper disable InconsistentNaming

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using JetBrains.Annotations;
	using UnityEngine;

	public sealed class setOnCameraMoveStartedListenerProxy : AndroidJavaProxy
	{
		readonly Action<CameraMoveReason> _onCameraMoveStarted;

		public setOnCameraMoveStartedListenerProxy(Action<CameraMoveReason> onCameraMoveStarted)
			: base("com.google.android.gms.maps.GoogleMap$OnCameraMoveStartedListener")
		{
			_onCameraMoveStarted = onCameraMoveStarted;
		}

		[UsedImplicitly]
		public void onCameraMoveStarted(int reason)
		{
			GoogleMapsSceneHelper.Queue(() => _onCameraMoveStarted((CameraMoveReason) reason));
		}
	}
}