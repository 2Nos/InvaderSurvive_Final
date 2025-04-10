public class PlayerLocomotion
{
    private PlayerCore m_core;
    private LocomotionStateBase m_currentState;

    public PlayerLocomotion(PlayerCore core)
    {
        this.m_core = core;
        ChangeState(new IdleState(this));
    }

    public void Update()
    {
        m_currentState?.Update();
    }

    public void ChangeState(LocomotionStateBase newState)
    {
        m_currentState?.Exit();
        m_currentState = newState;
        m_currentState?.Enter(m_core);
    }

   // public MainStateAndSubFlagsManager Flags => core.StateAndFlagManager;
    //public PlayerCombat Combat => core.Combat;
}