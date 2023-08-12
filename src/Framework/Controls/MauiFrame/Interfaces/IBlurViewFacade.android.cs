namespace StedySoft.Maui.Framework.Controls {

	#region Interface IBlurViewFacade
	public interface IBlurViewFacade {

		IBlurViewFacade SetBlurEnabled(bool enabled);

		IBlurViewFacade SetBlurAutoUpdate(bool enabled);

		IBlurViewFacade SetHasFixedTransformationMatrix(bool hasFixedTransformationMatrix);

		IBlurViewFacade SetBlurRadius(float radius);

		IBlurViewFacade SetOverlayColor(int overlayColor);

	}
	#endregion

}