using System;
using System.ComponentModel;
using System.Linq;

using Android.Graphics;
using Android.Widget;
using AView = Android.Views.View;
using AButton = Android.Widget.Button;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using MColor = Microsoft.Maui.Graphics.Color;
using MColors = Microsoft.Maui.Graphics.Colors;
using MImageButton = Microsoft.Maui.Controls.ImageButton;

namespace StedySoft.Maui.Framework.Controls {

	public partial class IconTintColorBehavior {

		protected override void OnAttachedTo(View bindable, AView platformView) {
			this.ApplyTintColor(bindable, platformView);

			this.PropertyChanged += (s, e) => {
				if (e.PropertyName == TintColorProperty.PropertyName) {
					this.ApplyTintColor(bindable, platformView);
				}
			};
		}

		protected override void OnDetachedFrom(View bindable, AView platformView) =>
			this.ClearTintColor(bindable, platformView);

		void ApplyTintColor(View element, AView control) {
			MColor color = this.TintColor;
			element.PropertyChanged += this.OnElementPropertyChanged;

			switch (control) {
				case ImageView image:
					SetImageViewTintColor(image, color);
					break;
				case AButton button:
					SetButtonTintColor(button, color);
					break;
				default:
					throw new NotSupportedException($"{nameof(IconTintColorBehavior)} only currently supports Android.Widget.Button and {nameof(ImageView)}.");
			}


			static void SetImageViewTintColor(ImageView image, MColor? color) {
				if (color is null) {
					image.ClearColorFilter();
					color = MColors.Transparent;
				}

				image.SetColorFilter(new PorterDuffColorFilter(color.ToPlatform(), PorterDuff.Mode.SrcIn ?? throw new InvalidOperationException("PorterDuff.Mode.SrcIn should not be null at runtime.")));
			}

			static void SetButtonTintColor(AButton button, MColor? color) {
				System.Collections.Generic.IEnumerable<Android.Graphics.Drawables.Drawable> drawables = button.GetCompoundDrawables().Where(d => d is not null);

				if (color is null) {
					foreach (Android.Graphics.Drawables.Drawable img in drawables) {
						img.ClearColorFilter();
					}
					color = MColors.Transparent;
				}

				foreach (Android.Graphics.Drawables.Drawable img in drawables) {
					img.SetTint(color.ToPlatform());
				}
			}
		}

		void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs args) {
			if (args.PropertyName is not string propertyName
				|| sender is not View bindable
				|| bindable.Handler?.PlatformView is not AView platformView) {
				return;
			}

			if (!propertyName.Equals(Image.SourceProperty.PropertyName, StringComparison.Ordinal)
				&& !propertyName.Equals(MImageButton.SourceProperty.PropertyName, StringComparison.Ordinal)) {
				return;
			}

			this.ApplyTintColor(bindable, platformView);
		}

		void ClearTintColor(View element, AView control) {
			element.PropertyChanged -= this.OnElementPropertyChanged;
			switch (control) {
				case ImageView image:
					image.ClearColorFilter();
					break;
				case AButton button:
					foreach (Android.Graphics.Drawables.Drawable drawable in button.GetCompoundDrawables()) {
						drawable?.ClearColorFilter();
					}
					break;
			}
		}

	}

}
