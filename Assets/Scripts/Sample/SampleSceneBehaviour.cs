using System;
using Base.Localization;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Zenject;

namespace Sample
{
	public class SampleSceneBehaviour : MonoInstaller<SampleSceneBehaviour>
	{
		[Inject] private readonly ILocalizationManager _localizationManager;

		[SerializeField] private Toggle _ruToggle;
		[SerializeField] private Toggle _enToggle;

		public override void InstallBindings()
		{
		}

		public override void Start()
		{
			var canvas = FindObjectOfType<Canvas>();
			Assert.IsTrue(canvas);

			var window = Container.InstantiatePrefabResource("SimpleLocalization");
			window.transform.SetParent(canvas.transform, false);
			_localizationManager.Localize(window);

			window = Container.InstantiatePrefabResource("InteractiveLocalization");
			window.transform.SetParent(canvas.transform, false);
			_localizationManager.Localize(window, true);

			switch (_localizationManager.CurrentLanguage)
			{
				case SystemLanguage.English:
					_enToggle.isOn = true;
					break;
				case SystemLanguage.Russian:
					_ruToggle.isOn = true;
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public void OnSelectLanguage(bool check)
		{
			if (!check) return;

			if (_ruToggle.isOn)
			{
				_localizationManager.SetCurrentLanguage(SystemLanguage.Russian);
			}
			else if (_enToggle.isOn)
			{
				_localizationManager.SetCurrentLanguage(SystemLanguage.English);
			}
		}
	}
}