using Project.Scripts.Infrastructure.Loading;
using Project.Scripts.Infrastructure.States.StateInfrastructure;
using Project.Scripts.Infrastructure.States.StateMachine;

namespace Project.Scripts.Infrastructure.States.GameStates
{
    public class LoadingZipEndlessGameSceneState: IState
    {
        private const string ZipGameSceneName = "ZipEndlessGame";
        private readonly IGameStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;


        public LoadingZipEndlessGameSceneState(IGameStateMachine stateMachine, ISceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            _sceneLoader.LoadScene(ZipGameSceneName, EnterZipEndlessGameState);
        }

        private void EnterZipEndlessGameState()
        {
            _stateMachine.Enter<ZipEndlessGameState>();
        }

        public void Exit()
        {
        }
    }
}