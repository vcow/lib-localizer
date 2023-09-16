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
		private object[] _formatArgs;
		private LocalString _localString;
		private TextMeshProUGUI _text;

		[SerializeField] private string _key;

		protected abstract ILocalizationManager LocalizationManager { get; }

		private void Start()
		{
			if (LocalizationManager == null)
			{
				Debug.LogWarning("ILocalizationService isn't present in LocaleTMPController.");
				return;
			}

			_text = GetComponent<TextMeshProUGUI>();
			_key ??= _text.text.Trim();
			_localString = new LocalString(LocalizationManager, _key, _formatArgs);
			_localString.ValueChangedEvent += OnLocalStringValueChanged;
			_text.text = _localString.Value;
		}

		private void OnLocalStringValueChanged(LocalString localString, string value)
		{
			_text.text = value;
		}

		private void OnDestroy()
		{
			_localString.ValueChangedEvent -= OnLocalStringValueChanged;
			_localString.Dispose();
		}
	}
}