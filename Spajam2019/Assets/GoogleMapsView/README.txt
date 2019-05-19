Documentation: https://github.com/TarasOsiris/unity-google-maps-docs/wiki
Support: support@ninevastudios.com
Ask us anything on our Discord server: https://discord.gg/SuJP9fY

---

### CHANGELOG

## v2.6.0

WARNING BREAKING CHANGE! The namespace has been renamed from `DeadMosquito.GoogleMapsView` -> `NinevaStudios.GoogleMaps` to avoid namespace collisions with the class name. Please update your usings after update.
WARNING BREAKING CHANGE! Place picker / Places API functionality has been removed in this release!

+ ADDED Added a minimal example to show the map
+ ADDED Functionality to customize marker properties when using marker clustering
+ ADDED Click listeners for polygons and polylines
+ ADDED Exposed iOS location usage description into the setup editor window
+ FIXED `Marker.SetIcon` changes marker size on iOS

---

## v2.5.0

IMPORTANT! Place picker/autocomplete functionality will unfortunately be removed in the next release as Google is deprecating Places API as the part of Google Play Services and removing the Place picker functionality completely. I have decided not to put new Places SKD 1.0 as the part of Google Maps plugin as it's incredibly heavy and has really a lot of dependencies.

+ Editor to configure the API key. Go to Window -> GoogleMapsView -> Edit Settings
+ FIXED `Marker.IsInfoWindowShown` not working correctly on iOS
+ FIXED `Marker.ShowInfoWindow/HideInfoWindow` not working correctly on iOS
+ FIXED memory issue with custom icon markers

---

## v2.4.1

IMPORTANT! Minimum supported Unity version is 2017.1.5f1

+ FIXED `IsVisible` property getter not working properly on Android
+ FIXED issue when map was always set to visible when resuming on Android
+ FIXED issue when `AnimateCamera` was not working properly with `CameraUpdate.NewCameraPosition`

---

## v2.4.0

+ FIXED Issue with marker click listener consuming events incorrectly on iOS
+ FIXED Issue when targeting API level 28 (Android 9.0) or above, more on this: https://developers.google.com/maps/documentation/android-sdk/config#specify_requirement_for_apache_http_legacy_library
+ IMPROVED Handle pause/resume of the game automatically on Android
+ ADDED event for handling orientation changes. See `GoogleMapsView.OnOrientationChange`
+ ADDED `IsTrafficEnabled` property to `GoogleMapsView.cs`
+ ADDED `GoogleMapsView.SetPadding` method
+ ADDED Google maps URLs functionality, see this for more details: https://developers.google.com/maps/documentation/urls/guide
+ ADDED `HideInfoWindow` method and `IsInfoWindowShown` property to `Marker.cs`
+ ADDED util class `SphericalUtils.cs` for navigational calculations (heading, offset, offset origin, distance, path length, area)
+ ADDED Callback to detect when camera started moving due to animation or user interaction

---

## v2.3.0

+ ADDED place picker an place autocompete features! (https://developers.google.com/places/android-sdk/placepicker)
+ ADDED ability to create marker images from `Texture2D` so now marker images can be created dynamically
+ ADDED ability to set arbitrary size and position of the view
+ ADDED ability to take map view snapshot
+ UPDATED Google Play Services Maps version to 15.0.1 (make sure to remove all previous *.aar files before updating)
+ UPDATED Google Maps SDK version to 2.7.0 for iOS
+ FIXED Plugin not working correctly on Android IL2CPP build

---

## v2.2.0

This release brings much awaited marker clustering and heatmaps features

+ ADDED Marker clustering feature! (https://developers.google.com/maps/documentation/android-api/utility/marker-clustering)
+ ADDED Heatmaps feature! (https://developers.google.com/maps/documentation/android-api/utility/heatmap)
+ ADDED Callback to know when marker info window was clicked
+ FIXED View still blocking the touch input even when hidden. (Android)
+ FIXED Issues on Android when bytecode stripping is enabled

---

## v2.1.1

+ FIXED Invalid view size on iPhone Plus devices

---

## v2.1.0

+ ADDED Support for map styling with Json
+ ADDED Functionality to hide/show map view without losing current map state
+ ADDED Functionality to get current camera position (lat, lng, tilt, zoom, bearing) from the map
+ ADDED Functionality to get/set map type directly on the map
+ ADDED Functionality to move camera instantly without animation
+ FIXED Back button not working on Android when map is visible

---

## v2.0.0

** IMPORTANT This release adds iOS support, please backup the project before updating and read the documentation! **

+ ADDED iOS Support! 
+ FIXED Marker position not being updated after marker was dragged

---

## v1.2.3

+ IMPROVED Added proguard configuration to avoid stripping plugin classes
+ CHANGED java library package to avoid conflicting package with other Dead Mosquite Games packages

---

## v1.2.2

+ ADDED function to getch user location when location is enabled

---

## v1.2.1

+ FIXED Compilation issued when the platform is not set to Android
+ ADDED `ShowInfoWindow()` method to the Marker class

---

## v1.2.0

+ UPDATED Google Play Services Maps version to 11.0.2 (make sure to remove all previous *.aar files before updating)
+ ADDED Functionality to draw polylines on the map
+ ADDED Functionality to draw polygons on the map
+ ADDED Functionality to animate camera.
+ IMPROVED Demo now handles screen orientation changes properly
+ FIXED Now everything works correctly when splitting application binary

---

## v1.1.0

+ ADDED Functionality for showing user location on the map
+ ADDED Functionality to modify Map Ui Options
+ ADDED Listener for map click 
+ ADDED Listener for long map click
+ ADDED functionality to show/modify Ground Overlays on the map
+ ADDED functionality to show/modify Markers on the map
+ ADDED functionality to show/modify Circles on the map
+ ADDED Circle click listener functionality
+ ADDED Marker click listener functionality

+ ADDED Function to set map view padding

---

## v1.0.0

+ Initial release

---

Attributions

Open Source libraries used:

* https://github.com/getsocial-im/getsocial-unity-sdk