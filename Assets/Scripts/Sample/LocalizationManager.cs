using Base.Localization;
using UnityEngine;

namespace Sample
{
	public class LocalizationManager : LocalizationManagerBase<LocaleTextController, LocaleTMPController>
	{
		public LocalizationManager(ILocalizationProvider localizationProvider, SystemLanguage defaultLanguage)
			: base(localizationProvider, defaultLanguage)
		{
		}
	}
}