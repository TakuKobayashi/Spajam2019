using DeadMosquito.JniToolkit;

namespace NinevaStudios.GoogleMaps
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Internal;
	using JetBrains.Annotations;
	using MiniJSON;
	using UnityEngine;

	/// <summary>
	/// https://developers.google.com/android/reference/com/google/android/gms/maps/model/LatLng
	/// 
	/// An immutable class representing a pair of latitude and longitude coordinates, stored as degrees.
	/// </summary>
	[PublicAPI]
	public sealed class LatLng
	{
		public static readonly LatLng Zero = new LatLng(0, 0);

		readonly double _latitude;
		readonly double _longitude;

		/// <summary>
		/// Latitude
		/// </summary>
		[PublicAPI]
		public double Latitude
		{
			get { return _latitude; }
		}

		/// <summary>
		/// Longitude
		/// </summary>
		[PublicAPI]
		public double Longitude
		{
			get { return _longitude; }
		}

		/// <summary>
		/// Constructs a <see cref="LatLng"/> with the other <see cref="LatLng"/> values
		/// </summary>
		/// <param name="latLng"><see cref="LatLng"/> to make a copy of</param>
		[PublicAPI]
		public LatLng(LatLng latLng)
		{
			_latitude = latLng._latitude;
			_longitude = latLng._longitude;
		}

		/// <summary>
		/// Constructs a <see cref="LatLng"/> with the given latitude and longitude, measured in degrees.
		/// </summary>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		[PublicAPI]
		public LatLng(double latitude, double longitude)
		{
			_latitude = Math.Max(-90.0D, Math.Min(90.0D, latitude));

			if (-180.0D <= longitude && longitude < 180.0D)
			{
				_longitude = longitude;
			}
			else
			{
				_longitude = ((longitude - 180.0D) % 360.0D + 360.0D) % 360.0D - 180.0D;
			}
		}

		public override string ToString()
		{
			return new StringBuilder(60).Append("lat/lng: (").Append(_latitude).Append(",").Append(_longitude).Append(")")
				.ToString();
		}

		[NotNull]
		public static LatLng FromAJO(AndroidJavaObject ajo)
		{
			return GoogleMapUtils.IsAndroid
				? new LatLng(ajo.GetDouble("latitude"), ajo.GetDouble("longitude"))
				: new LatLng(0, 0);
		}

		public AndroidJavaObject ToAJO()
		{
			return GoogleMapUtils.IsAndroid
				? new AndroidJavaObject("com.google.android.gms.maps.model.LatLng", _latitude, _longitude)
				: null;
		}

		public static LatLng FromJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return null;
			}
			
			var dic = Json.Deserialize(json) as Dictionary<string, object>;
			return FromDictionary(dic);
		}

		static LatLng FromDictionary(Dictionary<string, object> dic)
		{
			var lat = dic.GetDouble("lat");
			var lng = dic.GetDouble("lng");
			return new LatLng(lat, lng);
		}

		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>();
			result["lat"] = _latitude;
			result["lng"] = _longitude;
			return result;
		}

		public static List<object> ToJsonList(List<LatLng> coords)
		{
			var result = new List<object>();
			foreach (var latLng in coords)
			{
				result.Add(latLng.ToDictionary());
			}
			return result;
		}
		
		public static List<object> ToJsonList(List<List<LatLng>> holes)
		{
			var result = new List<object>();

			foreach (var hole in holes)
			{
				var holeCoordsList = new List<object>();
				foreach (var coord in hole)
				{
					holeCoordsList.Add(coord.ToDictionary());
				}
				result.Add(holeCoordsList);
			}

			return result;
		}

		public static List<LatLng> ListFromJson(string pointsJson)
		{
			var points = Json.Deserialize(pointsJson) as List<object>;
			var result = new List<LatLng>();
			foreach (var point in points)
			{
				result.Add(FromDictionary(point as Dictionary<string, object>));
			}
			return result;
		}

		public static List<List<LatLng>> HolesListFromJson(string holesJson)
		{
			Debug.Log(holesJson);
			var holes = Json.Deserialize(holesJson) as List<object>;
			var result = new List<List<LatLng>>();
			foreach (var hole in holes)
			{
				var coordsOfHole = new List<LatLng>();
				foreach (var coord in (List<object>) hole)
				{
					var coordDic = coord as Dictionary<string, object>;
					coordsOfHole.Add(FromDictionary(coordDic));
				}
				result.Add(coordsOfHole);
			}

			return result;
		}
	}
}