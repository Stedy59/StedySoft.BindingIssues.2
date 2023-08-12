using System;

namespace StedySoft.Maui.Framework {

	#region Class JniWeakReference
	public class JniWeakReference<T>(T target) where T : Java.Lang.Object {

		private readonly WeakReference<T> _reference = new(target);

		public bool TryGetTarget(out T target) {
			target = null;
			if (this._reference.TryGetTarget(out T innerTarget)
					&& innerTarget.Handle != IntPtr.Zero) {
				target = innerTarget;
			}
			return target != null;
		}

		public override string ToString() =>
			$"[JniWeakReference] {this._reference}";

	}
	#endregion

}
