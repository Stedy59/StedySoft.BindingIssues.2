using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;

using StedySoft.Maui.Framework.Controls;

namespace StedySoft.Maui.Framework {

	#region Class RealtimeBlurView
	public class RealtimeBlurView : View {

		#region Declarations
		private static int BLUR_IMPL;
		private static int RENDERING_COUNT;


		private readonly float[] mRadii = new float[8];

		private float _downSampleFactor;

		private int mOverlayColor;

		private float mBlurRadius;

		private readonly IBlurImplementation _blurImplementation;

		private bool mDirty;

		private Bitmap mBitmapToBlur, mBlurredBitmap;

		private Canvas mBlurringCanvas;

		private bool mIsRendering;

		private readonly Paint mPaint;

		private JniWeakReference<View> _weakDecorView;

		private bool mDifferentRoot;

		private bool _isContainerShown;

		private bool _autoUpdate;

		private readonly PreDrawListener preDrawListener;
		#endregion

		#region Constructor
		public RealtimeBlurView(Context context, string formsId = null) : base(context) {
			this._blurImplementation = this.GetBlurImplementation();
			this.mPaint = new Paint();
			this._isContainerShown = true;
			this._autoUpdate = true;
			this.preDrawListener = new PreDrawListener(this);
		}

		public RealtimeBlurView(IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer) { }
		#endregion

		#region Private Classes
		private class PreDrawListener : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener {

			private readonly JniWeakReference<RealtimeBlurView> _weakBlurView;

			public PreDrawListener(RealtimeBlurView blurView) =>
				this._weakBlurView = new JniWeakReference<RealtimeBlurView>(blurView);

			public PreDrawListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

			public bool OnPreDraw() {
				if (!this._weakBlurView.TryGetTarget(out RealtimeBlurView blurView)) { return false; }
				if (!blurView._isContainerShown) { return false; }
				View mDecorView = blurView.GetRootView();
				int[] locations = new int[2];
				Bitmap oldBmp = blurView.mBlurredBitmap;
				View decor = mDecorView;
				if (!decor.IsNullOrDisposed() && blurView.IsShown && blurView.Prepare()) {
					bool redrawBitmap = blurView.mBlurredBitmap != oldBmp;
					decor.GetLocationOnScreen(locations);
					int x = -locations[0];
					int y = -locations[1];
					blurView.GetLocationOnScreen(locations);
					x += locations[0] > 5 ? locations[0] - 5 : locations[0];
					y += locations[1] > 5 ? locations[1] - 5 : locations[1];
					blurView.mBitmapToBlur.EraseColor(blurView.mOverlayColor & 0xffffff);
					int rc = blurView.mBlurringCanvas.Save();
					blurView.mIsRendering = true;
					RENDERING_COUNT++;
					try {
						blurView.mBlurringCanvas.Scale(1f * blurView.mBitmapToBlur.Width / blurView.Width, 1f * blurView.mBitmapToBlur.Height / blurView.Height);
						blurView.mBlurringCanvas.Translate(-x, -y);
						decor.Background?.Draw(blurView.mBlurringCanvas);
						decor.Draw(blurView.mBlurringCanvas);
					}
					finally {
						blurView.mIsRendering = false;
						RENDERING_COUNT--;
						blurView.mBlurringCanvas.RestoreToCount(rc);
					}
					blurView.Blur(blurView.mBitmapToBlur, blurView.mBlurredBitmap);
					if (redrawBitmap || blurView.mDifferentRoot) { blurView.Invalidate(); }
				}
				return true;
			}
		}
		#endregion

		#region Protected Overrides
		protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility) {
			base.OnVisibilityChanged(changedView, visibility);
			if (changedView.GetType().Name == "PageContainer") {
				this._isContainerShown = visibility == ViewStates.Visible;
				this.SetAutoUpdate(this._isContainerShown);
			}
		}

		protected override void OnAttachedToWindow() {
			base.OnAttachedToWindow();
			View mDecorView = this.GetRootView();
			if (mDecorView == null) {
				this.SetRootView(this.GetActivityDecorView());
			}
			else {
				this.OnAttached(mDecorView);
			}
		}

		protected override void OnDetachedFromWindow() {
			View mDecorView = this.GetRootView();
			if (mDecorView != null) { this.UnsubscribeToPreDraw(mDecorView); }
			this.Release();
			base.OnDetachedFromWindow();
		}

