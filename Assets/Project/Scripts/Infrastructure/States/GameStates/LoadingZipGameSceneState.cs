using Project.Scripts.Infrastructure.Loading;
using Project.Scripts.Infrastructure.States.StateInfrastructure;
using Project.Scripts.Infrastructure.States.StateMachine;

namespace Project.Scripts.Infrastructure.States.GameStates
{
    public class LoadingZipGameSceneState : IState
    {
        private const string ZipGameSceneName = "ZipGame";
        private readonly IGameStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;


        public LoadingZipGameSceneState(IGameStateMachine stateMachine, ISceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            _sceneLoader.LoadScene(ZipGameSceneName, EnterZipGameState);
        }

        private void EnterZipGameState()
        {
            _stateMachine.Enter<ZipGameState>();
        }

        public void Exit()
        {
        }


    }
}