using System;
using Base.LocaleService;
using UnityEngine;

namespace Sample
{
	public class LocaleService : LocaleServiceBase
	{
		protected override string LocalePersistKey => @"test_locale_key";
		protected override string LocalesManifestFileName => @"manifest";
		protected override string LocalesPath => @"Locales";

		protected override bool IsLanguageSupported(SystemLanguage lang)
		{
			switch (lang)
			{
				case SystemLanguage.Russian:
				case SystemLanguage.English:
					// TODO: Add supported languages here.
					return true;
			}

			return false;
		}

		protected override SystemLanguage KeyToLanguage(string key)
		{
			switch (key)
			{
				case "ru_ru": return SystemLanguage.Russian;
				case "en_us": return SystemLanguage.English;
				default:
					throw new NotSupportedException($"Language {key} is not supported.");
			}
		}
	}
}