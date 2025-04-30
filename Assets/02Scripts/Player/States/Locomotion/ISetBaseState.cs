using UnityEngine;
using DUS.Player.Locomotion;

public interface ISetBaseState
{
    // 본인 상태 설정
    public LocomotionMainState DetermineStateType();

    // 본인 애니메이션 타입 설정
    public AniParmType SetAniParmType();
}
