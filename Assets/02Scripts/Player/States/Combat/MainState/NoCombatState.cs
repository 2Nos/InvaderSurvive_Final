using UnityEngine;

public class NoCombatState : ICombatState
{
    private readonly PlayerCombat combat;

    public NoCombatState(PlayerCombat combat)
    {
        this.combat = combat;
    }

    public void Enter()
    {
        //combat.Flags.CombatMain = CombatMainState.Shooting;
        combat.Flags.SetCombatFlag(CombatSubFlags.Aming);

        // 달리기 중이라면 멈추고 걷기로
        if (combat.Flags.HasLocomotionFlag(LocomotionSubFlags.None))
        {
            combat.Flags.ClearLocomotionFlag(LocomotionSubFlags.None);
           // combat.Locomotion.ChangeState(new MoveState(combat.Locomotion));
        }
    }

    public void Update()
    {

    }

    public void Exit()
    {
        combat.Flags.ClearCombatFlag(CombatSubFlags.None);
    }
}