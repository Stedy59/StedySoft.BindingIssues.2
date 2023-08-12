using System;

using Android.Graphics.Drawables;
using Android.Widget;
using AView = Android.Views.View;

using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using MColors = Microsoft.Maui.Graphics.Colors;
using MRect = Microsoft.Maui.Graphics.Rect;
using MSize =Microsoft.Maui.Graphics.Size;

namespace StedySoft.Maui.Framework.Controls {

	#region Class MauiFrameHandler
	public partial class MauiFrameHandler : ViewHandler<IContentView, FrameLayout> {

		#region Declarations
		private WrappedViewGroup _viewGroup;

		private GradientDrawable _highlightDrawable;
		private GradientDrawable _backgroundDrawable;

		private RealtimeBlurView _realtimeBlurView;
		#endregion

		#region Protected Overrides
		public override void SetVirtualView(IView view) =>
			base.SetVirtualView(view);

		protected override FrameLayout CreatePlatformView() =>
			new(this.Context) {
				ClipToOutline = true };

		protected override void ConnectHandler(FrameLayout platformView) {
			this._viewGroup = new WrappedViewGroup(this.Context) {
				CrossPlatformMeasure = new Func<double, double, MSize>(this.VirtualView.CrossPlatformMeasure),
				CrossPlatformArrange = new Func<MRect, MSize>(this.VirtualView.CrossPlatformArrange)
			};

			platformView.ViewAttachedToWindow += this.OnViewAttachedToWindow;
			platformView.ViewDetachedFromWindow += this.OnViewDetachedFromWindow;

			base.ConnectHandler(platformView);
		}

		protected override void DisconnectHandler(FrameLayout platformView) {
			this._viewGroup.CrossPlatformMeasure = null;
			this._viewGroup.CrossPlatformArrange = null;

			platformView.ViewAttachedToWindow -= this.OnViewAttachedToWindow;
			platformView.ViewDetachedFromWindow -= this.OnViewDetachedFromWindow;

			platformView.RemoveAllViews();
			base.DisconnectHandler(platformView);
		}
		#endregion

		#region Private Methods
		private void _updateDrawables() {
			switch (this.VirtualView.As<MauiFrame>().FrameStyle) {
				case FrameStyle.Acrylic:
					if (this._realtimeBlurView == null) {
						this._realtimeBlurView = new RealtimeBlurView(this.Context);
						this._realtimeBlurView.SetBlurRadius(100);
						this._realtimeBlurView.SetOverlayColor(MColors.Transparent.ToPlatform());
						this._realtimeBlurView.SetDownSampleFactor(2);
						this.PlatformView.AddView(this._realtimeBlurView);
					}
					break;
				case FrameStyle.Highlight:
					if (this._highlightDrawable == null) {
						this._highlightDrawable = new GradientDrawable();
						this._highlightDrawable?.SetShape(ShapeType.Rectangle);
					}
					this._updateHighlightColor();
					break;
				case FrameStyle.Standard:
					break;
			}

			if (this._backgroundDrawable == null) {
				this._backgroundDrawable = new GradientDrawable();
				this._backgroundDrawable?.SetShape(ShapeType.Rectangle);
			}
			this._updateBackgroundColor();

			this._updateCornerRadius();

			if (this._viewGroup.Parent == null) { this.PlatformView.AddView(this._viewGroup); }

			this._updateLayerDrawable();
		}

		private void _updateBackgroundColor() {
			switch (this.VirtualView.As<MauiFrame>().FrameStyle) {
				case FrameStyle.Acrylic:
					this._backgroundDrawable?.SetColor(this.VirtualView.As<MauiFrame>().TintColor.ToPlatform());
					this._backgroundDrawable?.SetAlpha((int)(this.VirtualView.As<MauiFrame>().TintOpacity * 255));
					break;
				case FrameStyle.Highlight:
				case FrameStyle.Standard:
					this._backgroundDrawable?.SetColor(this.VirtualView.As<MauiFrame>().BackgroundColor.ToPlatform());
					break;
			}
		}

		private void _updateCornerRadius() {
			float cornerRadius = this.Context.ToPixels(this.VirtualView.As<MauiFrame>().CornerRadius);
			this._realtimeBlurView?.SetCornerRadius(cornerRadius, cornerRadius, cornerRadius, cornerRadius);
			this._highlightDrawable?.SetCornerRadius(cornerRadius);
			this._backgroundDrawable?.SetCornerRadius(cornerRadius);
		}

		private void _updateFrameStyle() {
			switch (this.VirtualView.As<MauiFrame>().FrameStyle) {
				case FrameStyle.Acrylic:
				case FrameStyle.Highlight:
				case FrameStyle.Standard:
					break;
			}
		}

		private void _updateHighlightColor() =>
			this._highlightDrawable?.SetColor(this.VirtualView.As<MauiFrame>().HighlightColor.ToPlatform());

		private void _updateLayerDrawable() {
			switch (this.VirtualView.As<MauiFrame>().FrameStyle) {
				case FrameStyle.Acrylic:
					this._viewGroup?.SetBackgroundDrawable(this._backgroundDrawable);
					break;
				case FrameStyle.Highlight: {
						if (this._highlightDrawable == null || this._backgroundDrawable == null) { return; }
						using LayerDrawable layer = new(new Drawable[] { this._highlightDrawable, this._backgroundDrawable });
						layer.SetLayerInsetTop(1, (int)this.Context.ToPixels(this.VirtualView.As<MauiFrame>().HighlightThickness));
						this._viewGroup?.SetBackgroundDrawable(layer);
					}
					break;
				case FrameStyle.Standard:
					this._viewGroup?.SetBackgroundDrawable(this._backgroundDrawable);
					break;
			}
		}
		#endregion

		#region Public Methods
		public static void MapBackgroundColor(MauiFrameHandler handler, MauiFrame mauiFrame) =>
			handler._updateBackgroundColor();

		public static void MapContent(MauiFrameHandler handler, MauiFrame mauiFrame) {
			if (handler.VirtualView.As<IContentView>().PresentedContent is IView view) {
				handler._viewGroup?.RemoveAllViews();
				handler._viewGroup?.AddView(view.ToPlatform(handler.MauiContext));
			}
		}

		public static void MapCornerRadius(MauiFrameHandler handler, MauiFrame mauiFrame) =>
			handler._updateCornerRadius();

		public static void MapFrameStyle(MauiFrameHandler handler, MauiFrame mauiFrame) =>
			handler._updateFrameStyle();

		public static void MapHighlightColor(MauiFrameHandler handler, MauiFrame mauiFrame) =>
			handler._updateHighlightColor();

		public static void MapHighlightThickness(MauiFrameHandler handler, MauiFrame mauiFrame) =>
			handler._updateLayerDrawable();

		public static void MapTintColor(MauiFrameHandler handler, MauiFrame mauiFrame) =>
			handler._updateBackgroundColor();

		public static void MapTintOpacity(MauiFrameHandler handler, MauiFrame mauiFrame) =>
			handler._updateBackgroundColor();
		#endregion

		#region Events
		private void OnViewAttachedToWindow(object sender, AView.ViewAttachedToWindowEventArgs e) =>
			this._updateDrawables();

		private void OnViewDetachedFromWindow(object sender, AView.ViewDetachedFromWindowEventArgs e) {
			this._realtimeBlurView?.Dispose();
			this._realtimeBlurView = null;
			this._highlightDrawable?.Dispose();
			this._highlightDrawable = null;
			this._backgroundDrawable?.Dispose();
			this._backgroundDrawable = null;
		}
		#endregion

	}
	#endregion

}