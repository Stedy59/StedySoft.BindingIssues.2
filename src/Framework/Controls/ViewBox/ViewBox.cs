using System.ComponentModel;

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

namespace StedySoft.Maui.Framework.Controls {

	#region Class ViewBox
	[ContentProperty(nameof(ViewBox.Content))]
	public class ViewBox : Layout {

		#region Protected Overrides
		protected override ILayoutManager CreateLayoutManager() =>
			new ViewBoxLayoutManager(this);

		protected override void OnChildAdded(Element child) {
			if (child is View view) {
				view.PropertyChanged += this.OnChildViewPropertyChanged;
			}
			base.OnChildAdded(child);
		}

		protected override void OnChildRemoved(Element child, int oldLogicalIndex) {
			if (child is View view) {
				view.PropertyChanged -= this.OnChildViewPropertyChanged;
			}
			base.OnChildRemoved(child, oldLogicalIndex);
		}
		#endregion

		#region Bindable Properties
		public static readonly BindableProperty ContentProperty =
			BindableProperty.Create(
				nameof(ViewBox.Content),
				typeof(View),
				typeof(ViewBox),
				propertyChanged: (b, o, n) => {
					ViewBox vb = b.As<ViewBox>();
					vb.Children.Clear();
					vb.Children.Add(n.As<View>());
				});

		public View Content {
			get => (View)this.GetValue(ViewBox.ContentProperty);
			set => this.SetValue(ViewBox.ContentProperty, value);
		}

		public static readonly BindableProperty ScaleModeProperty =
			BindableProperty.Create(
				nameof(ViewBox.ScaleMode),
				typeof(ScaleModes),
				typeof(ViewBox),
				ScaleModes.ScaleBoth,
				propertyChanged: (b, o, n) => b.As<ViewBox>()?.InvalidateMeasure());

		public ScaleModes ScaleMode {
			get => (ScaleModes)this.GetValue(ViewBox.ScaleModeProperty);
			set => this.SetValue(ViewBox.ScaleModeProperty, value);
		}
		#endregion

		#region Internal Properties
		internal SizeRequest? ContentMeasurementCache { get; set; }
		#endregion

		#region Public Methods
		public void ClearSizeCache() =>
			this.ContentMeasurementCache = null;
		#endregion

		#region Events
		private void OnChildViewPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (sender is not View) { return; }
			if (e.PropertyName is "VerticalOptions" or "HorizontalOptions") { this.InvalidateMeasure(); }
		}
		#endregion

	}
	#endregion

}
