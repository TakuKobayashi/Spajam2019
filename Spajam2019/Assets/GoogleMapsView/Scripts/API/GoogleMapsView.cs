using DeadMosquito.JniToolkit;

namespace NinevaStudios.GoogleMaps
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using MiniJSON;
	using Internal;
	using JetBrains.Annotations;
	using UnityEngine;

	/// <summary>
	/// Main class to create and show GoogleMapsView
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
	public sealed class GoogleMapsView
	{
		public const string FixApiKeyMessage = "Go to Window -> Google Maps View -> Edit Settings and provide your API key there";
		
		const string ClassName = "com.deadmosquitogames.gmaps.GoogleMapsManager";
		const int VisibilityAndroidVisible = 0;
		const int VisibilityAndroidGone = 8;

		bool _wasDismissed;
		readonly GoogleMapsOptions _options;

		AndroidJavaObject _ajo;
		IntPtr _mapPtr = IntPtr.Zero;

		public IntPtr MapPtr
		{
			get { return _mapPtr; }
		}

		public AndroidJavaObject GoogleMapAJO
		{
			get { return _ajo.CallAJO("getMap"); }
		}

		public AndroidJavaObject GoogleMapViewAJO
		{
			get { return _ajo.CallAJO("getMapView"); }
		}

		public event Action OnOrientationChange;

		/// <summary>
		/// Whether the map is displayed. Use this property to hide/show the view. All the state is saved
		/// </summary>
		[PublicAPI]
		public bool IsVisible
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

				CheckIfDismissed();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewIsVisible(_mapPtr);
#endif

#pragma warning disable 0162
				return _ajo.CallBool("isVisible");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfDismissed();

				if (GoogleMapUtils.IsAndroid)
				{
					_ajo.MainThreadCallNonBlocking("setVisible", value);
				}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSetVisible(_mapPtr, value);
#endif
			}
		}

		public bool CachedVisibilityHack { get; set; }

		/// <summary>
		/// Gets the current position of the camera.
		/// 
		/// The <see cref="CameraPosition"/> returned is a snapshot of the current position, and will not automatically update when the camera moves.
		/// </summary>
		[PublicAPI]
		public CameraPosition CameraPosition
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return null;
				}

				CheckIfDismissed();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				var json = _googleMapsViewGetCameraPosition(_mapPtr);
				return CameraPosition.FromJson(json);
#endif

#pragma warning disable 0162
				var ajo = GetValueIfAndroid<AndroidJavaObject>("getCameraPosition");
				return CameraPosition.FromAJO(ajo);
#pragma warning restore 0162
			}
		}

		/// <summary>
		/// Gets/sets the map type
		/// </summary>
		[PublicAPI]
		public GoogleMapType MapType
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return GoogleMapType.None;
				}

				CheckIfDismissed();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return (GoogleMapType) _googleMapsViewGetMapType(_mapPtr);
#endif

#pragma warning disable 0162
				return (GoogleMapType) GetValueIfAndroid<int>("getMapType");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfDismissed();

				SetValueIfAndroid("setMapType", (int) value);

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSetMapType(_mapPtr, (int) value);
#endif
			}
		}

		/// <summary>
		/// Gets the user interface settings for the map.
		/// </summary>
		[PublicAPI]
		public UiSettings UiSettings
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return new UiSettings();
				}

				return GoogleMapUtils.IsAndroid
					? new UiSettings(GoogleMapAJO.MainThreadCallAJO("getUiSettings"))
					: new UiSettings(_mapPtr);
			}
		}

		/// <summary>
		/// Gets the user location if it can be retrieved.
		/// </summary>
		/// <value>The location of the user.</value>
		[PublicAPI]
		public Location Location
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return null;
				}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				var locationJson = _googleMapsViewGetMyLocation(_mapPtr);
				return Location.FromJson(locationJson);
#endif

#pragma warning disable 0162
				var locationAJO = GoogleMapAJO.MainThreadCallAJO("getMyLocation");
				return Location.FromAJO(locationAJO);
