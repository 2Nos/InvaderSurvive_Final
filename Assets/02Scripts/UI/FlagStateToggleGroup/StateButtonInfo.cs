using UnityEngine;
public enum StateButtonTpye
{
    None = 0,
    Run = 1 << 1,         // �޸���
    Crouch = 1 << 2,         // �ɱ�
    Jump = 1 << 3,          // ����
    Dodge = 1 << 4,         // ȸ��
    Attack = 1 << 5,        // ����
}

public enum ButtonType
{
    None = 0,
    Toggle = 1 << 1,         // ��� ��ư
}
public class StateButtonInfo : MonoBehaviour
{
    public ButtonType m_ButtonType;
    public StateButtonTpye m_StateButtonType;
}
