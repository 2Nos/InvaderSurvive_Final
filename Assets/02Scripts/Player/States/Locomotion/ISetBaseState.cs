using UnityEngine;
using DUS.Player.Locomotion;

public interface ISetBaseState
{
    // ���� ���� ����
    public LocomotionMainState DetermineStateType();

    // ���� �ִϸ��̼� Ÿ�� ����
    public AniParmType SetAniParmType();
}
