using Base.LocaleService;
using Zenject;

namespace Sample
{
	public class GameInstaller : MonoInstaller<GameInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<ILocaleService>().To<LocaleService>().AsSingle();
		}
	}
}