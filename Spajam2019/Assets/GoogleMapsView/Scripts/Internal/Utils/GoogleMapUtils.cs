namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using System.IO;
	using System.Runtime.InteropServices;
	using UnityEngine;

	public static class GoogleMapUtils
	{
		public static bool IsAndroid
		{
			get { return Application.platform == RuntimePlatform.Android; }
		}

		public static bool IsIos
		{
			get { return Application.platform == RuntimePlatform.IPhonePlayer; }
		}

		public static bool IsNotAndroid
		{
			get { return !IsAndroid; }
		}

		public static bool IsNotIosRuntime
		{
			get { return !IsIos; }
		}

		public static bool IsPlatformSupported
		{
			get { return IsAndroid || IsIos; }
		}

		public static bool IsPlatformNotSupported
		{
			get { return !IsPlatformSupported; }
		}

		public static bool IsZero(this IntPtr intPtr)
		{
			return intPtr == IntPtr.Zero;
		}

		public static bool IsNonZero(this IntPtr intPtr)
		{
			return intPtr != IntPtr.Zero;
		}
		
		public static T Cast<T>(this IntPtr instancePtr)
		{
			var instanceHandle = GCHandle.FromIntPtr(instancePtr);
			if (!(instanceHandle.Target is T))
			{
				throw new InvalidCastException("Failed to cast IntPtr");
			}

			var castedTarget = (T) instanceHandle.Target;
			return castedTarget;
		}
		
		public static IntPtr GetPointer(this object obj)
		{
			return obj == null ? IntPtr.Zero : GCHandle.ToIntPtr(GCHandle.Alloc(obj));
		}

		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}

		public static string ToFullStreamingAssetsPath(this string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}

			return Path.Combine(Application.streamingAssetsPath, fileName);
		}
	}
}