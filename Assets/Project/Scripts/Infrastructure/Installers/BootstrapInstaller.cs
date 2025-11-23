using Project.Scripts.Gameplay.Common.Random;
using Project.Scripts.Gameplay.Common.Time;
using Project.Scripts.Gameplay.StaticData;
using Project.Scripts.Gameplay.Windows;
using Project.Scripts.Infrastructure.AssetManagement;
using Project.Scripts.Infrastructure.Loading;
using Project.Scripts.Infrastructure.Progress;
using Project.Scripts.Infrastructure.States.Factory;
using Project.Scripts.Infrastructure.States.GameStates;
using Project.Scripts.Infrastructure.States.StateMachine;
using Project.Scripts.Progress.Provider;
using Zenject;

namespace Project.Scripts.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller, ICoroutineRunner, IInitializable
    {
        public override void InstallBindings()
        {
            BindGameStateMachine();
            BindInfrastructureServices();
            BindAssetManagementServices();
            BindCommonServices();
            BindGameplayServices();
        }
        private void BindGameStateMachine()
        {
            Container.BindInterfacesAndSelfTo<StateFactory>().AsSingle();
            Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
            
            //States
            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitializeProgressState>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<LoadingHomeScreenState>().AsSingle();
            Container.BindInterfacesAndSelfTo<HomeScreenState>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<LoadingZipGameSceneState>().AsSingle();
            Container.BindInterfacesAndSelfTo<ZipGameState>().AsSingle();       
            
            Container.BindInterfacesAndSelfTo<LoadingZipEndlessGameSceneState>().AsSingle();
            Container.BindInterfacesAndSelfTo<ZipEndlessGameState>().AsSingle();
        }



        private void BindGameplayServices()
        {
            Container.Bind<IGameProgressService>().To<GameProgressService>().AsSingle();
            Container.Bind<IProgressProvider>().To<ProgressProvider>().AsSingle();
            Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
        }

        private void BindInfrastructureServices()
        {
            Container.BindInterfacesTo<BootstrapInstaller>().FromInstance(this).AsSingle();
        }

        private void BindAssetManagementServices()
        {
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
            Container.Bind<IWindowFactory>().To<WindowFactory>().AsSingle();
            Container.Bind<IWindowService>().To<WindowService>().AsSingle();
        }

        private void BindCommonServices()
        {
            Container.Bind<IRandomService>().To<UnityRandomService>().AsSingle();
            Container.Bind<ITimeService>().To<UnityTimeService>().AsSingle();
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
        }



        public void Initialize()
        {
            Container.Resolve<IGameStateMachine>().Enter<BootstrapState>();
        }
    }
}