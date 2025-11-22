using Project.Scripts.Infrastructure.States.StateInfrastructure;

namespace Project.Scripts.Infrastructure.States.StateMachine
{
    public interface IGameStateMachine
    {
        void Enter<TState>() where TState : class, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>;
    }
}