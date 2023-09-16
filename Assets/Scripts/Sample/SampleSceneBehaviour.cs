using Base.Localization;
using UnityEngine;
using Zenject;

namespace Sample
{
	public class SampleSceneBehaviour : MonoInstaller<SampleSceneBehaviour>
	{
#pragma warning disable 649
		[Inject] private readonly ILocalizationManager _localizationManager;
#pragma warning restore 649

		public override void InstallBindings()
		{
		}

		public override void Start()
		{
			Debug.Log(_localizationManager.GetLocalized("key.1"));
			Debug.Log(_localizationManager.GetLocalized("key.2"));
			Debug.Log(_localizationManager.GetLocalized("key.3"));
			Debug.Log(_localizationManager.GetLocalized("key.4"));
		}
	}
}