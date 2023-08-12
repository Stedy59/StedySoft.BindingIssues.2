using Microsoft.Maui.Hosting;

namespace StedySoft.BindingIssues {

	#region Class MauiProgram
	public static partial class MauiProgram {

		public static MauiAppBuilder ConfigurePlatform(this MauiAppBuilder builder) => builder;

	}
	#endregion

}
