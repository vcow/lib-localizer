using Base.Localization;
using Base.Localization.Template;
using Zenject;

namespace Sample
{
	public sealed class LocaleTMPController : LocaleTMPControllerBase
	{
		[Inject] private readonly ILocalizationManager _localizationManager;
		protected override ILocalizationManager LocalizationManager => _localizationManager;
	}
}