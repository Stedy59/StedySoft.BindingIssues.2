using System;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace StedySoft.Maui.Framework.Controls {

	#region Class MauiClockFace
	public class MauiClockFace : MauiFrame, IDisposable {

		#region Declarations
		private bool _disposed;

		private Grid _clockGrid { get; set; }
		private Image _imgClockHour { get; set; }
		private Image _imgClockMinute { get; set; }
		private Image _imgClockSecond { get; set; }
		#endregion

		#region Constructor
		public MauiClockFace() =>
			this.ControlTemplate = Application.Current.Resources.TryGetTemplate("MauiClockFaceTemplate");

		protected virtual void Dispose(bool disposing) {
			if (!this._disposed) {
				ClockManager.Current.Stop();
				this._disposed = true;
			}
		}

		public void Dispose() {
			this.Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion

		#region Protected Overrides
		protected override void OnApplyTemplate() {
			base.OnApplyTemplate();

			this._clockGrid = this.GetTemplateChild("clockGrid").As<Grid>();
			this._imgClockHour ??= this._clockGrid?.FindByName<Image>("imgClockHour");
			this._imgClockMinute ??= this._clockGrid?.FindByName<Image>("imgClockMinute");
			this._imgClockSecond ??= this._clockGrid?.FindByName<Image>("imgClockSecond");

			_ = this.Dispatcher.Dispatch(() => {
				DateTime dt = DateTime.Now;
				new Animation {
					{ 0, 1, new Animation(v => this._imgClockHour.Rotation = v, 0, 30 * (dt.Hour % 12) + 0.5 * dt.Minute) },
					{ 0, 1, new Animation(v => this._imgClockMinute.Rotation = v, 0, 6 * dt.Minute + 0.1 * dt.Second) },
					{ 0, 1, new Animation(v => this._imgClockSecond.Rotation = v, 0, 6 * dt.Second + 0.006 * dt.Millisecond) }
				}.Commit(
					this,
					"CurrentTimeAnimation",
					length: (uint)TimeSpan.FromSeconds(1.5).TotalMilliseconds,
					easing: Easing.SinOut,
					finished: async (v, c) => {
						if (this.UseBinding) {
							_ = this._imgClockHour.Bind(Image.RotationProperty, path: "HourAngle", source: ClockManager.Current);
							_ = this._imgClockMinute.Bind(Image.RotationProperty, path: "MinuteAngle", source: ClockManager.Current);
							_ = this._imgClockSecond.Bind(Image.RotationProperty, path: "SecondAngle", source: ClockManager.Current);
						}
						else {
							ClockManager.Current.PropertyChanged += (s, e) => {
								switch (e.PropertyName) {
									case "HourAngle":
										this._imgClockHour.Rotation = ClockManager.Current.HourAngle; break;
									case "MinuteAngle":
										this._imgClockMinute.Rotation = ClockManager.Current.MinuteAngle; break;
									case "SecondAngle":
										this._imgClockSecond.Rotation = ClockManager.Current.SecondAngle; break;
								}
							};
						}
						await ClockManager.Current.StartAsync();
					});
			});
		}
		#endregion

		#region BindableProperties
		public static readonly BindableProperty UseBindingProperty =
			BindableProperty.Create(
				nameof(MauiClockFace.UseBinding),
				typeof(bool),
				typeof(MauiClockFace),
				false);

		public bool UseBinding {
			get => (bool)this.GetValue(UseBindingProperty);
			set => this.SetValue(UseBindingProperty, value);
		}
		#endregion

	}
	#endregion

}
