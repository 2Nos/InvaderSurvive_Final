using UnityEngine;

public class MoveState : PlayerMainState
{
    protected override LocomotionStateFlags LocomotionFlag => LocomotionStateFlags.Moving;
    public MoveState(PlayerLocomotion locomotion) : base(locomotion)
    {
        m_locomotion = locomotion;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Move 상태 진입");
    }
    public override void Update()
    {
        HandleMovement();
        HandleSubLocomotionFlags(); // 핵심
    }
    public override void Exit()
    {

    }

    private void HandleMovement()
    {
        m_locomotion.HandleMovement();
        m_locomotion.HandleRotation();
    }

    //복합 처리 패턴, Move상태에서 굳이 벗어날 필요없는 행동들에 대해서는 Move에서 계속 처리
    private void HandleSubLocomotionFlags()
    {
        // 입력 확인
        bool isSprinting = m_inputManager.IsSprinting;
        bool isCrouching = m_inputManager.IsCrouching;
        bool isAiming = m_inputManager.IsAiming;

       
    }
}
