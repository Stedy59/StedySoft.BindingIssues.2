using System;

using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

using Microsoft.UI.Xaml.Media;

using Border = Microsoft.UI.Xaml.Controls.Border;
using CornerRadius = Microsoft.UI.Xaml.CornerRadius;
using Thickness = Microsoft.UI.Xaml.Thickness;

namespace StedySoft.Maui.Framework.Controls {

	#region Class MauiFrameHandler
	public partial class MauiFrameHandler : ViewHandler<IContentView, Border> {

		#region Declarations
		private AcrylicBrush _acrylicBrush { get; set; }
		#endregion

		#region Protected Overrides
		public override void SetVirtualView(IView view) =>
			base.SetVirtualView(view);

		protected override Border CreatePlatformView() =>
			new() {
				Child = new WrappedView {
					CrossPlatformMeasure = new Func<double, double, Size>(this.VirtualView.CrossPlatformMeasure),
					CrossPlatformArrange = new Func<Rect, Size>(this.VirtualView.CrossPlatformArrange)
				}

			};

		protected override void ConnectHandler(Border platformView) {
			this._acrylicBrush = new AcrylicBrush();
			base.ConnectHandler(platformView);
		}

		protected override void DisconnectHandler(Border platformView) {
			this._acrylicBrush = null;
			base.DisconnectHandler(platformView);
		}
		#endregion

		#region Private Methods
		private void _updateBackgroundColor() {
			switch (this.VirtualView.As<MauiFrame>().FrameStyle) {
				case FrameStyle.Acrylic:
					this.PlatformView.Child.As<WrappedView>().Background = Colors.Transparent.ToPlatform();
					break;
				case FrameStyle.Highlight:
					this.PlatformView.Child.As<WrappedView>().Background = this.VirtualView.As<MauiFrame>().BackgroundColor.ToPlatform();
					break;
				case FrameStyle.Standard:
					this.PlatformView.Child.As<WrappedView>().Background = Colors.Transparent.ToPlatform();
					break;
			}
		}

		private void _updateCornerRadius() {
			CornerRadius cornerRadius = new(this.VirtualView.As<MauiFrame>().CornerRadius);
			this.PlatformView.Child.As<WrappedView>().CornerRadius = cornerRadius;
			this.PlatformView.CornerRadius = cornerRadius;
		}

		private void _updateFrameStyle() {
			switch (this.VirtualView.As<MauiFrame>().FrameStyle) {
				case FrameStyle.Acrylic:
					this.PlatformView.Background = this._acrylicBrush;
					this.PlatformView.Child.As<WrappedView>().Background = Colors.Transparent.ToPlatform();
					this.PlatformView.Child.As<WrappedView>().Margin = new Thickness(0);
					break;
				case FrameStyle.Highlight:
					this.PlatformView.Background = this.VirtualView.As<MauiFrame>().HighlightColor.ToPlatform();
					this.PlatformView.Child.As<WrappedView>().Background = this.VirtualView.As<MauiFrame>().BackgroundColor.ToPlatform();
					this.PlatformView.Child.As<WrappedView>().Margin = new Thickness(0, this.VirtualView.As<MauiFrame>().HighlightThickness, 0, 0);
					break;
				case FrameStyle.Standard:
					this.PlatformView.Background = this.VirtualView.As<MauiFrame>().BackgroundColor.ToPlatform();
					this.PlatformView.Child.As<WrappedView>().Background = Colors.Transparent.ToPlatform();
					this.PlatformView.Child.As<WrappedView>().Margin = new Thickness(0);
					break;
			}
		}

		private void _updateHighlightColor() {
			switch (this.VirtualView.As<MauiFrame>().FrameStyle) {
				case FrameStyle.Acrylic:
					this.PlatformView.Background = this._acrylicBrush;
					break;
				case FrameStyle.Highlight:
					this.PlatformView.Background = this.VirtualView.As<MauiFrame>().HighlightColor.ToPlatform();
					break;
				case FrameStyle.Standard:
					this.PlatformView.Background = this.VirtualView.As<MauiFrame>().BackgroundColor.ToPlatform();
					break;
			}
		}

		private void _updateLayerDrawable() {
			switch (this.VirtualView.As<MauiFrame>().FrameStyle) {
				case FrameStyle.Acrylic:
					this.PlatformView.Child.As<WrappedView>().Margin = new Thickness(0);
					break;
				case FrameStyle.Highlight:
					this.PlatformView.Child.As<WrappedView>().Margin = new Thickness(0, this.VirtualView.As<MauiFrame>().HighlightThickness, 0, 0);
					break;
				case FrameStyle.Standard:
					this.PlatformView.Child.As<WrappedView>().Margin = new Thickness(0);
					break;
			}
		}

		private void _updateTintColor() =>
			this._acrylicBrush.TintColor = this.VirtualView.As<MauiFrame>().TintColor.ToWindowsColor();

		private void _updateTintOpacity() =>
			this._acrylicBrush.TintOpacity = this.VirtualView.As<MauiFrame>().TintOpacity;
		#endregion

		#region Public Methods
		public static void MapBackgroundColor(MauiFrameHandler handler, MauiFrame mauiFrame) =>
			handler._updateBackgroundColor();

		public static void MapContent(MauiFrameHandler handler, MauiFrame mauiFrame) {
			if (handler.VirtualView.PresentedContent is IView view) {
				handler.PlatformView.Child.As<WrappedView>().Children.Clear();
				handler.PlatformView.Child.As<WrappedView>().Children.Add(view.ToPlatform(handler.MauiContext));
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
			handler._updateTintColor();

		public static void MapTintOpacity(MauiFrameHandler handler, MauiFrame mauiFrame) =>
			handler._updateTintOpacity();
		#endregion

	}
	#endregion

}