#pragma warning restore 0162
			}
		}

		/// <summary>
		/// While enabled and the location is available, the my-location layer continuously draws an indication of a user's current location and bearing, and displays UI controls that allow a user to interact with their location (for example, to enable or disable camera tracking of their location and bearing).
		/// In order to use the my-location-layer feature you need to request permission for either ACCESS_COARSE_LOCATION or ACCESS_FINE_LOCATION unless you have set a custom location source.
		/// </summary>
		[PublicAPI]
		public bool IsMyLocationEnabled
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewGetMyLocationEnabled(_mapPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<bool>("isMyLocationEnabled");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				SetValueIfAndroid("setMyLocationEnabled", value);

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSetMyLocationEnabled(_mapPtr, value);
#endif
			}
		}

		/// <summary>
		/// Checks whether the map is drawing traffic data. This is subject to the availability of traffic data.
		/// Changing this property turns the traffic layer on or off.
		/// </summary>
		[PublicAPI]
		public bool IsTrafficEnabled
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewIsTrafficEnabled(_mapPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<bool>("isTrafficEnabled");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				SetValueIfAndroid("setTrafficEnabled", value);

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSetTrafficEnabled(_mapPtr, value);
#endif
			}
		}

		[PublicAPI]
		public GoogleMapsView([CanBeNull] GoogleMapsOptions options)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			_options = options ?? new GoogleMapsOptions();

			if (GoogleMapUtils.IsAndroid)
			{
				JniToolkitUtils.RunOnUiThread(() => { _ajo = new AndroidJavaObject(ClassName, JniToolkitUtils.Activity); });
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			if (GoogleMapsViewSettings.IosApiKey == GoogleMapsViewSettings.IOS_KEY_PLACEHOLDER || string.IsNullOrEmpty(GoogleMapsViewSettings.IosApiKey))
			{
				Debug.LogError("The provided API key is incorrect. " + FixApiKeyMessage);
			}
			
			_googleMapsViewInit(GoogleMapsViewSettings.IosApiKey);
			_mapPtr = _createGoogleMapsView();
#endif
		}

		/// <summary>
		/// This method is for providing API key when running on iOS. MUST be called before creating <see cref="GoogleMapsView"/>
		/// </summary>
		/// <param name="apiKey"></param>
		/// <exception cref="ArgumentNullException"></exception>
		[PublicAPI]
		[Obsolete(FixApiKeyMessage)]
		public static void SetIosApiKey([NotNull] string apiKey)
		{
			if (apiKey == null)
			{
				throw new ArgumentNullException("apiKey");
			}

			if (GoogleMapUtils.IsNotIosRuntime)
			{
				return;
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewInit(apiKey);
#endif
		}

		/// <summary>
		/// Set view position and size
		/// </summary>
		/// <param name="rect">New view position and size</param>
		[PublicAPI]
		public void SetRect(Rect rect)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				JniToolkitUtils.RunOnUiThread(() => _ajo.Call("setRect",
					(int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
//			rect = HackForIphone6Plus(rect);

			if (_mapPtr.IsNonZero())
			{
				_googleMapsViewSetRect(_mapPtr, (int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height);
			}
#endif
		}

		/// <summary>
		/// Show the view on the screen.
		/// </summary>
		/// <param name="rect">Rect representing position on the screen.</param>
		/// <param name="onMapReady">Optional callback executed when maps is ready.</param>
		[PublicAPI]
		public void Show(Rect rect, [CanBeNull] Action onMapReady = null)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				JniToolkitUtils.RunOnUiThread(() =>
				{
					_ajo.Call("show",
						(int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height,
						_options.AJO, new OnMapReadyCallbackProxy(onMapReady));
					GoogleMapsSceneHelper.Instance.Register(this);
				});
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
//			rect = HackForIphone6Plus(rect);

			if (_mapPtr.IsNonZero())
			{
				var options = Json.Serialize(_options.ToDictionary());
				_googleMapsViewShow(_mapPtr, (int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height, options);
				if (onMapReady != null)
				{
					onMapReady();
				}

				GoogleMapsSceneHelper.Instance.Register(this);
			}
#endif
		}

		static Rect HackForIphone6Plus(Rect rect)
		{
			var iPhonePlusVert = (Screen.width == 1080 && Screen.height == 1920) || (Screen.width == 1242 && Screen.height == 2208);
			var iPhonePlusHoriz = (Screen.width == 1920 && Screen.height == 1080) || (Screen.width == 2208 && Screen.height == 1242);

			if (iPhonePlusVert || iPhonePlusHoriz)
			{
				const float downsampleRatio = 1.15f;
				rect = new Rect(rect.position * downsampleRatio, rect.size * downsampleRatio);
			}

			return rect;
		}

		/// <summary>
		/// Dismisses this view.
		/// </summary>
		[PublicAPI]
		public void Dismiss()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (_wasDismissed)
			{
				Debug.LogError("Calling Dismiss() on GoogleMapsView twice. This view was alredy dismissed");
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				if (!_ajo.IsJavaNull())
				{
					_ajo.Call("dismiss");
					_ajo.Dispose();
				}

				_options.Dispose();
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			if (_mapPtr.IsNonZero())
			{
				_googleMapsViewRemove(_mapPtr);
			}
#endif
			GoogleMapsSceneHelper.Instance.Unregister(this);

			_wasDismissed = true;
		}

		/// <summary>
		/// Sets padding on the map.
		/// This method allows you to define a visible region on the map, to signal to the map that portions of the map around the edges may be obscured, by setting padding on each of the four edges of the map. Map functions will be adapted to the padding. For example, the zoom controls, compass, copyright notices and Google logo will be moved to fit inside the defined region, camera movements will be relative to the center of the visible region, etc.
		/// </summary>
		/// <param name="left">The number of pixels of padding to be added on the left of the map.</param>
		/// <param name="top">The number of pixels of padding to be added on the top of the map.</param>
		/// <param name="right">The number of pixels of padding to be added on the right of the map.</param>
		/// <param name="bottom">The number of pixels of padding to be added on the bottom of the map.</param>
		[PublicAPI]
		public void SetPadding(int left, int top, int right, int bottom)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();

			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("setPadding", left, top, right, bottom);
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewSetPadding(_mapPtr, left, top, right, bottom);
#endif
		}

		/// <summary>
		/// 	Sets the styling of the base map.
		/// 
		/// 	Using the style options, you can apply custom styles to features and elements on the map.
		/// 
		/// 	For more details see https://developers.google.com/android/reference/com/google/android/gms/maps/GoogleMap.html#setMapStyle(com.google.android.gms.maps.model.MapStyleOptions) 
		/// 
		/// 	Set to <code>null</code> to clear any previous custom styling.
		/// </summary>
		/// <param name="styleJson">
		/// 	Map style in json format
		/// </param>
		/// <returns>
		/// 	<code>true</code> if the style was successfully parsed;
		/// 
		/// 	<code>false</code> if problems were detected with the style json, including, e.g. unparsable styling JSON, unrecognized feature type, unrecognized element type, or invalid styler keys.
		/// 	If the return value is <code>false</code>, the current style is left unchanged.
		/// </returns>
		[PublicAPI]
		public bool SetMapStyle([CanBeNull] string styleJson)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return false;
			}

			CheckIfDismissed();

			if (GoogleMapUtils.IsAndroid)
			{
				return GoogleMapAJO.MainThreadCall<bool>("setMapStyle", styleJson == null ? null : new AndroidJavaObject("com.google.android.gms.maps.model.MapStyleOptions", styleJson));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			return _googleMapsViewSetStyle(_mapPtr, styleJson);
#endif

#pragma warning disable 0162
			return false;
#pragma warning restore 0162
		}

		#region draw_on_map

		/// <summary>
		/// Add a circle to this map.
		/// </summary>
		/// <returns>The circle added.</returns>
		/// <param name="circleOptions">Circle options.</param>
		[PublicAPI]
		public Circle AddCircle([NotNull] CircleOptions circleOptions)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return new Circle();
			}

			CheckIfDismissed();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var options = Json.Serialize(circleOptions.ToDictionary());
			var circlePtr = _googleMapsViewAddCircle(_mapPtr, options);
			return new Circle(circlePtr, _mapPtr);
#endif

#pragma warning disable 0162
			var circleAJO = _ajo.MainThreadCallAJO("addCircle", circleOptions.ToAJO());
			return new Circle(circleAJO);
#pragma warning restore 0162
		}

		/// <summary>
		/// Adds the marker to this map.
		/// </summary>
		/// <returns>The marker added.</returns>
		/// <param name="markerOptions">Marker options.</param>
		[PublicAPI]
		public Marker AddMarker([NotNull] MarkerOptions markerOptions)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return new Marker();
			}

			CheckIfDismissed();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var options = Json.Serialize(markerOptions.ToDictionary());
			var markerPtr = _googleMapsViewAddMarker(_mapPtr, options);
			return new Marker(markerPtr, _mapPtr);
