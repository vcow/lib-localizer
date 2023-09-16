using UnityEngine;

namespace Base.Localization
{
	/// <summary>
	/// Event handler delegate for the current language changed event.
	/// </summary>
	public delegate void CurrentLanguageChangedHandler(ILocalizationManager localizationManager,
		SystemLanguage language);

	/// <summary>
	/// String localization manager.
	/// </summary>
	public interface ILocalizationManager
	{
		/// <summary>
		/// The key of current localization.
		/// </summary>
		SystemLanguage CurrentLanguage { get; }

		/// <summary>
		/// A current language changed event.
		/// </summary>
		event CurrentLanguageChangedHandler CurrentLanguageChangedEvent;

		/// <summary>
		/// Set the current localization language.
		/// </summary>
		/// <param name="lang">A key of the new localization language.</param>
		void SetCurrentLanguage(SystemLanguage lang);

		/// <summary>
		/// Get localized string by the key.
		/// </summary>
		/// <param name="key">The string key.</param>
		/// <returns>The localized value, or key if there is no value for the current language.</returns>
		string GetLocalized(string key);

		/// <summary>
		/// Get localized string by the key for the specified language.
		/// </summary>
		/// <param name="key">The string key.</param>
		/// <param name="language">The language for which localization is requested.</param>
		/// <returns>The localized value, or key if there is no value for the current locale.</returns>
		string GetLocalized(string key, SystemLanguage language);

		/// <summary>
		/// Localize specified UI.
		/// </summary>
		/// <param name="ui">The root object of the localized UI.</param>
		/// <param name="applyController">A flag indicating to apply a controller to all of the found text elements
		/// in order to track changes of the current language.</param>
		/// <param name="includeInactive">Apply also to inactive objects.</param>
		public void Localize(GameObject ui, bool applyController = false, bool includeInactive = true);

		/// <summary>
		/// Replace/append existing localization data with the new data from specified localization provider.
		/// </summary>
		/// <param name="provider">Localization provider with the new data.</param>
		/// <param name="clear">If true all the records that doesn't exist in the new provider will be deleted.</param>
		public void ReplaceWithProvider(ILocalizationProvider provider, bool clear = false);
	}
}