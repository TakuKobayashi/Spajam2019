using DeadMosquito.JniToolkit;

namespace NinevaStudios.GoogleMaps
{
	using System.Collections.Generic;
	using Internal;
	using MiniJSON;
	using UnityEngine;
	using JetBrains.Annotations;

	/// <summary>
	/// An immutable class representing a latitude/longitude aligned rectangle.
	/// </summary>
	[PublicAPI]
	public sealed class LatLngBounds
	{
		const string SWLat = "latLngBoundsSouthWestLat";
		const string SWLng = "latLngBoundsSouthWestLng";
		const string NELat = "latLngBoundsNorthEastLat";
		const string NELng = "latLngBoundsNorthEastLng";
		
		public static readonly LatLngBounds Zero = new LatLngBounds(LatLng.Zero, LatLng.Zero);
		
		readonly LatLng _southwest;
		readonly LatLng _northeast;

		/// <summary>
		/// Creates a new bounds based on a southwest and a northeast corner.
		/// </summary>
		/// <param name="southwest">Southwest corner.</param>
		/// <param name="northeast">Northeast corner.</param>
		public LatLngBounds(LatLng southwest, LatLng northeast)
		{
			_southwest = southwest;
			_northeast = northeast;
		}

		public AndroidJavaObject ToAJO()
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return null;
			}

			return new AndroidJavaObject("com.google.android.gms.maps.model.LatLngBounds", _southwest.ToAJO(),
				_northeast.ToAJO());
		}

		public static LatLngBounds FromAJO(AndroidJavaObject ajo)
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return new LatLngBounds(LatLng.Zero, LatLng.Zero);
			}

			var northeast = LatLng.FromAJO(ajo.GetAJO("northeast"));
			var southwest = LatLng.FromAJO(ajo.GetAJO("southwest"));
			return new LatLngBounds(southwest, northeast);
		}

		public override string ToString()
		{
			return string.Format("[LatLngBounds SW: {0}, NE: {1}]", _southwest, _northeast);
		}

		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>();
			result[SWLat] = _southwest.Latitude;
			result[SWLng] = _southwest.Longitude;
			result[NELat] = _northeast.Latitude;
			result[NELng] = _northeast.Longitude;
			return result;
		}

		public static LatLngBounds FromJson(string boundsJson)
		{
			var dic = Json.Deserialize(boundsJson) as Dictionary<string, object>;
			var latSW = dic.GetDouble(SWLat);
			var lngSW = dic.GetDouble(SWLng);
			var latNE = dic.GetDouble(NELat);
			var lngNE = dic.GetDouble(NELng);
			
			return new LatLngBounds(new LatLng(latSW, lngSW), new LatLng(latNE, lngNE));
		}
	}
}