#endif

#pragma warning disable 0162
			var markerAJO = _ajo.MainThreadCallAJO("addMarker", markerOptions.ToAJO());
			return new Marker(markerAJO);
#pragma warning restore 0162
		}

		/// <summary>
		/// Adds the ground overlay to this map.
		/// </summary>
		/// <returns>The ground overlay added.</returns>
		/// <param name="overlayOptions">Ground overlay options.</param>
		[PublicAPI]
		public GroundOverlay AddGroundOverlay([NotNull] GroundOverlayOptions overlayOptions)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return new GroundOverlay();
			}

			CheckIfDismissed();

			if (overlayOptions.IsImageMissing)
			{
				Debug.LogError("Image for ground overlay is missing. You can't create ground overlay without an image. Errors ahead...");
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var options = Json.Serialize(overlayOptions.ToDictionary());
			var overlayPtr = _googleMapsViewAddGroundOverlay(_mapPtr, options);
			return new GroundOverlay(overlayPtr, _mapPtr);
#endif

#pragma warning disable 0162
			var groundOverlayAJO = _ajo.MainThreadCallAJO("addGroundOverlay", overlayOptions.ToAJO());
			return new GroundOverlay(groundOverlayAJO);
#pragma warning restore 0162
		}

		/// <summary>
		/// Adds the polyline to this map.
		/// </summary>
		/// <param name="polylineOptions">Polyline options.</param>
		/// <returns>Polyline options.</returns>
		[PublicAPI]
		public Polyline AddPolyline([NotNull] PolylineOptions polylineOptions)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return new Polyline();
			}

			CheckIfDismissed();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var options = Json.Serialize(polylineOptions.ToDictionary());
			var polylinePtr = _googleMapsViewAddPolyline(_mapPtr, options);
			return new Polyline(polylinePtr, _mapPtr);
