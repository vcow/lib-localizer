using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Base.Localization
{
	/// <summary>
	/// Value changed event handler delegate for the LocaleString.
	/// </summary>
	public delegate void LocalStringValueChangedHandler(LocalString localString, string value);

	/// <summary>
	/// Interactive string, which supports formatting for the localized text.
	/// </summary>
	public class LocalString : IDisposable
	{
		private readonly ILocalizationManager _localizationManager;
		private string _value;
		private string _key;
		private object[] _formatArgs;

		private bool _isDisposed;

		// IDisposable

		public void Dispose()
		{
			if (_isDisposed) return;
			_isDisposed = true;

			_localizationManager.CurrentLanguageChangedEvent -= OnCurrentLanguageChangedEvent;

			ValueChangedEvent = null;
		}

		// \IDisposable

		private void OnCurrentLanguageChangedEvent(ILocalizationManager localizationManager, SystemLanguage language)
		{
			OnUpdateValue(language);
		}

		public override string ToString()
		{
			return _value;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="localizationManager">The instance of ILocalizationManager.</param>
		/// <param name="key">String localization key.</param>
		/// <param name="formatArgs">Format string arguments (see System.String.Format() method).</param>
		public LocalString(ILocalizationManager localizationManager, string key, object[] formatArgs = null)
		{
			Assert.IsNotNull(localizationManager);

			_localizationManager = localizationManager;
			_key = key;
			_formatArgs = formatArgs;

			localizationManager.CurrentLanguageChangedEvent += OnCurrentLanguageChangedEvent;
			OnUpdateValue(_localizationManager.CurrentLanguage);
		}

		private void OnUpdateValue(SystemLanguage language)
		{
			if (_isDisposed) return;

			var localString = _localizationManager.GetLocalized(_key, language);
			if (_formatArgs != null)
			{
				localString = string.Format(localString, _formatArgs);
			}

			Value = localString;
		}

		/// <summary>
		/// Resulting formatted string.
		/// </summary>
		public string Value
		{
			get => _value;
			private set
			{
				if (value == _value) return;
				_value = value;
				ValueChangedEvent?.Invoke(this, _value);
			}
		}

		/// <summary>
		/// String value changed event.
		/// </summary>
		public event LocalStringValueChangedHandler ValueChangedEvent;

		/// <summary>
		/// The localization key.
		/// </summary>
		public string Key
		{
			set
			{
				if (value == _key) return;
				_key = value;
				OnUpdateValue(_localizationManager.CurrentLanguage);
			}
			get => _key;
		}

		/// <summary>
		/// Formatted string arguments.
		/// </summary>
		public IEnumerable<object> FormatArgs
		{
			set
			{
				_formatArgs = value?.ToArray();
				OnUpdateValue(_localizationManager.CurrentLanguage);
			}
			get => _formatArgs.ToArray();
		}
	}
}