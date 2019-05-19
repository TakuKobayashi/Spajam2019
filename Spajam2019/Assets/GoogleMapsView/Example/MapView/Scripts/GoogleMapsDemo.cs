#if UNITY_2018_3_OR_NEWER && PLATFORM_ANDROID
	using UnityEngine.Android;
#endif

using System.Collections.Generic;
using JetBrains.Annotations;
using NinevaStudios.GoogleMaps.Internal;
using UnityEngine;
using UnityEngine.UI;
using NinevaStudios.GoogleMaps;

public class GoogleMapsDemo : MonoBehaviour
{
	#region map_options

	public Texture2D icon;

	public TextAsset customStyleJson;
	public TextAsset heatmapDataJsonPoliceStations;
	public TextAsset heatmapDataJsonMedicare;
	public TextAsset markerClusterData;

	[Header("Bounds")] public Toggle boundsToggle;

	[Header("Options Toggles")] public Toggle ambientToggle;

	public Toggle compassToggle;
	public Toggle liteModeToggle;
	public Toggle mapToolbarToggle;
	public Toggle rotateGesturesToggle;
	public Toggle scrollGesturesToggle;
	public Toggle tiltGesturesToggle;
	public Toggle zoomGesturesToggle;
	public Toggle zoomControlsToggle;

	[Header("Map Type")] public Dropdown mapType;

	[Header("Min/Max Zoom")] public InputField minZoom;

	public InputField maxZoom;

	[Header("Camera Position")] public Slider camPosLat;

	public Slider camPosLng;
	public Slider camPosZoom;
	public Slider camPosTilt;
	public Slider camPosBearing;

	[Header("Camera Labels")] public Text camPosLatText;

	public Text camPosLngText;
	public Text camPosZoomText;
	public Text camPosTiltText;
	public Text camPosBearingText;

	[Header("Bound South-West")] [Range(-90, 90)] [SerializeField]
	float _boundsSouthWestPosLat;

	[Range(-180, 180)] [SerializeField] float _boundsSouthWestPosLng;

	[Header("Bound North-East")] [Range(-90, 90)] [SerializeField]
	float _boundsNorthEastPosLat;

	[Range(-180, 180)] [SerializeField] float _boundsNorthEastPosLng;

	#endregion

	#region circle_options

	[Header("Circle Center")] public Slider circleLat;

	public Slider circleLng;
	public Slider circleStokeWidth;
	public Slider circleRadius;
	public Toggle circleVisibilityToggle;
	public Toggle IsCameraMovementAnimatedToggle;

	#endregion

	#region marker_options

	[Header("Marker Center")] public Slider markerLat;

	public Slider markerLng;

	public Text isInfoWindowEnabledText;

	#endregion

	#region snapshot

	[Header("Map snapshot")] public Image snapshotImage;

	#endregion

	public RectTransform rect;

	GoogleMapsView _map;
	Circle _circle;
	Marker _marker;
	GroundOverlay _groundOverlay;
	Polyline _polyline;
	Polygon _coloradoPolygon;
	TileOverlay _heatmap;
	ClusterManager _clusterManager;

	void Awake()
	{
		SetupEvents();
		SetInitialOptionsValues();
	}

	void Start()
	{
		// Show the map when the demo starts
		OnShow();
	}

	void SetInitialOptionsValues()
	{
		mapType.value = (int) GoogleMapType.Normal;

		// Camera position
		camPosLat.value = 52.0779648f;
		camPosLng.value = 4.334087f;
		camPosZoom.value = 2f;
		camPosTilt.value = 1f;
		camPosBearing.value = 0f;

		// Zoom constraints
		minZoom.text = "1.0";
		maxZoom.text = "20.0";
	}

	void SetupEvents()
	{
		// Camera position
		camPosLat.onValueChanged.AddListener(newValue => { camPosLatText.text = string.Format("Lat:{0}", newValue); });
		camPosLng.onValueChanged.AddListener(newValue => { camPosLngText.text = string.Format("Lng:{0}", newValue); });
		camPosZoom.onValueChanged.AddListener(
			newValue => { camPosZoomText.text = string.Format("Zoom:{0}", newValue); });
		camPosTilt.onValueChanged.AddListener(
			newValue => { camPosTiltText.text = string.Format("Tilt:{0}", newValue); });
		camPosBearing.onValueChanged.AddListener(newValue => { camPosBearingText.text = string.Format("Bearing:{0}", newValue); });
	}

