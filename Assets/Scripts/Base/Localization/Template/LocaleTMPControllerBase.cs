using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Base.Localization.Template
{
	/// <summary>
	/// A base class of the interactive localization controller for the UI TextMeshPro game object. Tracks changes
	/// of the current selected language and change the text in the TextMeshPro component respectively. Can receive
	/// the list of formatted string arguments. Use initial text of the TextMeshPro component as a localization key.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshProUGUI))]
	public abstract class LocaleTMPControllerBase : MonoBehaviour
	{
		private LocalString _localString;
		private TextMeshProUGUI _text;

		[SerializeField] private bool _localizeText = true;
		[SerializeField] private bool _interactWithManager = true;
		[SerializeField] private string _key;
		[SerializeField] private List<string> _formatArguments;

		protected abstract ILocalizationManager LocalizationManager { get; }

		protected virtual void Start()
		{
			if (!_localizeText)
			{
				return;
			}

			if (LocalizationManager == null)
			{
				Debug.LogWarning("ILocalizationService isn't present in LocaleTMPController.");
				return;
			}

			_text = GetComponent<TextMeshProUGUI>();
			if (string.IsNullOrEmpty(_key))
			{
				_key = _text.text.Trim();
			}

			_localString = new LocalString(LocalizationManager, _key, _formatArguments?.Cast<object>().ToArray());
			_text.text = _localString.Value;

			if (_interactWithManager)
			{
				_localString.ValueChangedEvent += OnLocalStringValueChanged;
			}
		}

		private void OnLocalStringValueChanged(LocalString localString, string value)
		{
			_text.text = value;
		}

		protected virtual void OnDestroy()
		{
			if (_localString == null)
			{
				return;
			}

			if (_interactWithManager)
			{
				_localString.ValueChangedEvent -= OnLocalStringValueChanged;
			}

			_localString.Dispose();
		}
	}
}