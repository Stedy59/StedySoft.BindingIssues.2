using Android.Content;
using Android.Graphics;

namespace StedySoft.Maui.Framework.Controls {

	#region Class EmptyBlurImplementation
	public class EmptyBlurImplementation : IBlurImplementation {

		public bool Prepare(Context context, Bitmap buffer, float radius) => false;

		public void Release() {	}

		public void Blur(Bitmap input, Bitmap output) {	}

	}
	#endregion

}