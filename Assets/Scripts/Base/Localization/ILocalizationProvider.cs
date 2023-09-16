using System.Collections.Generic;

namespace Base.Localization
{
	/// <summary>
	/// The Localization Provider interface. Localization Provider is the source of localized strings
	/// for the ILocalizationService implementation. A whole content of the Localization Provider must be presented
	/// (downloaded) before being sent to the LocalizationService.
	/// </summary>
	public interface ILocalizationProvider
	{
		/// <summary>
		/// A list of the localized strings.
		/// </summary>
		IEnumerable<LocaleEntry> Locales { get; }
	}
}