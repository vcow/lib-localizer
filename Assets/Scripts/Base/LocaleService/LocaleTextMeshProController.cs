using System.Linq;
using TMPro;
using UnityEngine;

namespace Base.LocaleService
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LocaleTextMeshProController : MonoBehaviour, ILocaleController
	{
		private string _key;
		private object[] _formatArgs;
		private LocalString _localString;
		private ILocaleService _localeService;
		private TextMeshProUGUI _text;

		private void Start()
		{
			_text = GetComponent<TextMeshProUGUI>();
			_key = _key ?? _text.text.Trim();
			_localString = new LocalString(_localeService, _key, _formatArgs);
			_localString.TextChangedEvent += OnLocalTextChanged;
			_text.text = _localString.Value;
		}

		private void OnLocalTextChanged(LocalString localString, string oldText)
		{
			_text.text = localString.Value;
		}

		private void OnDestroy()
		{
			if (_localString != null)
			{
				_localString.TextChangedEvent -= OnLocalTextChanged;
				_localString.Dispose();
			}
		}

		// ILocaleController

		ILocaleService ILocaleController.LocaleService
		{
			set => _localeService = value;
		}

		public string Key
		{
			get => _key;
			set
			{
				_key = value;
				if (_localString != null) _localString.Key = value;
			}
		}

		public string Format(params object[] args)
		{
			_formatArgs = args != null && args.Length > 0 ? args.ToArray() : null;
			if (_localString == null) return null;

			_localString.FormatArgs = _formatArgs;
			return _localString.Value;
		}

		// \ILocaleController
	}
}