	/// <summary>
	/// Shows the <see cref="GoogleMapsView"/>
	/// </summary>
	[UsedImplicitly]
	public void OnShow()
	{
		Dismiss();

		_map = new GoogleMapsView(CreateMapViewOptions());
		_map.Show(RectTransformToScreenSpace(rect), OnMapReady);
	}

	void OnMapReady()
	{
		Debug.Log("Map is ready: " + _map);
		_map.SetPadding(0, 0, 0, 0);

		var isStyleUpdateSuccess = _map.SetMapStyle(customStyleJson.text);
		if (isStyleUpdateSuccess)
		{
			Debug.Log("Successfully updated style of the map");
		}
		else
		{
			Debug.LogError("Setting new map style failed.");
		}

#if UNITY_2018_3_OR_NEWER && PLATFORM_ANDROID
			if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
			{
				Permission.RequestUserPermission(Permission.FineLocation);
			}
#endif

		// UNCOMMENT if testing with showing users location. DON'T FORGET MANIFEST LOCATION PERMISSION!!!
		_map.IsMyLocationEnabled = true;
		_map.UiSettings.IsMyLocationButtonEnabled = true;
		_map.OnOrientationChange += () => { _map.SetRect(RectTransformToScreenSpace(rect)); };

		_map.SetOnCameraMoveStartedListener(moveReason => Debug.Log("Camera move started because: " + moveReason));
		_map.SetOnCircleClickListener(circle => Debug.Log("Circle clicked: " + circle));
		_map.SetOnPolylineClickListener(polyline => Debug.Log("Polyline clicked: " + polyline));
		_map.SetOnPolygonClickListener(polygon => Debug.Log("Polygon clicked: " + polygon));
		_map.SetOnMarkerClickListener(marker => Debug.Log("Marker clicked: " + marker), false);
		_map.SetOnInfoWindowClickListener(marker => Debug.Log("Marker info window clicked: " + marker));
		_map.SetOnMapClickListener(point =>
		{
			Debug.Log("Map clicked: " + point);
			_map.AddMarker(DemoUtils.RandomColorMarkerOptions(point));
		});
		_map.SetOnLongMapClickListener(point =>
		{
			Debug.Log("Map long clicked: " + point);
			_map.AddCircle(DemoUtils.RandomColorCircleOptions(point));
		});

		// When the map is ready we can start drawing on it
		AddCircle();
		AddMarker();
		AddGroundOverlay();
		AddPolyline();
		AddPolygon();
		AddHeatmap();
		AddMarkerCluster();

		AddOtherExampleOverlays();
	}

	void AddOtherExampleOverlays()
	{
		// New Ark overlay image
		_map.AddGroundOverlay(DemoUtils.CreateNewArkGroundOverlay());

		// Talkeetna
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_map.AddGroundOverlay(DemoUtils.CreateTalkeetnaGroundOverlayForIos());
		}

		// Medicare
		var medicareHeatmapOptions = DemoUtils.CreateDemoHeatMap(heatmapDataJsonMedicare.text);
		_map.AddTileOverlay(medicareHeatmapOptions);

