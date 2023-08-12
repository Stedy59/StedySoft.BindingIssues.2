using System;

namespace StedySoft.Maui.Framework {

	#region Class JniExtensions
	public static class JniExtensions {

		#region Public Methods
		public static bool IsNullOrDisposed(this Java.Lang.Object javaObject) =>
			javaObject == null || javaObject.Handle == IntPtr.Zero;

		public static bool IsDisposed(this Java.Lang.Object obj) =>
			obj.Handle == IntPtr.Zero;

		public static bool IsAlive(this Java.Lang.Object obj) =>
			obj?.IsDisposed() == false;

		public static bool IsDisposed(this Android.Runtime.IJavaObject obj) =>
			obj.Handle == IntPtr.Zero;

		public static bool IsAlive(this Android.Runtime.IJavaObject obj) =>
			obj?.IsDisposed() == false;
		#endregion

	}
	#endregion

}
