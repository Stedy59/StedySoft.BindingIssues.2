using Android.Content;
using Android.Graphics;

namespace StedySoft.Maui.Framework.Controls {

	#region Interface IBlurImplementation
	public interface IBlurImplementation {

		bool Prepare(Context context, Bitmap buffer, float radius);

		void Release();

		void Blur(Bitmap input, Bitmap output);

	}
	#endregion

}
