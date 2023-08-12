using Android.Content;
using Android.Graphics;
using Android.Renderscripts;

using Element = Android.Renderscripts.Element;

namespace StedySoft.Maui.Framework.Controls {

	#region Class StockBlurImplementation
	public class StockBlurImplementation : IBlurImplementation {

		#region Declarations
		private RenderScript _renderScript;

		private ScriptIntrinsicBlur _blurScript;

		private Allocation _blurInput;

		private Allocation _blurOutput;
		#endregion

		#region Public Methods
		public bool Prepare(Context context, Bitmap buffer, float radius) {
			if (this._renderScript == null) {
				try {
					this._renderScript = RenderScript.Create(context);
					this._blurScript = ScriptIntrinsicBlur.Create(this._renderScript, Element.U8_4(this._renderScript));
				}
				catch {
					this.Release();
					return false;
				}
			}
			this._blurScript.SetRadius(radius);
			this._blurInput = Allocation.CreateFromBitmap(this._renderScript, buffer, Allocation.MipmapControl.MipmapNone, AllocationUsage.Script);
			this._blurOutput = Allocation.CreateTyped(this._renderScript, this._blurInput.Type);
			return true;
		}


		public void Release() {
			if (!this._blurInput.IsNullOrDisposed()) {
				this._blurInput.Destroy();
				this._blurInput = null;
			}
			if (!this._blurOutput.IsNullOrDisposed()) {
				this._blurOutput.Destroy();
				this._blurOutput = null;
			}
			if (!this._blurScript.IsNullOrDisposed()) {
				this._blurScript.Destroy();
				this._blurScript = null;
			}
			if (!this._renderScript.IsNullOrDisposed()) {
				this._renderScript.Destroy();
				this._renderScript = null;
			}
		}

		public void Blur(Bitmap input, Bitmap output) {
			this._blurInput.CopyFrom(input);
			this._blurScript.SetInput(this._blurInput);
			this._blurScript.ForEach(this._blurOutput);
			this._blurOutput.CopyTo(output);
		}
		#endregion

	}
	#endregion

}
