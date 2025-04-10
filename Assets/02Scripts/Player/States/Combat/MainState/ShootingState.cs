using UnityEngine;

//Shooting -> Aiming로 전이 Aiming에서 발사
public class ShootingState : ICombatState
{
    private readonly PlayerCombat combat;

    public ShootingState(PlayerCombat combat)
    {
        this.combat = combat;
    }

    public void Enter()
    {
        //combat.Flags.CombatMain = CombatMainState.Shooting;
        combat.Flags.SetCombatFlag(CombatSubFlags.Aming);

        // 달리기 중이라면 멈추고 걷기로
        if (combat.Flags.HasLocomotionFlag(LocomotionSubFlags.Sprinting))
        {
            combat.Flags.ClearLocomotionFlag(LocomotionSubFlags.Sprinting);
           // combat.Locomotion.ChangeState(new MoveState(combat.Locomotion));
        }
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            combat.ChangeState(new NoCombatState(combat));
        }
    }

    public void Exit()
    {
        combat.Flags.ClearCombatFlag(CombatSubFlags.Aming);
    }
}