#endif

#pragma warning disable 0162
			var polylineAJO = _ajo.MainThreadCallAJO("addPolyline", polylineOptions.ToAJO());
			return new Polyline(polylineAJO);
#pragma warning restore 0162
		}

		/// <summary>
		/// Adds the polygon to this map.
		/// </summary>
		/// <param name="polygonOptions">Polygon options.</param>
		/// <returns>Polygon options.</returns>
		[PublicAPI]
		public Polygon AddPolygon([NotNull] PolygonOptions polygonOptions)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return new Polygon();
			}

			CheckIfDismissed();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var options = Json.Serialize(polygonOptions.ToDictionary());
			var polygonPtr = _googleMapsViewAddPolygon(_mapPtr, options);
			return new Polygon(polygonPtr, _mapPtr);
#endif


#pragma warning disable 0162
			var polygonAJO = _ajo.MainThreadCallAJO("addPolygon", polygonOptions.ToAJO());
			return new Polygon(polygonAJO);
#pragma warning restore 0162
		}

		/// <summary>
		/// Adds a tile overlay to this map. See <see cref="TileOverlay"/> for more information.
		/// Note that unlike other overlays, if the map is recreated, tile overlays are not automatically restored and must be re-added manually.
		/// </summary>
		/// <param name="tileOverlayOptions">A tile-overlay options object that defines how to render the overlay.</param>
		/// <returns>The <see cref="TileOverlay"/> that was added to the map.</returns>
		[PublicAPI]
		public TileOverlay AddTileOverlay([NotNull] TileOverlayOptions tileOverlayOptions)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return new TileOverlay();
			}

			CheckIfDismissed();

			if (!tileOverlayOptions.HasTileProvider)
			{
				throw new ArgumentException("tileOverlayOptions must have tile provider");
			}


#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var options = Json.Serialize(tileOverlayOptions.ToDictionary());
			var overlayPtr = _googleMapsViewTileOverlay(_mapPtr, options);
			return new TileOverlay(overlayPtr, _mapPtr);
#endif


#pragma warning disable 0162
			var overlayAJO = _ajo.MainThreadCallAJO("addTileOverlay", tileOverlayOptions.ToAJO());
			return new TileOverlay(overlayAJO);