		// Berlin marker
		_map.AddMarker(DemoUtils.CreateTexture2DMarkerOptions(icon));
	}

	void AddCircle()
	{
		_circle = _map.AddCircle(DemoUtils.CreateInitialCircleOptions());
		CenterCamera(_circle.Center);
	}

	void AddMarker()
	{
		_marker = _map.AddMarker(DemoUtils.CreateInitialMarkerOptions());
		CenterCamera(_marker.Position);
	}

	void AddGroundOverlay()
	{
		_groundOverlay = _map.AddGroundOverlay(DemoUtils.CreateInitialGroundOverlayOptions());
		CenterCamera(_groundOverlay.Position);
	}

	void AddPolyline()
	{
		_polyline = _map.AddPolyline(DemoUtils.CreateInitialPolylineOptions());
		CenterCamera(new LatLng(10, 10));
	}

	void AddPolygon()
	{
		_coloradoPolygon = _map.AddPolygon(DemoUtils.CreateColoradoStatePolygonOptions());
		CenterCamera(DemoUtils.ColoradoBorders[0]);
	}

	void AddHeatmap()
	{
		var policeStations = DemoUtils.DeserializeLocations(heatmapDataJsonPoliceStations.text);
		_heatmap = _map.AddHeatmapWithDefaultLook(policeStations);
		CenterCamera(DemoUtils.MelbourneLatLng);
	}

	void AddMarkerCluster()
	{
		// Check London to see the cluster
		_clusterManager = new ClusterManager(_map);
		AddClusterItems();
	}

	void AddClusterItems()
	{
		if (_clusterManager != null)
		{
			_clusterManager.AddItems(DemoUtils.DeserializeClusterMarkers(markerClusterData.text));
		}
	}

	GoogleMapsOptions CreateMapViewOptions()
	{
		var options = new GoogleMapsOptions();

		options.MapType((GoogleMapType) mapType.value);

		// Camera position
		options.Camera(CameraPosition);

		// Bounds
		if (boundsToggle.isOn)
		{
			options.LatLngBoundsForCameraTarget(Bounds);
		}

		options.AmbientEnabled(ambientToggle.isOn);
		options.CompassEnabled(compassToggle.isOn);
		options.LiteMode(liteModeToggle.isOn);
		options.MapToolbarEnabled(mapToolbarToggle.isOn);
		options.RotateGesturesEnabled(rotateGesturesToggle.isOn);
		options.ScrollGesturesEnabled(scrollGesturesToggle.isOn);
		options.TiltGesturesEnabled(tiltGesturesToggle.isOn);
		options.ZoomGesturesEnabled(zoomGesturesToggle.isOn);
		options.ZoomControlsEnabled(zoomControlsToggle.isOn);

		options.MinZoomPreference(float.Parse(minZoom.text));
		options.MaxZoomPreference(float.Parse(maxZoom.text));

		return options;
	}

	LatLngBounds Bounds
	{
		get
		{
			var southWest = new LatLng(_boundsSouthWestPosLat, _boundsSouthWestPosLng);
			var northEast = new LatLng(_boundsNorthEastPosLat, _boundsNorthEastPosLng);
			return new LatLngBounds(southWest, northEast);
		}
	}

	CameraPosition CameraPosition
	{
		get
		{
			return new CameraPosition(
				new LatLng(camPosLat.value, camPosLng.value),
				camPosZoom.value,
				camPosTilt.value,
				camPosBearing.value);
		}
	}

	#region update_buttons_click

	[UsedImplicitly]
	public void OnUpdateCircleButtonClick()
	{
		if (_circle == null)
		{
			AddCircle();
			return;
		}

		Debug.Log("Current circle: " + _circle + ", updating properties...");
		UpdateCircleProperties();
	}

	[UsedImplicitly]
	public void OnRemoveCircleClick()
	{
		if (_circle != null)
		{
			_circle.Remove();
			_circle = null;
			Debug.Log("Circle was removed.");
		}
	}

	[UsedImplicitly]
	public void OnUpdateMarkerButtonClick()
	{
		if (_marker == null)
		{
			AddMarker();
			return;
		}

		Debug.Log("Current marker: " + _marker + ", updating properties...");
		UpdateMarkerProperties();
	}

	[UsedImplicitly]
	public void OnShowMarkerInfoWindow()
	{
		if (_marker == null)
		{
			return;
		}

		_marker.ShowInfoWindow();
		isInfoWindowEnabledText.text = _marker.IsInfoWindowShown.ToString();
	}

	[UsedImplicitly]
	public void OnHideMarkerInfoWindow()
	{
		if (_marker == null)
		{
			return;
		}

		_marker.HideInfoWindow();
		isInfoWindowEnabledText.text = _marker.IsInfoWindowShown.ToString();
	}

	[UsedImplicitly]
	public void OnRemoveMarkerClick()
	{
		if (_marker == null)
		{
			return;
		}

		_marker.Remove();
		_marker = null;
		Debug.Log("Marker was removed.");
	}

	[UsedImplicitly]
	public void OnUpdateGroundOverlayClick()
	{
		if (_groundOverlay == null)
		{
			AddGroundOverlay();
			return;
		}

		Debug.Log("Current ground overlay: " + _groundOverlay + ", updating properties...");
		UpdateGroundOverlayProperties();
	}

	[UsedImplicitly]
	public void OnRemoveGroundOverlayClick()
	{
		if (_groundOverlay == null)
		{
			return;
		}

		_groundOverlay.Remove();
		_groundOverlay = null;
		Debug.Log("Ground overlay was removed.");
	}

	[UsedImplicitly]
	public void OnUpdateHeatmapOverlayClick()
	{
		if (_heatmap == null)
		{
			AddHeatmap();
			return;
		}

		Debug.Log("Current heatmap overlay: " + _heatmap + ", updating properties...");
		UpdateHeatmapProperties();
	}

	[UsedImplicitly]
	public void OnRemoveHeatmapClick()
	{
		if (_heatmap == null)
		{
			return;
		}

		_heatmap.Remove();
		_heatmap = null;
		Debug.Log("Tile overlay was removed.");
	}

	[UsedImplicitly]
	public void OnUpdatePolylineClick()
	{
		if (_polyline == null)
		{
			AddPolyline();
			return;
		}

		Debug.Log("Current polyline: " + _polyline + ", updating properties...");
		UpdatePolylineProperties();
	}

	[UsedImplicitly]
	public void OnRemovePolylineClick()
	{
		if (_polyline == null)
		{
			return;
		}

		_polyline.Remove();
		_polyline = null;
		Debug.Log("Polyline was removed.");
	}

	[UsedImplicitly]
	public void OnUpdatePolygonClick()
	{
		if (_coloradoPolygon == null)
		{
			AddPolygon();
			return;
		}

		Debug.Log("Current polygon: " + _coloradoPolygon + ", updating properties...");
		UpdatePolygonProperties();
	}

	[UsedImplicitly]
	public void OnRemovePolygonClick()
	{
		if (_coloradoPolygon == null)
		{
			return;
		}

		_coloradoPolygon.Remove();
		_coloradoPolygon = null;
		Debug.Log("Polygon was removed.");
	}

	#region marker_clustering

	[UsedImplicitly]
	public void OnAddClusterItems()
	{
		AddClusterItems();
	}

	[UsedImplicitly]
	public void OnAddSingleClusterItem()
	{
		if (_clusterManager != null)
		{
			var latLng = new LatLng(51.5005642, -0.1241729);
			var clusterItem = new ClusterItem(latLng, "Westminster Bridge", latLng.ToString());
			var westminsterBridge = clusterItem;
			_clusterManager.AddItem(westminsterBridge);
		}
	}

	[UsedImplicitly]
	public void OnClearClusterItems()
	{
		if (_clusterManager != null)
		{
			Debug.Log("Clearing cluster items");
			_clusterManager.ClearItems();
		}
	}

	#endregion

	/// <summary>
	/// Removes all markers, polylines, polygons, overlays, etc from the map.
	/// </summary>
	[UsedImplicitly]
	public void OnClearMapClick()
	{
		if (_map == null)
		{
			return;
		}

		_map.Clear();
		// All the elements are now removed, we cannot access them any more
		_circle = null;
		_marker = null;
		_groundOverlay = null;
		_polyline = null;
		_coloradoPolygon = null;
		_heatmap = null;
	}

	[UsedImplicitly]
	public void OnTestUiSettingsButtonClick(bool enable)
	{
		if (_map == null)
		{
			return;
		}

		EnableAllSettings(_map.UiSettings, enable);
	}

	static void EnableAllSettings(UiSettings settings, bool enable)
	{
		Debug.Log("Current Ui Settings: " + settings);

		// Buttons/other
		settings.IsCompassEnabled = enable;
		settings.IsIndoorLevelPickerEnabled = enable;
		settings.IsMapToolbarEnabled = enable;
		settings.IsMyLocationButtonEnabled = enable;
		settings.IsZoomControlsEnabled = enable;

		// Gestures
		settings.IsRotateGesturesEnabled = enable;
		settings.IsScrollGesturesEnabled = enable;
		settings.IsTiltGesturesEnabled = enable;
		settings.IsZoomGesturesEnabled = enable;
		settings.SetAllGesturesEnabled(enable);
	}

	#endregion

	void UpdateCircleProperties()
	{
		var circleCenter = new LatLng(circleLat.value, circleLng.value);
		CenterCamera(circleCenter);

		_circle.Center = circleCenter;
		_circle.FillColor = ColorUtils.RandomColor();
		_circle.StrokeColor = ColorUtils.RandomColor();
		_circle.StrokeWidth = circleStokeWidth.value;
		_circle.Radius = circleRadius.value;
		_circle.ZIndex = 1f;
		_circle.IsVisible = circleVisibilityToggle.isOn;
		_circle.IsClickable = true;
	}

	void UpdateMarkerProperties()
	{
		var markerPosition = new LatLng(markerLat.value, markerLng.value);
		CenterCamera(markerPosition);

		_marker.Position = markerPosition;
		_marker.Alpha = 1f;
		_marker.IsDraggable = true;
		_marker.Flat = true;
		_marker.IsVisible = true;
		_marker.Rotation = 0;
		_marker.SetAnchor(0.5f, 1f);
		_marker.SetInfoWindowAnchor(0.5f, 1f);
		_marker.Snippet = "Updated Marker";
		_marker.Title = "You can drag this marker";
		_marker.ZIndex = 2;
	}

	void UpdateGroundOverlayProperties()
	{
		CenterCamera(DemoUtils.BerlinLatLng);

		_groundOverlay.Bearing = 135;
		_groundOverlay.IsClickable = true;
		_groundOverlay.IsVisible = true;

		// Mutually exclusive but setting both to test
		_groundOverlay.Position = DemoUtils.BerlinLatLng;
		_groundOverlay.Bounds = DemoUtils.BerlinLatLngBounds;

		_groundOverlay.Transparency = 0.25f;
		_groundOverlay.ZIndex = 3;
		_groundOverlay.SetDimensions(200000); // Just setting twice to test
		_groundOverlay.SetDimensions(200000, 200000);
		_groundOverlay.SetImage(ImageDescriptor.FromAsset("overlay.png"));
		_groundOverlay.SetPositionFromBounds(DemoUtils.BerlinLatLngBounds);
	}

	void UpdateHeatmapProperties()
	{
		CenterCamera(DemoUtils.MelbourneLatLng);

		_heatmap.FadeIn = true;
		_heatmap.ZIndex = 1;
		_heatmap.Transparency = 0.5f;
		_heatmap.IsVisible = true;

		_heatmap.ClearTileCache();
	}

	void UpdatePolylineProperties()
	{
		_polyline.Points = DemoUtils.UsaPolylinePoints;
		_polyline.StartCap = new RoundCap();
		_polyline.StartCap = new SquareCap();
		_polyline.JointType = JointType.Bevel;

		// pixels on Android and points on iOS
		var width = Application.platform == RuntimePlatform.Android ? 25f : 3f;

		_polyline.Width = width;
		_polyline.Color = ColorUtils.RandomColor();
		_polyline.IsGeodesic = false;
		_polyline.IsVisible = true;
		_polyline.IsClickable = true;
		_polyline.ZIndex = 1f;
	}

	void UpdatePolygonProperties()
	{
		_coloradoPolygon.Points = DemoUtils.ColoradoBorders;
		_coloradoPolygon.Holes = new List<List<LatLng>>(); // no holes
		_coloradoPolygon.FillColor = Color.yellow;
		_coloradoPolygon.StrokeColor = Color.blue;
		_coloradoPolygon.StrokeJointType = JointType.Bevel;

		// pixels on Android and points on iOS
		var coloradoPolygonStrokeWidth = Application.platform == RuntimePlatform.Android ? 25f : 3f;

		_coloradoPolygon.StrokeWidth = coloradoPolygonStrokeWidth;
		_coloradoPolygon.IsGeodesic = false;
		_coloradoPolygon.IsVisible = true;
		_coloradoPolygon.IsClickable = true;
		_coloradoPolygon.ZIndex = 1f;
	}

	void Dismiss()
	{
		if (_map != null)
		{
			_map.Dismiss();
			_map = null;
		}
	}

	#region camera_animations

	[UsedImplicitly]
	public void AnimateCameraNewCameraPosition()
	{
		AnimateCamera(CameraUpdate.NewCameraPosition(CameraPosition));
	}

	[UsedImplicitly]
	public void AnimateCameraNewLatLng()
	{
		AnimateCamera(CameraUpdate.NewLatLng(new LatLng(camPosLat.value, camPosLng.value)));
	}

	[UsedImplicitly]
	public void AnimateCameraNewLatLngBounds1()
	{
		AnimateCamera(CameraUpdate.NewLatLngBounds(Bounds, 10));
	}

	[UsedImplicitly]
	public void AnimateCameraNewLatLngBounds2()
	{
		AnimateCamera(CameraUpdate.NewLatLngBounds(Bounds, 100, 100, 10));
	}

	[UsedImplicitly]
	public void AnimateCameraNewLatLngZoom()
	{
		const int zoom = 10;
		AnimateCamera(CameraUpdate.NewLatLngZoom(new LatLng(camPosLat.value, camPosLng.value), zoom));
	}

	[UsedImplicitly]
	public void AnimateCameraScrollBy()
	{
		const int xPixel = 250;
		const int yPixel = 250;
		AnimateCamera(CameraUpdate.ScrollBy(xPixel, yPixel));
	}

	[UsedImplicitly]
	public void AnimateCameraZoomByWithFixedLocation()
	{
		const int amount = 5;
		const int x = 100;
		const int y = 100;
		AnimateCamera(CameraUpdate.ZoomBy(amount, x, y));
	}

	[UsedImplicitly]
	public void AnimateCameraZoomByAmountOnly()
	{
		const int amount = 5;
		AnimateCamera(CameraUpdate.ZoomBy(amount));
	}

	[UsedImplicitly]
	public void AnimateCameraZoomIn()
	{
		AnimateCamera(CameraUpdate.ZoomIn());
	}

	[UsedImplicitly]
	public void AnimateCameraZoomOut()
	{
		AnimateCamera(CameraUpdate.ZoomOut());
	}

	[UsedImplicitly]
	public void AnimateCameraZoomTo()
	{
		const int zoom = 10;
		AnimateCamera(CameraUpdate.ZoomTo(zoom));
	}

	void AnimateCamera(CameraUpdate cameraUpdate)
	{
		if (_map == null)
		{
			return;
		}

		if (IsCameraMovementAnimatedToggle.isOn)
		{
			_map.AnimateCamera(cameraUpdate);
		}
		else
		{
			_map.MoveCamera(cameraUpdate);
		}
	}

	#endregion

	[UsedImplicitly]
	public void OnLogMyLocation()
	{
		if (_map == null)
		{
			return;
		}

		if (!_map.IsMyLocationEnabled)
		{
			Debug.Log("Location tracking is not enabled. Set 'IsMyLocationButtonEnabled' to 'true' to start tracking location");
			return;
		}

		if (_map.Location != null)
		{
			Debug.Log("My location: " + _map.Location);
		}
		else
		{
			Debug.Log("My location is not available");
		}
	}

	[UsedImplicitly]
	public void OnSetCustomStyle()
	{
		if (_map != null)
		{
			_map.SetMapStyle(customStyleJson.text);
		}
	}

	[UsedImplicitly]
	public void OnResetToDefaultStyle()
	{
		if (_map != null)
		{
			_map.SetMapStyle(null);
		}
	}

	[UsedImplicitly]
	public void OnSetMapVisible()
	{
		if (_map != null)
		{
			_map.IsVisible = true;
		}
	}

	[UsedImplicitly]
	public void OnSetMapInvisible()
	{
		if (_map != null)
		{
			_map.IsVisible = false;
		}
	}

	[UsedImplicitly]
	public void OnSetMapPosition()
	{
		if (_map != null)
		{
			_map.SetRect(new Rect(0f, 0f, Screen.width / 2f, Screen.height / 2f));
		}
	}

	[UsedImplicitly]
	public void OnLogMapProperties()
	{
		if (_map != null)
		{
			Debug.Log("Current map: " + _map);
		}
	}

	[UsedImplicitly]
	public void OnUpdateMapProperties()
	{
		if (_map != null)
		{
			_map.MapType = GoogleMapType.Hybrid;
		}
	}

	[UsedImplicitly]
	public void OnTakeSnapshot()
	{
		if (_map != null)
		{
			_map.TakeSnapshot(texture =>
			{
				Debug.Log("Snapshot captured: " + texture.width + " x " + texture.height);
				snapshotImage.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			});
		}
	}

	[UsedImplicitly]
	public void LogMapProperties()
	{
		Debug.Log(_map);
	}

	#region helpers

	static Rect RectTransformToScreenSpace(RectTransform transform)
	{
		Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
		Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
		rect.x -= transform.pivot.x * size.x;
		rect.y -= (1.0f - transform.pivot.y) * size.y;
		rect.x = Mathf.CeilToInt(rect.x);
		rect.y = Mathf.CeilToInt(rect.y);
		return rect;
	}

	#endregion

	void CenterCamera(LatLng latLng)
	{
		if (_map != null)
		{
			_map.AnimateCamera(CameraUpdate.NewLatLng(latLng));
		}
	}
}