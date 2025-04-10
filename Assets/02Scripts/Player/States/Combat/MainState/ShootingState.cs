using UnityEngine;

//Shooting -> Aiming�� ���� Aiming���� �߻�
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

        // �޸��� ���̶�� ���߰� �ȱ��
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