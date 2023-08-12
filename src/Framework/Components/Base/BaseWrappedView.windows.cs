using System;

using Microsoft.UI.Xaml.Controls;

using MRect = Microsoft.Maui.Graphics.Rect;
using MSize = Microsoft.Maui.Graphics.Size;

using WRect = Windows.Foundation.Rect;
using WSize = Windows.Foundation.Size;

namespace StedySoft.Maui.Framework.Controls {

	#region Abstract Class BaseWrappedView
	public abstract class BaseWrappedView : Grid {

		internal Func<double, double, MSize>? CrossPlatformMeasure { get; set; }
		internal Func<MRect, MSize>? CrossPlatformArrange { get; set; }

		protected override WSize MeasureOverride(WSize availableSize) {
			if (this.CrossPlatformMeasure == null || (availableSize.Width * availableSize.Height == 0)) {
				return base.MeasureOverride(availableSize);
			}
			MSize size = this.CrossPlatformMeasure(availableSize.Width, availableSize.Height);
			return new WSize(size.Width, size.Height);
		}

		protected override WSize ArrangeOverride(WSize finalSize) {
			if (this.CrossPlatformArrange == null) { return base.ArrangeOverride(finalSize); }
			MSize actual = this.CrossPlatformArrange(new MRect(0, 0, finalSize.Width, finalSize.Height));
			return new WSize(Math.Max(0, actual.Width), Math.Max(0, actual.Height));
		}

		public void Layout(int l, int t, int r, int b) =>
			this.Arrange(new WRect(l, t, r, b));

	}
	#endregion

}