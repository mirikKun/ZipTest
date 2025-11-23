using Project.Scripts.Infrastructure.States.StateInfrastructure;
using Project.Scripts.Infrastructure.States.StateMachine;


namespace Project.Scripts.Infrastructure.States.GameStates
{
    public class InitializeProgressState : IState
    {
        private readonly IGameStateMachine _stateMachine;

        public InitializeProgressState(
            IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
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
        }

        public void Exit()
        {
        }
    }
}