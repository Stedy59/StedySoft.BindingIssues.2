using System;

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;

using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Graphics.Platform;

using MPathF = Microsoft.Maui.Graphics.PathF;
using MRect = Microsoft.Maui.Graphics.Rect;
using MRectF = Microsoft.Maui.Graphics.RectF;
using MSize = Microsoft.Maui.Graphics.Size;

namespace StedySoft.Maui.Framework.Controls {

	#region Abstract Class BaseWrappedViewGroup
	public abstract class BaseWrappedViewGroup : ViewGroup {

		#region Declarations
		private IShape _clipShape { get; set; }
		private Context _context { get; }
		#endregion

		#region Constructors
		protected BaseWrappedViewGroup(Context context) : base(context) =>
			this._context = context;

		protected BaseWrappedViewGroup(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) {
			Context context = this.Context;
			ArgumentNullException.ThrowIfNull(context);
			this._context = context;
		}

		protected BaseWrappedViewGroup(Context context, IAttributeSet attrs)
			: base(context, attrs) =>
				this._context = context;

		protected BaseWrappedViewGroup(Context context, IAttributeSet attrs, int defStyleAttr)
			: base(context, attrs, defStyleAttr) =>
				this._context = context;

		protected BaseWrappedViewGroup(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
			: base(context, attrs, defStyleAttr, defStyleRes) =>
				this._context = context;
		#endregion

		#region Protected Overrides
		protected override void DispatchDraw(Canvas? canvas) {
			this._clipChild(canvas);
			base.DispatchDraw(canvas);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
			if (this.CrossPlatformMeasure == null) {
				base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
				return;
			}

			double deviceIndependentWidth = widthMeasureSpec.ToDouble(this._context);
			double deviceIndependentHeight = heightMeasureSpec.ToDouble(this._context);

			MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
			MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);

			MSize measure = this.CrossPlatformMeasure(deviceIndependentWidth, deviceIndependentHeight);
			double width = widthMode == MeasureSpecMode.Exactly ? deviceIndependentWidth : measure.Width;
			double height = heightMode == MeasureSpecMode.Exactly ? deviceIndependentHeight : measure.Height;

			int platformWidth = Math.Max(this.MinimumWidth, (int)this._context.ToPixels(width));
			int platformHeight = Math.Max(this.MinimumHeight, (int)this._context.ToPixels(height));

			this.SetMeasuredDimension(platformWidth, platformHeight);
		}

		protected override void OnLayout(bool changed, int left, int top, int right, int bottom) {
			if (this.CrossPlatformArrange == null) { return; }
			_ = this.CrossPlatformArrange(this._context.ToCrossPlatformRectInReferenceFrame(left, top, right, bottom));
		}
		#endregion

		#region Internal Functions
		internal Func<double, double, MSize>? CrossPlatformMeasure { get; set; }
		internal Func<MRect, MSize>? CrossPlatformArrange { get; set; }
		#endregion

		#region Internal Properties
		internal IShape? ClipShape {
			get => this._clipShape;
			set {
				this._clipShape = value;
				this.PostInvalidate();
			}
		}

		internal double ClipStrokeThickness { get; set; }
		#endregion

		#region Private Methods
		private void _clipChild(Canvas? canvas) {
			if (this.ClipShape == null || canvas == null) { return; }
			float density = this._context.GetDisplayDensity();
			MPathF path = this.ClipShape?.PathForBounds(
				new MRectF(
					(float)this.ClipStrokeThickness,
					(float)this.ClipStrokeThickness,
					canvas.Width / density - (float)this.ClipStrokeThickness * 2f,
					canvas.Height / density - (float)this.ClipStrokeThickness * 2f));
			Path currentPath = path?.AsAndroidPath(scaleX: density, scaleY: density);
			if (currentPath != null) { _ = canvas.ClipPath(currentPath); }
		}
		#endregion

	}
	#endregion

}