		protected override void OnDraw(Canvas canvas) {
			base.OnDraw(canvas);
			this.DrawRoundedBlurredBitmap(canvas, this.mBlurredBitmap, this.mOverlayColor);
		}

		protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
			base.OnSizeChanged(w, h, oldw, oldh);
			if (w > 0 && h > 0) { _ = this.preDrawListener.OnPreDraw(); }
		}
		#endregion

		#region Protected Methods
		protected IBlurImplementation GetBlurImplementation() {
			try {
				StockBlurImplementation stockBlurImplementation = new();
				Bitmap bmp = Bitmap.CreateBitmap(4, 4, Bitmap.Config.Argb8888);
				_ = stockBlurImplementation.Prepare(this.Context, bmp, 4);
				stockBlurImplementation.Release();
				bmp.Recycle();
				BLUR_IMPL = 3;
			}
			catch { }
			if (BLUR_IMPL == 0) { BLUR_IMPL = -1; }
			return BLUR_IMPL switch {
				3 => new StockBlurImplementation(),
				_ => new EmptyBlurImplementation(),
			};
		}

		protected bool Prepare() {
			if (this.mBlurRadius == 0) {
				this.Release();
				return false;
			}
			float downSampleFactor = this._downSampleFactor;
			float radius = this.mBlurRadius / downSampleFactor;
			if (radius > 25) {
				downSampleFactor = downSampleFactor * radius / 25;
				radius = 25;
			}
			int width = this.Width;
			int height = this.Height;
			int scaledWidth = Math.Max(1, (int)(width / downSampleFactor));
			int scaledHeight = Math.Max(1, (int)(height / downSampleFactor));
			bool dirty = this.mDirty;
			if (this.mBlurringCanvas == null
				|| this.mBlurredBitmap == null
				|| this.mBlurredBitmap.Width != scaledWidth
				|| this.mBlurredBitmap.Height != scaledHeight) {
				dirty = true;
				this.ReleaseBitmap();
				bool r = false;
				try {
					this.mBitmapToBlur = Bitmap.CreateBitmap(scaledWidth, scaledHeight, Bitmap.Config.Argb8888);
					if (this.mBitmapToBlur == null) {
						return false;
					}
					this.mBlurringCanvas = new Canvas(this.mBitmapToBlur);
					this.mBlurredBitmap = Bitmap.CreateBitmap(scaledWidth, scaledHeight, Bitmap.Config.Argb8888);
					if (this.mBlurredBitmap == null) { return false; }
					r = true;
				}
				catch { }
				finally {
					if (!r) { this.Release(); }
				}
				if (!r) { return false; }
			}
			if (dirty) {
				if (this._blurImplementation.Prepare(this.Context, this.mBitmapToBlur, radius)) {
					this.mDirty = false;
				}
				else {
					return false;
				}
			}
			return true;
		}

		protected void Blur(Bitmap bitmapToBlur, Bitmap blurredBitmap) =>
			this._blurImplementation.Blur(bitmapToBlur, blurredBitmap);

		protected View GetActivityDecorView() {
			Context ctx = this.Context;
			for (int i = 0; i < 4 && ctx != null && ctx is not Activity && ctx is ContextWrapper wrapper; i++) {
				ctx = wrapper.BaseContext;
			}
			return ctx is Activity activity ? activity.Window.DecorView : null;
		}
		#endregion

		#region Private Methods
		private void SubscribeToPreDraw(View decorView) {
			if (decorView.IsNullOrDisposed() || decorView.ViewTreeObserver.IsNullOrDisposed()) { return; }
			decorView.ViewTreeObserver.AddOnPreDrawListener(this.preDrawListener);
		}

		private void UnsubscribeToPreDraw(View decorView) {
			if (decorView.IsNullOrDisposed() || decorView.ViewTreeObserver.IsNullOrDisposed()) { return; }
			decorView.ViewTreeObserver.RemoveOnPreDrawListener(this.preDrawListener);
		}

		private View GetRootView() {
			View mDecorView = null;
			_ = (this._weakDecorView?.TryGetTarget(out mDecorView));
			return mDecorView;
		}

		private void OnAttached(View mDecorView) {
			if (mDecorView != null) {
				using Handler handler = new(Looper.MainLooper);
				_ = handler.PostDelayed(
					() => {
						this.SubscribeToPreDraw(mDecorView);
						this.mDifferentRoot = mDecorView.RootView != this.RootView;
						if (this.mDifferentRoot) { mDecorView.PostInvalidate(); }
					},
					10);
			}
			else {
				this.mDifferentRoot = false;
			}
		}

		private void SetAutoUpdate(bool autoUpdate) {
			if (autoUpdate) {
				this.EnableAutoUpdate();
				return;
			}
			this.DisableAutoUpdate();
		}

		private void DrawRoundedBlurredBitmap(Canvas canvas, Bitmap blurredBitmap, int overlayColor) {
			if (blurredBitmap != null) {
				RectF mRectF = new() { Right = this.Width, Bottom = this.Height };
				this.mPaint.Reset();
				this.mPaint.AntiAlias = true;
				BitmapShader shader = new(blurredBitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
				Matrix matrix = new();
				_ = matrix.PostScale(mRectF.Width() / blurredBitmap.Width, mRectF.Height() / blurredBitmap.Height);
				shader.SetLocalMatrix(matrix);
				_ = this.mPaint.SetShader(shader);
				using Path path = new();
				path.AddRoundRect(mRectF, this.mRadii, Path.Direction.Cw);
				canvas.DrawPath(path, this.mPaint);
			}
		}

		private void EnableAutoUpdate() {
			if (this._autoUpdate) { return; }
			this._autoUpdate = true;
			using Handler handler = new(Looper.MainLooper);
			_ = handler.PostDelayed(
				() => {
					View mDecorView = this.GetRootView();
					if (mDecorView == null || !this._autoUpdate) { return; }
					this.SubscribeToPreDraw(mDecorView);
				},
				100);
		}

		private void DisableAutoUpdate() {
			if (!this._autoUpdate) { return; }
			this._autoUpdate = false;
			View mDecorView = this.GetRootView();
			if (mDecorView == null) { return; }
			this.UnsubscribeToPreDraw(mDecorView);
		}

		private void ReleaseBitmap() {
			if (!this.mBitmapToBlur.IsNullOrDisposed()) {
				this.mBitmapToBlur.Recycle();
				this.mBitmapToBlur = null;
			}
			if (!this.mBlurredBitmap.IsNullOrDisposed()) {
				this.mBlurredBitmap.Recycle();
				this.mBlurredBitmap = null;
			}
		}
		#endregion

		#region Public Overrides
		public override void Draw(Canvas canvas) {
			if (!this.mIsRendering && RENDERING_COUNT <= 0) { base.Draw(canvas); }
		}
		#endregion

		#region Public Methods
		public void SetCornerRadius(float topLeft, float topRight, float bottomRight, float bottomLeft) {
			float[] radius = new float[8] { topLeft, topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft };
			if (this.mRadii == radius) { return; }
			this.mDirty = true;
			this.mRadii[0] = topLeft;
			this.mRadii[1] = topLeft;
			this.mRadii[2] = topRight;
			this.mRadii[3] = topRight;
			this.mRadii[4] = bottomRight;
			this.mRadii[5] = bottomRight;
			this.mRadii[6] = bottomLeft;
			this.mRadii[7] = bottomLeft;
			this.Invalidate();
		}

		public void SetDownSampleFactor(float factor) {
			if (factor <= 0) { throw new ArgumentException("DownSample factor must be greater than 0."); }
			if (this._downSampleFactor != factor) {
				this._downSampleFactor = factor;
				this.mDirty = true;
				this.ReleaseBitmap();
				this.Invalidate();
			}
		}

		public void Destroy() {
			if (this._weakDecorView != null && this._weakDecorView.TryGetTarget(out View mDecorView)) { this.UnsubscribeToPreDraw(mDecorView); }
			this.Release();
			this._weakDecorView = null;
		}

		public void Release() {
			this.SetRootView(null);
			this.ReleaseBitmap();
			this._blurImplementation?.Release();
		}

		public void SetBlurRadius(float radius, bool invalidate = true) {
			if (this.mBlurRadius != radius) {
				this.mBlurRadius = radius;
				this.mDirty = true;
				if (invalidate) { this.Invalidate(); }
			}
		}

		public void SetOverlayColor(int color, bool invalidate = true) {
			if (this.mOverlayColor != color) {
				this.mOverlayColor = color;
				if (invalidate) { this.Invalidate(); }
			}
		}

		public void SetRootView(View rootView) {
			View mDecorView = this.GetRootView();
			if (mDecorView != rootView) {
				this.UnsubscribeToPreDraw(mDecorView);
				this._weakDecorView = new JniWeakReference<View>(rootView);
				if (this.IsAttachedToWindow) { this.OnAttached(rootView); }
			}
		}
		#endregion

	}
	#endregion

}