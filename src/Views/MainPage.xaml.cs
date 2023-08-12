using Microsoft.Maui.Controls;

using StedySoft.Maui.Framework.Controls;

namespace StedySoft.BindingIssues.Views {

	public partial class MainPage : ContentPage {

		public MainPage() =>
			this.InitializeComponent();

		protected override void OnDisappearing() =>
			ClockManager.Current.Stop();

	}

}
