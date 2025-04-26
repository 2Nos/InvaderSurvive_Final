public class PlayerCombat
{
    private readonly PlayerCore m_core;
    private CombatBaseState m_currentState;

    public PlayerCombat(PlayerCore core)
    {
        this.m_core = core;
        //SwithcCurrentState(new NoCombatState(this));
    }

    public void Update()
    {
        m_currentState?.Update();
    }

    public void ChangeState(CombatBaseState newState)
    {
        m_currentState?.Exit();
        m_currentState = newState;
        m_currentState?.Enter();
    }

}