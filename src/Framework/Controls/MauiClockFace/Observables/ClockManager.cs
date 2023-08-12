using System;
using System.Threading.Tasks;

namespace StedySoft.Maui.Framework.Controls {

	#region Class ClockManager
	public sealed class ClockManager : ObservableObject {

		#region Singleton Design Pattern
		private static readonly Lazy<ClockManager> _current =
			new(() => new ClockManager());

		public static ClockManager Current =>
			ClockManager._current.Value;
		#endregion

		#region Declarations

		private string _hour;
		private string _minute;
		private string _second;
		private string _amPm;

		private double _hourAngle;
		private double _minuteAngle;
		private double _secondAngle;
		#endregion

		#region Constructor
		private ClockManager() { }
		#endregion

		#region Private Methods
		private async Task _updateLoop() {
			while (this.IsRunning) {
				DateTime dt = DateTime.Now;

				this.Hour = dt.ToString("hh");
				this.Minutes = dt.ToString("mm");
				this.Seconds = dt.ToString("ss");
				this.AmPm = " " + dt.ToString("tt");

				this.HourAngle = 30 * (dt.Hour % 12) + 0.5 * dt.Minute;
				this.MinuteAngle = 6 * dt.Minute + 0.1 * dt.Second;
				this.SecondAngle = 6 * dt.Second + 0.006 * dt.Millisecond;

				await Task.Delay(16);
			}
		}
		#endregion

		#region Properties
		public bool IsRunning { get; private set; }

		public string Hour {
			get => this._hour;
			set => this.SetProperty(ref this._hour, value);
		}

		public string Minutes {
			get => this._minute;
			set => this.SetProperty(ref this._minute, value);
		}

		public string Seconds {
			get => this._second;
			set => this.SetProperty(ref this._second, value);
		}

		public string AmPm {
			get => this._amPm;
			set => this.SetProperty(ref this._amPm, value);
		}

		public double HourAngle {
			get => this._hourAngle;
			set => this.SetProperty(ref this._hourAngle, value);
		}

		public double MinuteAngle {
			get => this._minuteAngle;
			set => this.SetProperty(ref this._minuteAngle, value);
		}

		public double SecondAngle {
			get => this._secondAngle;
			set => this.SetProperty(ref this._secondAngle, value);
		}
		#endregion

		#region Public Methods
		public async Task StartAsync() {
			if (!this.IsRunning) { this.IsRunning = true;  await this._updateLoop(); }
		}
		public void Stop() =>
			this.IsRunning = false;
		#endregion

	}
	#endregion

}
