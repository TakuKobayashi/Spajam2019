using NinevaStudios.GoogleMaps;
using UnityEngine;

public class GoogleMapsMinimalDemo : MonoBehaviour
{
	void Start()
	{
		var cameraPosition = new CameraPosition(
			new LatLng(51.5285582f, -0.2416799f), 10, 0, 0);
		var options = new GoogleMapsOptions()
			.Camera(cameraPosition);

		var map = new GoogleMapsView(options);
		map.Show(new Rect(0, 0, Screen.width / 2, Screen.height / 2), OnMapReady);
	}

	void OnMapReady()
	{
		Debug.Log("The map is ready!");
	}
}