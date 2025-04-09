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

        //이동 회전 처리
        HandleMovement();

        // ↓ 복수 LocomotionFlag 처리 (달리기, 앉기 등)
        HandleLocomotionSubFlags();

        // ↓ 전이 우선순위 처리 (공중, 슬라이딩, 벽 달리기 등)
        foreach (var transition in DefaultTransitions)
        {
            if (TryCommonTransition(transition)) return;
        }

        // ↓ 이동 입력이 더 이상 없으면 Idle 상태로
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