#pragma warning restore 0162
		}

		/// <summary>
		/// Method to create a heatmap with default settings. If you want to customize the heatmap use <see cref="HeatmapTileProvider"/>  class and <see cref="AddTileOverlay"/> method
		/// </summary>
		/// <param name="data">Points to display as a heatmap.</param>
		[PublicAPI]
		public TileOverlay AddHeatmapWithDefaultLook([NotNull] List<LatLng> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return new TileOverlay();
			}

			CheckIfDismissed();

			var heatmapTileProvider = new HeatmapTileProvider.Builder().Data(data).Build();
			var options = new TileOverlayOptions().TileProvider(heatmapTileProvider);
			return AddTileOverlay(options);
		}

		/// <summary>
		/// Removes all markers, polylines, polygons, overlays, etc from the map.
		/// </summary>
		[PublicAPI]
		public void Clear()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewClear(_mapPtr);
#endif

			if (GoogleMapUtils.IsAndroid)
			{
				_ajo.MainThreadCall("clear");
			}
		}

		#endregion

		/// <summary>
		/// Animates the movement of the camera from the current position to the position defined in the update.
		/// See <see cref="CameraUpdate"/> for a set of updates.
		/// </summary>
		/// <param name="cameraUpdate">See <see cref="CameraUpdate"/> for a set of updates.</param>
		[PublicAPI]
		public void AnimateCamera([NotNull] CameraUpdate cameraUpdate)
		{
			if (cameraUpdate == null)
			{
				throw new ArgumentNullException("cameraUpdate");
			}

			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewAnimateCamera(_mapPtr, Json.Serialize(cameraUpdate.ToDictionary()), true);
#endif

			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("animateCamera", cameraUpdate.ToAJO());
			}
		}

		/// <summary>
		/// Repositions the camera according to the instructions defined in the update. The move is instantaneous, and a subsequent <see cref="CameraPosition"/> will reflect the new position.
		/// 
		/// See <see cref="CameraUpdate"/> for a set of updates.
		/// </summary>
		/// <param name="cameraUpdate"></param>
		/// <exception cref="ArgumentNullException"></exception>
		[PublicAPI]
		public void MoveCamera([NotNull] CameraUpdate cameraUpdate)
		{
			if (cameraUpdate == null)
			{
				throw new ArgumentNullException("cameraUpdate");
			}

			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewAnimateCamera(_mapPtr, Json.Serialize(cameraUpdate.ToDictionary()), false);
#endif

			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("moveCamera", cameraUpdate.ToAJO());
			}
		}

		/// <summary>
		/// Takes snaphot of current location displayed in map view and convert it to texture
		/// </summary>
		[PublicAPI]
		public void TakeSnapshot([NotNull] Action<Texture2D> listener)
		{
			if (listener == null)
			{
				throw new ArgumentNullException("listener");
			}

			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("snapshot", new SnapshotReadyCallbackProxy(listener));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewTakeSnapshot(_mapPtr, Callbacks.ImageResultCallback, listener.GetPointer());
#endif
		}

		#region listeners

		/// <summary>
		/// Sets a callback that's invoked when a circle is clicked.
		/// </summary>
		/// <param name="listener">The callback that's invoked when a circle is clicked. To unset the callback, use null.</param>
		[PublicAPI]
		public void SetOnCircleClickListener(Action<Circle> listener)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();

			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("setOnCircleClickListener",
					listener == null ? null : new OnCircleClickListenerProxy(listener));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewSetOnCircleClickListener(Callbacks.OnCircleClickedCallback, listener.GetPointer());
#endif
		}

		/// <summary>
		/// Sets a callback that's invoked when a polyline is clicked.
		/// </summary>
		/// <param name="listener">The callback that's invoked when a polyline is clicked. To unset the callback, use null.</param>
		[PublicAPI]
		public void SetOnPolylineClickListener(Action<Polyline> listener)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();
			
			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("setOnPolylineClickListener",
					listener == null ? null : new OnPolylineClickListenerProxy(listener));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewSetOnPolylineClickListener(Callbacks.OnPolylineClickedCallback, listener.GetPointer());
#endif
		}
		
		/// <summary>
		/// Sets a callback that's invoked when a polygon is clicked.
		/// </summary>
		/// <param name="listener">The callback that's invoked when a polygon is clicked. To unset the callback, use null.</param>
		[PublicAPI]
		public void SetOnPolygonClickListener(Action<Polygon> listener)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();
			
			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("setOnPolygonClickListener",
					listener == null ? null : new OnPolygonClickListenerProxy(listener));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewSetOnPolygonClickListener(Callbacks.OnPolygonClickedCallback, listener.GetPointer());

