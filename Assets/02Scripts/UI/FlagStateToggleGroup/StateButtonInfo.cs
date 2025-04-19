using UnityEngine;
public enum StateButtonTpye
{
    None = 0,
    Run = 1 << 1,         // 달리기
    Crouch = 1 << 2,         // 앉기
    Jump = 1 << 3,          // 점프
    Dodge = 1 << 4,         // 회피
    Attack = 1 << 5,        // 공격
}

public enum ButtonType
{
    None = 0,
    Toggle = 1 << 1,         // 토글 버튼
}
public class StateButtonInfo : MonoBehaviour
{
    public ButtonType m_ButtonType;
    public StateButtonTpye m_StateButtonType;
}
