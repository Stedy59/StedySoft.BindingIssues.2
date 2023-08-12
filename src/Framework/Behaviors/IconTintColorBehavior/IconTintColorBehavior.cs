using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace StedySoft.Maui.Framework.Controls {

	public partial class IconTintColorBehavior : PlatformBehavior<View> {

		public static readonly BindableProperty TintColorProperty =
			BindableProperty.Create(
				nameof(IconTintColorBehavior.TintColor),
				typeof(Color),
				typeof(IconTintColorBehavior),
				default);

		public Color? TintColor {
			get => (Color?)this.GetValue(IconTintColorBehavior.TintColorProperty);
			set => this.SetValue(IconTintColorBehavior.TintColorProperty, value);
		}

	}

}
