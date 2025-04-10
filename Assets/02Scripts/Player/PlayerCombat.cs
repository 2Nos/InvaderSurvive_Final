public class PlayerCombat
{
    private ICombatState currentState;
    private readonly PlayerCore core;

    public PlayerCombat(PlayerCore core)
    {
        this.core = core;
        ChangeState(new NoCombatState(this));
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(ICombatState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public MainStateAndSubFlagsManager Flags => core.GetFlagManager();
    //public PlayerLocomotion Locomotion => core.GetLocm;
}