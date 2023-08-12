using System;
using System.Linq;

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace StedySoft.Maui.Framework.Controls {

	#region Class ViewBoxLayoutManager
	internal class ViewBoxLayoutManager(ViewBox viewBox) : ILayoutManager {

		#region Declarations
		private readonly ViewBox _viewBox = viewBox;

		private View? _view =>
			this._viewBox.Children.FirstOrDefault().As<View>();
		#endregion

		#region Private Methods
		private SizeRequest? _measureFull(View view) =>
			view.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins);

		private double _calculateScale(double value, double valueRequest) {
			double scale = valueRequest == 0 ? 0 : value / valueRequest;
			return this._viewBox.ScaleMode switch {
				ScaleModes.ScaleBoth => scale,
				ScaleModes.ScaleReduce => scale > 1 ? 1 : scale,
				ScaleModes.ScaleEnlarge => scale < 1 ? 1 : scale,
				_ => scale,
			};
		}
		#endregion

		#region Public Methods
		public Size ArrangeChildren(Rect bounds) {
			Size arrangeSize = bounds.Size;
			try {
				if (this._view is not null) {
					SizeRequest? requestCache = this._viewBox.ContentMeasurementCache = this._measureFull(this._view);
					Size request = requestCache.Value.Request;
					double scaleFactor = Math.Min(
						this._calculateScale(arrangeSize.Width, request.Width),
						this._calculateScale(arrangeSize.Height, request.Height));
					this._view.AnchorX = this._view.AnchorY = 0;
					this._view.Scale = scaleFactor;
					arrangeSize.Width = scaleFactor * request.Width;
					arrangeSize.Height = scaleFactor * request.Height;
					double yOffset = this._view.VerticalOptions.Alignment switch {
						LayoutAlignment.Start => 0,
						LayoutAlignment.End => bounds.Height - arrangeSize.Height,
						_ => (bounds.Height - arrangeSize.Height) / 2.0,
					};
					double xOffset = this._view.HorizontalOptions.Alignment switch {
						LayoutAlignment.Start => 0,
						LayoutAlignment.End => bounds.Width - arrangeSize.Width,
						_ => (bounds.Width - arrangeSize.Width) / 2.0,
					};
					_ = this._view.As<IView>().Arrange(new Rect(new Point(xOffset, yOffset), request));
				}
			}
			catch { }
			return arrangeSize;
		}

		public Size Measure(double widthConstraint, double heightConstraint) {
			SizeRequest zero = new(Size.Zero);
			if (this._view == null
				|| double.IsNaN(widthConstraint) || double.IsNegativeInfinity(widthConstraint)
				|| double.IsNaN(heightConstraint) || double.IsNegativeInfinity(heightConstraint)) {
				return zero;
			}
			this._viewBox.ContentMeasurementCache = this._measureFull(this._view);
			SizeRequest? contentMeasurementCache = this._viewBox.ContentMeasurementCache;
			if (contentMeasurementCache == null) { return zero; }
			Size request = contentMeasurementCache.Value.Request;
			return double.IsPositiveInfinity(widthConstraint) && double.IsPositiveInfinity(heightConstraint)
				? new SizeRequest(request)
				: double.IsPositiveInfinity(widthConstraint)
					? new SizeRequest(request * this._calculateScale(heightConstraint, request.Height))
					: double.IsPositiveInfinity(heightConstraint)
						? new SizeRequest(request * this._calculateScale(widthConstraint, request.Width))
						: new SizeRequest(request * Math.Min(
							this._calculateScale(widthConstraint, request.Width),
							this._calculateScale(heightConstraint, request.Height)));
		}
		#endregion

	}
	#endregion

}
