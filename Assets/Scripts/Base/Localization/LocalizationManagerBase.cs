using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Base.Localization.Template;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Localization
{
	/// <summary>
	/// A base class for the LocalizationManager.
	/// </summary>
	/// <typeparam name="TTextController">A type of the controller component that adds to the UI Text object
	/// when Localize() method with the applyController flag is calling.</typeparam>
	/// <typeparam name="TTMPController">A type of the controller component that adds to the UI TextMeshPro object
	/// /// when Localize() method with the applyController flag is calling.</typeparam>
	public abstract class LocalizationManagerBase<TTextController, TTMPController> : ILocalizationManager
		where TTextController : LocaleTextControllerBase
		where TTMPController : LocaleTMPControllerBase
	{
		protected virtual string LanguagePersistKey => "localization_lang_persist";

		private readonly Lazy<Dictionary<SystemLanguage, LocaleEntry>> _localesMap;

		private readonly Regex _transRegex = new Regex(@"(?:\\n|\\r\\n)");

		private SystemLanguage? _currentLanguage;
		private readonly SystemLanguage _defaultLanguage;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="localizationProvider">Localization provider for the Manager.</param>
		/// <param name="defaultLanguage">A default language which will be taken as current if there is no
		/// specified language given by user.</param>
		protected LocalizationManagerBase(ILocalizationProvider localizationProvider, SystemLanguage defaultLanguage)
		{
			_defaultLanguage = defaultLanguage;
			_localesMap = new Lazy<Dictionary<SystemLanguage, LocaleEntry>>(() =>
				localizationProvider.Locales
					.GroupBy(entry => entry.Language)
					.Select(entries =>
					{
						var entry = entries.First();
#if DEBUG || UNITY_EDITOR
						var entriesCount = entries.Count();
						if (entriesCount > 1)
						{
							Debug.LogErrorFormat("The language {0} has {1} entries.", entries.Key, entriesCount);
						}
#endif
						return entry;
					})
					.ToDictionary(entry => entry.Language));
		}

		// ILocalizationService

		public SystemLanguage CurrentLanguage
		{
			get => _currentLanguage ??= RestoreServiceState(_defaultLanguage);
			protected set
			{
				if (value == _currentLanguage) return;
				_currentLanguage = value;
				CurrentLanguageChangedEvent?.Invoke(this, value);
			}
		}

		public event CurrentLanguageChangedHandler CurrentLanguageChangedEvent;

		public virtual void SetCurrentLanguage(SystemLanguage lang)
		{
			if (lang == CurrentLanguage)
			{
				return;
			}

			CurrentLanguage = lang;
			PlayerPrefs.SetInt(LanguagePersistKey, (int)CurrentLanguage);
		}

		public string GetLocalized(string key)
		{
			return GetLocalized(key, CurrentLanguage);
		}

		public string GetLocalized(string key, SystemLanguage language)
		{
			try
			{
				var currentLocale = _localesMap.Value[language];
				var value = currentLocale.GetValue(key);
				return string.IsNullOrEmpty(value) ? key : ProcessRawString(value);
			}
			catch (KeyNotFoundException)
			{
				return key;
			}
		}

		public void Localize(GameObject ui, bool applyController = false, bool includeInactive = true)
		{
			var text = ui.GetComponentsInChildren<Text>(includeInactive);
			foreach (var t in text)
			{
				if (t.GetComponent<TTextController>() != null)
				{
					continue;
				}

				if (applyController)
				{
					ApplyLocaleTextController(t);
				}
				else
				{
					t.text = GetLocalized(t.text.Trim());
				}
			}

			var textPro = ui.GetComponentsInChildren<TextMeshProUGUI>(includeInactive);
			foreach (var tmp in textPro)
			{
				if (tmp.GetComponent<TTMPController>() != null)
				{
					continue;
				}

				if (applyController)
				{
					ApplyLocaleTMPController(tmp);
				}
				else
				{
					tmp.text = GetLocalized(tmp.text.Trim());
				}
			}
		}

		public void ReplaceWithProvider(ILocalizationProvider provider, bool clear = false)
		{
			var localesMap = _localesMap.Value;
			if (clear)
			{
				localesMap.Clear();
			}

			if (provider == null)
			{
				return;
			}

			foreach (var entry in provider.Locales
				         .GroupBy(entry => entry.Language)
				         .Select(entries =>
				         {
					         var entry = entries.First();
#if DEBUG || UNITY_EDITOR
					         var entriesCount = entries.Count();
					         if (entriesCount > 1)
					         {
						         Debug.LogErrorFormat("The language {0} has {1} entries.", entries.Key,
							         entriesCount);
					         }
#endif
					         return entry;
				         }))
			{
				if (localesMap.TryGetValue(entry.Language, out var oldEntry))
				{
					foreach (var key in entry)
					{
						oldEntry.SetValue(key, entry.GetValue(key));
					}
				}
				else
				{
					localesMap.Add(entry.Language, entry);
				}
			}
		}

		// \ILocalizationService

		/// <summary>
		/// Override this method in derived class if you need some additional LocaleTextController initializations.
		/// </summary>
		/// <param name="text">The UI Text to which is added LocaleTextController component.</param>
		protected virtual void ApplyLocaleTextController(Text text)
		{
			text.gameObject.AddComponent<TTextController>();
		}

		/// <summary>
		/// Override this method in derived class if you need some additional LocaleTMPController initializations.
		/// </summary>
		/// <param name="text">The UI TextMeshProUGUI to which is added LocaleTextTMPController component.</param>
		protected virtual void ApplyLocaleTMPController(TextMeshProUGUI text)
		{
			text.gameObject.AddComponent<TTMPController>();
		}

		/// <summary>
		/// Override this method in derived class if you using an alternative storage system for the selected language.
		/// To use your system for record selected language override SetCurrentLanguage() virtual method.
		/// </summary>
		/// <param name="defaultLanguage">The default language.</param>
		protected virtual SystemLanguage RestoreServiceState(SystemLanguage defaultLanguage)
		{
			var persist = false;
			var lang = defaultLanguage;
			if (PlayerPrefs.HasKey(LanguagePersistKey))
			{
				var i = PlayerPrefs.GetInt(LanguagePersistKey);
				persist = !Enum.IsDefined(typeof(SystemLanguage), i);
				if (!persist)
				{
					lang = (SystemLanguage)i;
				}
			}

			if (persist)
			{
				PlayerPrefs.SetInt(LanguagePersistKey, (int)CurrentLanguage);
			}

			return lang;
		}

		private string ProcessRawString(string raw)
		{
			var res = _transRegex.Replace(raw, "\n");
			return res;
		}
	}
}