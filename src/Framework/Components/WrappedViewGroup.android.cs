using Android.Content;
using Android.Runtime;
using Android.Util;

namespace StedySoft.Maui.Framework.Controls {

	#region Class WrappedViewGroup
	public class WrappedViewGroup : BaseWrappedViewGroup {

		public WrappedViewGroup(Context context) : base(context) { }

		public WrappedViewGroup(nint javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer) { }

		public WrappedViewGroup(Context context, IAttributeSet attrs)
			: base(context, attrs) { }

		public WrappedViewGroup(Context context, IAttributeSet attrs, int defStyleAttr)
			: base(context, attrs, defStyleAttr) { }

		public WrappedViewGroup(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
			: base(context, attrs, defStyleAttr, defStyleRes) { }

	}
	#endregion

}
