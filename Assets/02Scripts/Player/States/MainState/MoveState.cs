using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerLocomotion locomotion) : base(locomotion) { }
    protected override string AnimationBoolName => AnimKeys.Move;
    public override void Enter()
    {
        base.Enter();
        m_flagManager.LocomotionMain = LocomotionMainState.Moving;
    }
    public override void Update()
    {
        if (IsActionBlocked()) return;

        //�̵� ȸ�� ó��
        HandleMovement();

        // �� ���� LocomotionFlag ó�� (�޸���, �ɱ� ��)
        HandleLocomotionSubFlags();

        // �� ���� �켱���� ó�� (����, �����̵�, �� �޸��� ��)
        foreach (var transition in DefaultTransitions)
        {
            if (TryCommonTransition(transition)) return;
        }

        // �� �̵� �Է��� �� �̻� ������ Idle ���·�
        if (!m_inputManager.IsMoving)
        {
            m_locomotion.ChangeState(new IdleState(m_locomotion));
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}
