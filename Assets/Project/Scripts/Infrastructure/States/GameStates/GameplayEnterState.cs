using Code.Gameplay.Levels;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;

namespace Code.Infrastructure.States.GameStates
{
    public class GameplayEnterState : IState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly ILevelDataProvider _levelDataProvider;


        public GameplayEnterState(
            IGameStateMachine stateMachine,
            ILevelDataProvider levelDataProvider)
        {
            _stateMachine = stateMachine;
            _levelDataProvider = levelDataProvider;
        }

        public void Enter()
        {
            PlacePlayer();

            _stateMachine.Enter<GameloopLoopState>();
        }

        private void PlacePlayer()
        {
        }

        public void Exit()
        {
        }
    }
}