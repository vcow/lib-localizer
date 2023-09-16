using System;
using System.Collections.Generic;
using System.Linq;
using Base.Assignments.Initable;
using UnityEngine;
using UnityEngine.Assertions;

namespace Base.Localization.Template
{
	/// <summary>
	/// Localization provider for the ILocalizationManager that receive in input a CSV formatted strings where caption
	/// is a language abbreviation and left column is the string localization key.
	/// That provider works with the Unity resource assets as raw text sources and should receive that assets
	/// as a list of arguments when calling Init() method. If you wat to use some other sources, such as files or
	/// Internet resources override Init() method.
	/// </summary>
	public class CSVLocalizationProvider : ILocalizationProvider, IInitable
	{
		private List<LocaleEntry> _locales;
		private bool _isInited;

		// ILocalizationProvider

		public IEnumerable<LocaleEntry> Locales
		{
			get
			{
				Assert.IsTrue(_isInited, "CSVLocalizationProvider isn't initialized.");
				return _locales;
			}
		}

		// \ILocalizationProvider

		// IInitable

		public virtual void Init(params object[] args)
		{
			if (_isInited)
			{
				Debug.LogError("CSVLocalizationProvider is already initialized.");
				return;
			}

			var assets = args.Select(o => o as TextAsset).Where(asset => asset != null).ToArray();
			if (assets.Length == 0)
			{
				assets = args.Select(o => o as IEnumerable<TextAsset>)
					.Where(enumerable => enumerable != null)
					.SelectMany(enumerable => enumerable)
					.ToArray();
			}

			foreach (var textAsset in assets)
			{
				ApplyCsvRawData(textAsset.text);
			}

			IsInited = true;
		}

		public bool IsInited
		{
			get => _isInited;
			protected set
			{
				if (value == _isInited) return;
				_isInited = value;
				Assert.IsTrue(_isInited);
				InitCompleteEvent?.Invoke(this);
			}
		}

		public event InitCompleteHandler InitCompleteEvent;

		// \IInitable

		/// <summary>
		/// Parse raw CSV data, create LocaleEntry-s and add them to Locales list.
		/// </summary>
		/// <param name="rawData">The CSV formatted raw data string.</param>
		protected void ApplyCsvRawData(string rawData)
		{
			SystemLanguage[] header = null;
			using (var provider = CSV.Parse(rawData))
			{
				while (provider.MoveNext())
				{
					var columns = provider.Current;
					Assert.IsNotNull(columns);
					Assert.IsTrue(columns.Count > 1, "Invalid localization csv file.");

					if (header == null)
					{
						header = new[] { SystemLanguage.Unknown }.Concat(columns.Skip(1).Select(AsLanguage)).ToArray();
						if (_locales == null)
						{
							_locales = header.Select(language => new LocaleEntry(language)).ToList();
						}
						else
						{
							var unknownLanguages = header.Where(language =>
								language != SystemLanguage.Unknown &&
								_locales.All(entry => entry.Language != language));
							foreach (var unknownLanguage in unknownLanguages)
							{
								_locales.Add(new LocaleEntry(unknownLanguage));
							}
						}

						continue;
					}

					var key = columns[0];
					for (var i = 1; i < columns.Count; ++i)
					{
						var lang = header[i];
						var entry = _locales.First(localeEntry => localeEntry.Language == lang);
						entry.SetValue(key, columns[i]);
					}
				}
			}
		}

		/// <summary>
		/// Override that method to match the language abbreviation from the title and the localization language.
		/// </summary>
		/// <param name="raw">The language abbreviation from the CSV table header.</param>
		/// <returns>Returns SystemLanguage enumeration of the corresponding localization language.</returns>
		/// <exception cref="NotSupportedException">Thrown if unexpected abbreviation received.</exception>
		protected virtual SystemLanguage AsLanguage(string raw)
		{
			return raw switch
			{
				"ru" => SystemLanguage.Russian,
				"en" => SystemLanguage.English,
				"de" => SystemLanguage.German,
				"fr" => SystemLanguage.French,
				"ch" => SystemLanguage.Chinese,
				// TODO: Add other supported languages here.
				_ => throw new NotSupportedException($"The language {raw} isn't supported.")
			};
		}
	}
}