using Project.Scripts.Infrastructure.States.StateInfrastructure;

namespace Project.Scripts.Infrastructure.States.Factory
{
    public interface IStateFactory
    {
        T GetState<T>() where T : class, IExitableState;
    }
}