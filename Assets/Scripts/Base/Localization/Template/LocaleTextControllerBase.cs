using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Localization.Template
{
	/// <summary>
	/// A base class of the interactive localization controller for the UI Text game object. Tracks changes of the
	/// current selected language and change the text in the Text component respectively. Can receive the list
	/// of formatted string arguments. Use initial text of the Text component as a localization key.
	/// </summary>
	[DisallowMultipleComponent, RequireComponent(typeof(Text))]
	public abstract class LocaleTextControllerBase : MonoBehaviour
	{
		private LocalString _localString;
		private Text _text;

		[SerializeField] private bool _localizeText = true;
		[SerializeField] private bool _interactWithManager = true;
		[SerializeField] private string _key;
		[SerializeField] private List<string> _formatArguments;

		/// <summary>
		/// Pass a reference to the Localization Manager from the child class through this property.
		/// </summary>
		protected abstract ILocalizationManager LocalizationManager { get; }

		/// <summary>
		/// The flag indicates whether text localization is enabled or disabled.
		/// </summary>
		public virtual bool LocalizeText => _localizeText;

		/// <summary>
		/// The flag indicates whether the tracking of current language changes in the Localization Manager
		///  enabled or disabled.
		/// </summary>
		public virtual bool InteractWithManager => _interactWithManager;

		protected virtual void Start()
		{
			if (!LocalizeText)
			{
				return;
			}

			if (LocalizationManager == null)
			{
				Debug.LogWarning("ILocalizationService isn't present in LocaleTextController.");
				return;
			}

			_text = GetComponent<Text>();
			if (string.IsNullOrEmpty(_key))
			{
				_key = _text.text.Trim();
			}

			_localString = new LocalString(LocalizationManager, _key, _formatArguments?.Cast<object>().ToArray());
			_text.text = _localString.Value;

			if (InteractWithManager)
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

			if (InteractWithManager)
			{
				_localString.ValueChangedEvent -= OnLocalStringValueChanged;
			}

			_localString.Dispose();
		}
	}
}