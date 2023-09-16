using Base.Localization;
using UnityEngine;
using Zenject;

namespace Sample
{
	public class GameInstaller : MonoInstaller<GameInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<ILocalizationManager>().To<LocalizationManager>().AsSingle()
				.WithArguments(Application.systemLanguage);
		}
	}
}