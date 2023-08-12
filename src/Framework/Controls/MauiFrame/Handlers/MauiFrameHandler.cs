using Microsoft.Maui;

namespace StedySoft.Maui.Framework.Controls {

	#region Class MauiFrameHandler
	public partial class MauiFrameHandler : IViewHandler {

		#region Constructor
		public MauiFrameHandler()
			: base(Mapper, CommandMapper) { }

		public MauiFrameHandler(IPropertyMapper? mapper)
			: base(mapper ?? Mapper, CommandMapper) { }

		public MauiFrameHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
			: base(mapper ?? Mapper, commandMapper ?? CommandMapper) { }

		public static IPropertyMapper<MauiFrame, MauiFrameHandler> Mapper = new PropertyMapper<MauiFrame, MauiFrameHandler>(ViewMapper) {
			[nameof(MauiFrame.Background)] = MapBackgroundColor,
			[nameof(IContentView.Content)] = MapContent,
			[nameof(MauiFrame.CornerRadius)] = MapCornerRadius,
			[nameof(MauiFrame.FrameStyle)] = MapFrameStyle,
			[nameof(MauiFrame.HighlightColor)] = MapHighlightColor,
			[nameof(MauiFrame.HighlightThickness)] = MapHighlightThickness,
			[nameof(MauiFrame.TintColor)] = MapTintColor,
			[nameof(MauiFrame.TintOpacity)] = MapTintOpacity
		};

		public static CommandMapper<MauiFrame, MauiFrameHandler> CommandMapper = new(ViewCommandMapper) { };
		#endregion

	}
	#endregion

}
