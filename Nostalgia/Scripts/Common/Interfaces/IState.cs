namespace Nostal.Interfaces.State
{
    public interface IState
    {
        void OnStateEnter();
        void OnStateUpdate(float deltaTime);
        void OnStateExit();
    }
}
