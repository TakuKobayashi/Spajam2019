namespace NinevaStudios.GoogleMaps
{
	using System;
	using Internal;
	using JetBrains.Annotations;
	using DeadMosquito.JniToolkit;
	using UnityEngine;

	/// <summary>
	/// Defines a Bitmap image. For a marker, this class can be used to set the image of the marker icon. 
	/// For a ground overlay, it can be used to set the image to place on the surface of the earth.
	/// </summary>
	[PublicAPI]
	public sealed class ImageDescriptor
	{
		const string ImageDescriptorFactoryClass = "com.google.android.gms.maps.model.BitmapDescriptorFactory";
		const string ExpansionFileUtilsClass = "com.deadmosquitogames.gmaps.ExpansionFileUtils";

		[PublicAPI]
		public const float HUE_RED = 0.0F;

		[PublicAPI]
		public const float HUE_ORANGE = 30.0F;

		[PublicAPI]
		public const float HUE_YELLOW = 60.0F;

		[PublicAPI]
		public const float HUE_GREEN = 120.0F;

		[PublicAPI]
		public const float HUE_CYAN = 180.0F;

		[PublicAPI]
		public const float HUE_AZURE = 210.0F;

		[PublicAPI]
		public const float HUE_BLUE = 240.0F;

		[PublicAPI]
		public const float HUE_VIOLET = 270.0F;

		[PublicAPI]
		public const float HUE_MAGENTA = 300.0F;

		[PublicAPI]
		public const float HUE_ROSE = 330.0F;

		public enum ImageDescriptorType
		{
			Default,
			DefaultWithHue,
			AssetName,
			Texture2D
		}

		readonly ImageDescriptorType _descriptorType = ImageDescriptorType.Default;
		readonly string _assetName;
		readonly float _scaleOnIos;
		readonly float _hue;
		Texture2D _texure;

		public string AssetName
		{
			get { return _assetName; }
		}
		
		public Texture2D Texture
		{
			get { return _texure; }
		}

		public ImageDescriptorType DescriptorType
		{
			get { return _descriptorType; }
		}

		public float Hue
		{
			get { return _hue; }
		}

		public float ScaleOnIos
		{
			get { return _scaleOnIos; }
		}

		ImageDescriptor(float hue)
		{
			_descriptorType = ImageDescriptorType.DefaultWithHue;
			_hue = hue;
		}

		ImageDescriptor(string assetName, float scaleOnIos = 3.0f)
		{
			_descriptorType = ImageDescriptorType.AssetName;
			_assetName = assetName;
			_scaleOnIos = scaleOnIos;
		}

		ImageDescriptor(Texture2D texture2D)
		{
			_descriptorType = ImageDescriptorType.Texture2D;
			_texure = texture2D;
		}

		ImageDescriptor()
		{
		}

		/// <summary>
		/// Creates a <see cref="ImageDescriptor"/> that refers to the default marker image.
		/// </summary>
		/// <returns>The marker image descriptor.</returns>
		[PublicAPI]
		public static ImageDescriptor DefaultMarker()
		{
			return new ImageDescriptor();
		}

		/// <summary>
		/// Creates a <see cref="ImageDescriptor"/> that refers to a colorization of the default marker image. For convenience, there is a predefined set of hue values. E.g. <see cref="HUE_RED"/> 
		/// </summary>
		/// <returns>The marker image descriptor.</returns>
		/// <param name="hue">The hue of the marker. Value must be greater or equal to 0 and less than 360.</param>
		[PublicAPI]
		public static ImageDescriptor DefaultMarker(float hue)
		{
			return new ImageDescriptor(hue);
		}

		/// <summary>
		/// Creates a <see cref="ImageDescriptor"/> using the name of the image in the StreamingAssets directory. Must be full image name inside StreamingAssets folder e.g. "my-custom-marker.png"
		/// </summary>
		/// <param name="assetName">Asset name. Must be full image name inside StreamingAssets folder e.g. "my-custom-marker.png"</param>
		/// <returns>The image descriptor.</returns>
		[PublicAPI]
		public static ImageDescriptor FromAsset([NotNull] string assetName)
		{
			if (string.IsNullOrEmpty(assetName))
			{
				throw new ArgumentException("Image name cannot be null or empty", "assetName");
			}

			return new ImageDescriptor(assetName);
		}
		
		/// <summary>
		/// Creates a <see cref="ImageDescriptor"/> using the name of the passed <see cref="Texture2D"/> object. The texture must be readable.
		/// </summary>
		/// <param name="texture2D">Texture.</param>
		/// <returns>The image descriptor.</returns>
		[PublicAPI]
		public static ImageDescriptor FromTexture2D([NotNull] Texture2D texture2D)
		{
			if (texture2D == null)
			{
				throw new ArgumentNullException("texture2D");
			}

			return new ImageDescriptor(texture2D);
		}

		public AndroidJavaObject ToAJO()
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return null;
			}

			switch (_descriptorType)
			{
				case ImageDescriptorType.AssetName:
					using (var c = new AndroidJavaClass(ImageDescriptorFactoryClass))
					{
						try
						{
							if (IsAppBinarySplit)
							{
								var bitmap = ExpansionFileUtilsClass.AJCCallStaticOnce<AndroidJavaObject>("getBitmap", JniToolkitUtils.Activity, _assetName);
								return c.CallStaticAJO("fromBitmap", bitmap);
							}

							return c.CallStaticAJO("fromAsset", _assetName);
						}
						catch (Exception e)
						{
							Debug.LogError("Failed to load bitmap from expansion file: " + _assetName);
							Debug.LogException(e);
							return c.CallStaticAJO("defaultMarker");
						}
					}
				case ImageDescriptorType.DefaultWithHue:
					using (var c = new AndroidJavaClass(ImageDescriptorFactoryClass))
					{
						return c.CallStaticAJO("defaultMarker", _hue);
					}
				case ImageDescriptorType.Texture2D:
					using (var c = new AndroidJavaClass(ImageDescriptorFactoryClass))
					{
						return c.CallStaticAJO("fromBitmap", _texure.Texture2DToAndroidBitmap());
					}
				default:
					using (var c = new AndroidJavaClass(ImageDescriptorFactoryClass))
					{
						return c.CallStaticAJO("defaultMarker");
					}
			}
		}

		static bool IsAppBinarySplit
		{
			get { return Application.streamingAssetsPath.Contains("/obb/"); }
		}
	}
}