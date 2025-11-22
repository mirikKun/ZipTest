using Project.Scripts.Infrastructure.States.StateInfrastructure;
using Project.Scripts.Infrastructure.States.StateMachine;
using Project.Scripts.Progress.Data;
using Project.Scripts.Progress.Provider;

namespace Project.Scripts.Infrastructure.States.GameStates
{
    public class InitializeProgressState : IState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IProgressProvider _progressProvider;

        public InitializeProgressState(
            IGameStateMachine stateMachine,
            IProgressProvider progressProvider)
        {
            _stateMachine = stateMachine;
            _progressProvider = progressProvider;
        }

        public void Enter()
        {
            InitializeProgress();

            _stateMachine.Enter<LoadingHomeScreenState>();
        }

        private void InitializeProgress()
        {
            CreateNewProgress();
        }

        private void CreateNewProgress()
        {
            _progressProvider.SetProgressData(new ProgressData());
        }

        public void Exit()
        {
        }
    }
}