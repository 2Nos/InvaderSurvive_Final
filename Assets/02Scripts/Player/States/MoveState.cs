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
        Debug.Log("Move ���� ����");
    }
    public override void Update()
    {
        HandleMovement();
        HandleSubLocomotionFlags(); // �ٽ�
    }
    public override void Exit()
    {

    }

    private void HandleMovement()
    {
        m_locomotion.HandleMovement();
        m_locomotion.HandleRotation();
    }

    //���� ó�� ����, Move���¿��� ���� ��� �ʿ���� �ൿ�鿡 ���ؼ��� Move���� ��� ó��
    private void HandleSubLocomotionFlags()
    {
        // �Է� Ȯ��
        bool isSprinting = m_inputManager.IsSprinting;
        bool isCrouching = m_inputManager.IsCrouching;
        bool isAiming = m_inputManager.IsAiming;

       
    }
}