#endif
		}

		/// <summary>
		/// Sets a callback that's invoked when a marker is clicked or tapped.
		/// </summary>
		/// <param name="listener">The callback that's invoked when a marker is clicked or tapped. To unset the callback, use null.</param>
		/// <param name="defaultClickBehaviour">
		/// true if the listener has consumed the event (i.e., the default behavior should not occur); false otherwise (i.e., the default behavior should occur). 
		/// The default behavior is for the camera to move to the marker and an info window to appear.
		/// </param>
		[PublicAPI]
		public void SetOnMarkerClickListener(Action<Marker> listener, bool defaultClickBehaviour = true)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();

			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("setOnMarkerClickListener",
					listener == null ? null : new OnMarkerClickListenerProxy(listener, defaultClickBehaviour));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewSetOnMarkerClickListener(Callbacks.OnMarkerClickedCallback, listener.GetPointer(), defaultClickBehaviour);
#endif
		}

		/// <summary>
		/// Sets a callback that's invoked when a marker info window is clicked.
		/// </summary>
		/// <param name="listener">Callback that's invoked when a marker info window is clicked</param>
		[PublicAPI]
		public void SetOnInfoWindowClickListener([CanBeNull] Action<Marker> listener)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();

			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("setOnInfoWindowClickListener",
					listener == null ? null : new OnMarkerInfoWindowClickListenerProxy(listener));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewSetOnMarkerInfoWindowClickListener(Callbacks.OnMarkerClickedCallback, listener.GetPointer());
#endif
		}

		/// <summary>
		/// Sets a callback that's invoked when the map is tapped.
		/// </summary>
		/// <param name="onMapClicked">The callback that's invoked when the map is tapped. To unset the callback, use null.</param>
		[PublicAPI]
		public void SetOnMapClickListener(Action<LatLng> onMapClicked)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();

			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("setOnMapClickListener",
					onMapClicked == null ? null : new OnMapClickListenerProxy(onMapClicked));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewSetOnMapClickListener(Callbacks.OnLocationSelectedCallback, onMapClicked.GetPointer());
#endif
		}

		/// <summary>
		/// Sets a callback that's invoked when the map is long tapped.
		/// </summary>
		/// <param name="onMapLongClicked">The callback that's invoked when the map is long tapped. To unset the callback, use null.</param>
		[PublicAPI]
		public void SetOnLongMapClickListener([CanBeNull] Action<LatLng> onMapLongClicked)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();

			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("setOnMapLongClickListener",
					onMapLongClicked == null ? null : new OnMapLongClickListenerProxy(onMapLongClicked));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewSetOnMapLongClickListener(Callbacks.OnLocationSelectedCallback, onMapLongClicked.GetPointer());
#endif
		}

		/// <summary>
		/// Sets a callback that's invoked when the camera starts moving or the reason for camera motion has changed.
		/// </summary>
		/// <param name="onCameraMoveStarted"></param>
		[PublicAPI]
		public void SetOnCameraMoveStartedListener([CanBeNull] Action<CameraMoveReason> onCameraMoveStarted)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfDismissed();

			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCall("setOnCameraMoveStartedListener",
					onCameraMoveStarted == null ? null : new setOnCameraMoveStartedListenerProxy(onCameraMoveStarted));
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			Action<int> callbackWrapper = reason =>
			{
				if (onCameraMoveStarted != null)
				{
					onCameraMoveStarted((CameraMoveReason) reason);
				}
			};
			_googleMapsViewSetOnCameraMoveStartedListener(Callbacks.ActionIntCallback, callbackWrapper.GetPointer());
