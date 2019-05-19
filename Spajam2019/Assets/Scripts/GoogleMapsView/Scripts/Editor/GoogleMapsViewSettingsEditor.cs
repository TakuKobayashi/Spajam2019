using NinevaStudios.GoogleMaps.Internal;
using UnityEditor;
using UnityEngine;

namespace GoogleMapsView.Scripts.Editor
{
	[CustomEditor(typeof(GoogleMapsViewSettings))]
	public class GoogleMapsViewSettingsEditor : UnityEditor.Editor
	{
		const string ApiKeyTooltip = "You must obtain an API key from Google in order to use Google Maps API";
		const string IosUsageDescriptionTooltip= "Text shown on the native iOS dialog requesting to access device location";

		[MenuItem("Window/Google Maps View/Edit Settings", false, 1000)]
		public static void Edit()
		{
			Selection.activeObject = GoogleMapsViewSettings.Instance;
		}

		public override void OnInspectorGUI()
		{
			using (new EditorGUILayout.VerticalScope("box"))
			{
				EditorGUILayout.HelpBox(
					"The plugin will modify your AndroidManifest.xml before the build starts",
					MessageType.Info);
				
				GUILayout.Label("Android Settings", EditorStyles.boldLabel);
				GoogleMapsViewSettings.AndroidApiKey = EditorGUILayout.TextField(new GUIContent("Android API Key [?]", ApiKeyTooltip),  GoogleMapsViewSettings.AndroidApiKey);
				GoogleMapsViewSettings.AddLocationPermission =
					EditorGUILayout.Toggle("Automatically add location permission to AndroidManifest.xml? ", GoogleMapsViewSettings.AddLocationPermission);

				if (GUILayout.Button("Read how to get and setup Android API key"))
				{
					Application.OpenURL("https://github.com/NinevaStudios/unity-google-maps-docs/wiki/Setup-(Android)");
				}
			}
			
			EditorGUILayout.Space();

			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("iOS Settings", EditorStyles.boldLabel);
				GoogleMapsViewSettings.IosApiKey = EditorGUILayout.TextField(new GUIContent("iOS API Key [?]", ApiKeyTooltip), GoogleMapsViewSettings.IosApiKey);
				GoogleMapsViewSettings.IosUsageDescription = EditorGUILayout.TextField(new GUIContent("iOS Usage Description [?]", IosUsageDescriptionTooltip), GoogleMapsViewSettings.IosUsageDescription);

				if (GUILayout.Button("Read how to get and setup iOS API key"))
				{
					Application.OpenURL("https://github.com/NinevaStudios/unity-google-maps-docs/wiki/Setup-(iOS)");
				}
			}
			
			EditorGUILayout.Space();

			using (new EditorGUILayout.HorizontalScope("box"))
			{
				if (GUILayout.Button("Read Documentation"))
				{
					Application.OpenURL("https://github.com/NinevaStudios/unity-google-maps-docs/wiki");
				}
				if (GUILayout.Button("Ask us anything on Discord"))
				{
					Application.OpenURL("https://discord.gg/SuJP9fY");
				}
			}
		}
	}
}