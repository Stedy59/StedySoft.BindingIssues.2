using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

using WButton = Microsoft.UI.Xaml.Controls.Button;
using WImage = Microsoft.UI.Xaml.Controls.Image;

namespace StedySoft.Maui.Framework.Controls {

	public partial class IconTintColorBehavior {

		SpriteVisual? currentSpriteVisual;
		CompositionColorBrush? currentColorBrush;

		protected override void OnAttachedTo(View bindable, FrameworkElement platformView) {
			this.ApplyTintColor(platformView, bindable, this.TintColor);

			bindable.PropertyChanged += this.OnElementPropertyChanged;
			this.PropertyChanged += (s, e) => {
				if (e.PropertyName == TintColorProperty.PropertyName) {
					if (this.currentColorBrush is not null && this.TintColor is not null) {
						this.currentColorBrush.Color = this.TintColor.ToWindowsColor();
					}
					else {
						this.ApplyTintColor(platformView, bindable, this.TintColor);
					}
				}
			};
		}

		protected override void OnDetachedFrom(View bindable, FrameworkElement platformView) {
			bindable.PropertyChanged -= this.OnElementPropertyChanged;
			this.RemoveTintColor(platformView);
		}

		static bool TryGetButtonImage(WButton button, [NotNullWhen(true)] out WImage? image) {
			image = button.Content as WImage;
			return image is not null;
		}

		static bool TryGetSourceImageUri(WImage? imageControl, IImageElement? imageElement, [NotNullWhen(true)] out Uri? uri) {
			if (imageElement?.Source is UriImageSource uriImageSource) {
				uri = uriImageSource.Uri;
				return true;
			}

			if (imageControl?.Source is BitmapImage bitmapImage) {
				uri = bitmapImage.UriSource;
				return true;
			}

			uri = null;
			return false;
		}

		void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e) {
			if (e.PropertyName is not string propertyName
				|| sender is not View bindable
				|| bindable.Handler?.PlatformView is not FrameworkElement platformView) {
				return;
			}

			if (propertyName.Equals(Image.SourceProperty.PropertyName, StringComparison.Ordinal)
				|| propertyName.Equals(ImageButton.SourceProperty.PropertyName, StringComparison.Ordinal)) {
				this.ApplyTintColor(platformView, bindable, this.TintColor);
			}
		}

		void ApplyTintColor(FrameworkElement platformView, View element, Color? color) {
			this.RemoveTintColor(platformView);

			if (color is null) {
				return;
			}

			switch (platformView) {
				case WImage wImage:
					this.LoadAndApplyImageTintColor(element, wImage, color);
					break;

				case WButton button:
					if (!TryGetButtonImage(button, out WImage image)) {
						return;
					}

					this.LoadAndApplyImageTintColor(element, image, color);
					break;

				default:
					throw new NotSupportedException($"{nameof(IconTintColorBehavior)} only currently supports {typeof(WImage)} and {typeof(WButton)}.");
			}
		}

		void LoadAndApplyImageTintColor(View element, WImage image, Color color) {
			if (image.IsLoaded) {
				ApplyTintColor();
			}
			else {
				image.ImageOpened += OnImageOpened;
			}

			void OnImageOpened(object sender, RoutedEventArgs e) {
				image.ImageOpened -= OnImageOpened;

				ApplyTintColor();
			}

			void ApplyTintColor() {
				if (image.ActualSize != Vector2.Zero) {
					this.ApplyImageTintColor(element, image, color);
				}
				else {
					image.SizeChanged += OnImageSizeChanged;

					void OnImageSizeChanged(object sender, SizeChangedEventArgs e) {
						image.SizeChanged -= OnImageSizeChanged;
						this.ApplyImageTintColor(element, image, color);
					}
				}
			}
		}

		void ApplyImageTintColor(View element, WImage image, Color color) {
			if (!TryGetSourceImageUri(image, (IImageElement)element, out Uri uri)) {
				return;
			}

			float width = (float)image.ActualWidth;
			float height = (float)image.ActualHeight;
			Vector2 anchorPoint = new((float)element.AnchorX, (float)element.AnchorY);

			image.Width = image.Height = 0;

			bool requiresAdditionalCenterOffset = element.WidthRequest != -1 || element.HeightRequest != -1;
			Vector3 offset = requiresAdditionalCenterOffset ? new Vector3(width * anchorPoint.X, height * anchorPoint.Y, 0f) : Vector3.Zero;

			this.ApplyTintCompositionEffect(image, color, width, height, offset, anchorPoint, uri);
		}

		void ApplyTintCompositionEffect(FrameworkElement platformView, Color color, float width, float height, Vector3 offset, Vector2 anchorPoint, Uri surfaceMaskUri) {
			Compositor compositor = ElementCompositionPreview.GetElementVisual(platformView).Compositor;

			this.currentColorBrush = compositor.CreateColorBrush();
			this.currentColorBrush.Color = color.ToWindowsColor();

			LoadedImageSurface loadedSurfaceMask = LoadedImageSurface.StartLoadFromUri(surfaceMaskUri);

			CompositionMaskBrush maskBrush = compositor.CreateMaskBrush();
			maskBrush.Source = this.currentColorBrush;
			maskBrush.Mask = compositor.CreateSurfaceBrush(loadedSurfaceMask);

			this.currentSpriteVisual = compositor.CreateSpriteVisual();
			this.currentSpriteVisual.Brush = maskBrush;
			this.currentSpriteVisual.Size = new Vector2(width, height);
			this.currentSpriteVisual.Offset = offset;
			this.currentSpriteVisual.AnchorPoint = anchorPoint;

			ElementCompositionPreview.SetElementChildVisual(platformView, this.currentSpriteVisual);
		}

		void RemoveTintColor(FrameworkElement platformView) {
			if (this.currentSpriteVisual is null) {
				return;
			}

			switch (platformView) {
				case WImage wImage:
					this.RestoreOriginalImageSize(wImage);
					ElementCompositionPreview.SetElementChildVisual(platformView, null);
					break;

				case WButton button:
					if (TryGetButtonImage(button, out WImage image)) {
						this.RestoreOriginalImageSize(image);
						ElementCompositionPreview.SetElementChildVisual(image, null);
					}
					break;
			}

			this.currentSpriteVisual.Brush = null;
			this.currentSpriteVisual = null;
			this.currentColorBrush = null;
		}

		void RestoreOriginalImageSize(WImage image) {
			if (this.currentSpriteVisual is null) {
				return;
			}

			image.Width = this.currentSpriteVisual.Size.X;
			image.Height = this.currentSpriteVisual.Size.Y;
		}

	}

}