#endif
		}

		#endregion

		T GetValueIfAndroid<T>(string methodName)
		{
			CheckIfDismissed();
			return GoogleMapUtils.IsAndroid ? GoogleMapAJO.MainThreadCall<T>(methodName) : default(T);
		}

		void SetValueIfAndroid(string methodName, params object[] args)
		{
			CheckIfDismissed();
			if (GoogleMapUtils.IsAndroid)
			{
				GoogleMapAJO.MainThreadCallNonBlocking(methodName, args);
			}
		}

		void CheckIfDismissed()
		{
			if (_wasDismissed)
			{
				throw new Exception(
					"Current GoogleMapsView object was already dismissed. You can no longer perform any actions on it. Errors ahead.");
			}
		}

		public override string ToString()
		{
			if (GoogleMapUtils.IsAndroid && GoogleMapAJO.IsJavaNull())
			{
				return "null";
			}

			return string.Format("[GoogleMapsView: UiSettings={0}, IsMyLocationEnabled={1}, IsVisible={2}, CameraPosition={3}, MapType={4}, IsTrafficEnabled={5}]",
				UiSettings, IsMyLocationEnabled, IsVisible, CameraPosition, MapType, IsTrafficEnabled);
		}

		public void TriggerOrientationChange()
		{
			if (OnOrientationChange != null)
			{
				OnOrientationChange();
			}
		}

		public void ImmersiveModeAndroidHack()
		{
			CheckIfDismissed();
			_ajo.Call("immersiveModeHack");
		}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern void _googleMapsViewInit(string apiKey);

		[DllImport("__Internal")]
		static extern void _googleMapsViewRemove(IntPtr mapPtr);

		[DllImport("__Internal")]
		static extern IntPtr _createGoogleMapsView();

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetRect(IntPtr mapPtr, int x, int y, int width, int height);

		[DllImport("__Internal")]
		static extern void _googleMapsViewShow(IntPtr mapPtr, int x, int y, int width, int height, string options);

		[DllImport("__Internal")]
		static extern IntPtr _googleMapsViewAddCircle(IntPtr mapPtr, string options);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewIsVisible(IntPtr mapPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetVisible(IntPtr mapPtr, bool visible);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewIsTrafficEnabled(IntPtr mapPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetTrafficEnabled(IntPtr mapPtr, bool trafficEnabled);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetPadding(IntPtr mapPtr, int left, int top, int right, int bottom);

		[DllImport("__Internal")]
		static extern int _googleMapsViewGetMapType(IntPtr mapPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetMapType(IntPtr mapPtr, int mapType);

		[DllImport("__Internal")]
		static extern string _googleMapsViewGetCameraPosition(IntPtr mapPtr);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewSetStyle(IntPtr mapPtr, string json);

		[DllImport("__Internal")]
		static extern IntPtr _googleMapsViewAddMarker(IntPtr mapPtr, string options);

		[DllImport("__Internal")]
		static extern IntPtr _googleMapsViewAddGroundOverlay(IntPtr mapPtr, string options);

		[DllImport("__Internal")]
		static extern IntPtr _googleMapsViewAddPolyline(IntPtr mapPtr, string options);

		[DllImport("__Internal")]
		static extern IntPtr _googleMapsViewAddPolygon(IntPtr mapPtr, string options);

		[DllImport("__Internal")]
		static extern IntPtr _googleMapsViewTileOverlay(IntPtr mapPtr, string options);

		[DllImport("__Internal")]
		static extern void _googleMapsViewClear(IntPtr mapPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewAnimateCamera(IntPtr mapPtr, string animateCamera, bool isAnimated);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetMyLocationEnabled(IntPtr mapPtr, bool enabled);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewGetMyLocationEnabled(IntPtr mapPtr);

		[DllImport("__Internal")]
		static extern string _googleMapsViewGetMyLocation(IntPtr mapPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewTakeSnapshot(IntPtr mapPtr, Callbacks.ImageResultDelegate callback, IntPtr callbackActionPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetOnMapClickListener(Callbacks.OnLocationSelectedDelegate callback, IntPtr callbackActionPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetOnMapLongClickListener(Callbacks.OnLocationSelectedDelegate callback, IntPtr callbackActionPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetOnMarkerClickListener(Callbacks.OnItemClickedDelegate callback, IntPtr callbackActionPtr, bool defaultClickBehaviour);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetOnMarkerInfoWindowClickListener(Callbacks.OnItemClickedDelegate callback, IntPtr callbackActionPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetOnCircleClickListener(Callbacks.OnItemClickedDelegate callback, IntPtr callbackActionPtr);
		
		[DllImport("__Internal")]
		static extern void _googleMapsViewSetOnPolylineClickListener(Callbacks.OnItemClickedDelegate callback, IntPtr callbackActionPtr);
		
		[DllImport("__Internal")]
		static extern void _googleMapsViewSetOnPolygonClickListener(Callbacks.OnItemClickedDelegate callback, IntPtr callbackActionPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSetOnCameraMoveStartedListener(Callbacks.ActionIntCallbackDelegate callback, IntPtr callbackActionPtr);
#endif
	}
}