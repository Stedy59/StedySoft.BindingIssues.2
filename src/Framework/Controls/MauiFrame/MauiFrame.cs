using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace StedySoft.Maui.Framework.Controls {

	#region Class MauiFrame
	public class MauiFrame : ContentView {

		#region Bindable Properties
		public static readonly BindableProperty CornerRadiusProperty =
			BindableProperty.Create(
				nameof(MauiFrame.CornerRadius),
				typeof(double),
				typeof(MauiFrame),
				0d);

		public double CornerRadius {
			get => (double)this.GetValue(MauiFrame.CornerRadiusProperty);
			set => this.SetValue(MauiFrame.CornerRadiusProperty, value);
		}

		public static readonly BindableProperty HighlightColorProperty =
			BindableProperty.Create(
				nameof(MauiFrame.HighlightColor),
				typeof(Color),
				typeof(MauiFrame),
				Colors.White);

		public Color HighlightColor {
			get => (Color)this.GetValue(MauiFrame.HighlightColorProperty);
			set => this.SetValue(MauiFrame.HighlightColorProperty, value);
		}

		public static readonly BindableProperty HighlightThicknessProperty =
			BindableProperty.Create(
				nameof(MauiFrame.HighlightThickness),
				typeof(double),
				typeof(MauiFrame),
				2.15d);

		public double HighlightThickness {
			get => (double)this.GetValue(MauiFrame.HighlightThicknessProperty);
			set => this.SetValue(MauiFrame.HighlightThicknessProperty, value);
		}

		public static readonly BindableProperty FrameStyleProperty =
			BindableProperty.Create(
				nameof(MauiFrame.Style),
				typeof(FrameStyle),
				typeof(MauiFrame),
				FrameStyle.Highlight);

		public FrameStyle FrameStyle {
			get => (FrameStyle)this.GetValue(MauiFrame.FrameStyleProperty);
			set => this.SetValue(MauiFrame.FrameStyleProperty, value);
		}

		public static readonly BindableProperty TintColorProperty =
			BindableProperty.Create(
				nameof(MauiFrame.TintColor),
				typeof(Color),
				typeof(MauiFrame),
				Colors.White);

		public Color TintColor {
			get => (Color)this.GetValue(MauiFrame.TintColorProperty);
			set => this.SetValue(MauiFrame.TintColorProperty, value);
		}

		public static readonly BindableProperty TintOpacityProperty =
			BindableProperty.Create(
				nameof(MauiFrame.TintOpacity),
				typeof(double),
				typeof(MauiFrame),
				0.15d);

		public double TintOpacity {
			get => (double)this.GetValue(MauiFrame.TintOpacityProperty);
			set => this.SetValue(MauiFrame.TintOpacityProperty, value);
		}
		#endregion

	}
	#endregion

}
