using Base.GameService;
using Base.LocaleService;
using UnityEngine;
using Zenject;

namespace Sample
{
	public class SampleSceneBehaviour : MonoInstaller<SampleSceneBehaviour>
	{
#pragma warning disable 649
		[Inject] private readonly ILocaleService _localeService;
#pragma warning restore 649

		public override void InstallBindings()
		{
		}

		public override void Start()
		{
			_localeService.ReadyEvent += LocaleServiceOnReadyEvent;
			_localeService.Initialize();
		}

		private void LocaleServiceOnReadyEvent(IGameService service)
		{
			service.ReadyEvent -= LocaleServiceOnReadyEvent;
			Debug.Log(_localeService.GetLocalized("key.1"));
			Debug.Log(_localeService.GetLocalized("key.2"));
			Debug.Log(_localeService.GetLocalized("key.3"));
			Debug.Log(_localeService.GetLocalized("key.4"));
		}
	}
}