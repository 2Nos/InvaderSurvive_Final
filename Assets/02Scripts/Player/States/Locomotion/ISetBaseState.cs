using UnityEngine;

public interface ISetBaseState
{
    // ���� ���� ����
    public LocomotionMainState DetermineStateType();

    // ���� �ִϸ��̼� Ÿ�� ����
    public AniParmType SetAniParmType();
}
