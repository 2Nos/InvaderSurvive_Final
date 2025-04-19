using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateButtonGroupManager : MonoBehaviour
{
    private PlayerCore m_playerCore;

    [SerializeField]
    private StateButtonInfo[] m_stateBtnInfos;

    private Dictionary<StateButtonTpye, Button> m_flagBtnsMap;
    private Dictionary<StateButtonTpye, bool> m_toggleStatesMap; // ��� ���� ����
    private Dictionary<StateButtonTpye, Action<bool>> m_buttonInteractions; // Delegate�� ��ȣ�ۿ� ����

    private void Awake()
    {
        m_stateBtnInfos = GetComponentsInChildren<StateButtonInfo>();
        m_flagBtnsMap = new Dictionary<StateButtonTpye, Button>();
        m_toggleStatesMap = new Dictionary<StateButtonTpye, bool>();
        m_buttonInteractions = new Dictionary<StateButtonTpye, Action<bool>>();

        foreach (var flagInfo in m_stateBtnInfos)
        {
            var button = flagInfo.GetComponent<Button>();
            m_flagBtnsMap[flagInfo.m_StateButtonType] = button;

            // �ʱ� ��� ���� ���� (�⺻��: false)
            if (flagInfo.m_ButtonType == ButtonType.Toggle)
            {
                m_toggleStatesMap[flagInfo.m_StateButtonType] = false;
            }

            // ������ ���
            button.onClick.AddListener(() => OnButtonClicked(flagInfo));
        }

        m_playerCore = FindObjectOfType<PlayerCore>();

        // ��ư �� ��ȣ�ۿ� ���
        RegisterButtonInteractions();
    }

    private void OnDestroy()
    {
        foreach (var flagInfo in m_stateBtnInfos)
        {
            var button = m_flagBtnsMap[flagInfo.m_StateButtonType];
            button.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// Button Ŭ�� �� ȣ��Ǵ� �޼���
    /// </summary>
    private void OnButtonClicked(StateButtonInfo flagInfo)
    {
        if (flagInfo.m_ButtonType == ButtonType.Toggle)
        {
            // ��� ��ư ���� ó��
            bool newState = !m_toggleStatesMap[flagInfo.m_StateButtonType];
            m_toggleStatesMap[flagInfo.m_StateButtonType] = newState;

            UpdatePlayerState(flagInfo.m_StateButtonType, newState);
            UpdateButtonColor(flagInfo, newState);

            // Delegate�� ���� ��ȣ�ۿ� ����
            if (m_buttonInteractions.TryGetValue(flagInfo.m_StateButtonType, out var interaction))
            {
                interaction.Invoke(newState);
            }
        }
        else
        {
            // �Ϲ� ��ư ���� ó��
            HandleButtonAction(flagInfo.m_StateButtonType);
        }
    }

    /// <summary>
    /// �÷��̾� ���� ������Ʈ
    /// </summary>
    private void UpdatePlayerState(StateButtonTpye buttonType, bool isActive)
    {
        switch (buttonType)
        {
            case StateButtonTpye.Run:
                m_playerCore.m_InputManager.SetIsRunInput(isActive);
                break;
            case StateButtonTpye.Crouch:
                m_playerCore.m_InputManager.SetIsCrouchInput(isActive);
                break;
            case StateButtonTpye.Jump:
                if(m_playerCore.m_Locomotion.m_GetIsGrounded())
                {
                    m_playerCore.m_InputManager.SetIsInAirInput(isActive);
                    Debug.Log("Jump Input Set");
                }
                break;
            case StateButtonTpye.Dodge:
                m_playerCore.m_InputManager.SetIsDodgeInput(isActive);
                break;
            case StateButtonTpye.Attack:
                m_playerCore.m_InputManager.SetIsAttackInput(isActive);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ��ư �� ��ȣ�ۿ� ���
    /// </summary>
    private void RegisterButtonInteractions()
    {
        // Run ��ư�� Ȱ��ȭ�Ǹ� Crouch ��ư�� ��Ȱ��ȭ
        m_buttonInteractions[StateButtonTpye.Run] = isActive =>
        {
            /*if (isActive)
            {
                SetToggleState(StateButtonTpye.Crouch, false);
            }*/
        };

        // Crouch ��ư�� Ȱ��ȭ�Ǹ� Run ��ư�� ��Ȱ��ȭ
        m_buttonInteractions[StateButtonTpye.Crouch] = isActive =>
        {
            /*if (isActive)
            {
                SetToggleState(StateButtonTpye.Run, false);
            }*/
        };
    }

    /// <summary>
    /// Ư�� ��� ���¸� ������ ����
    /// </summary>
    private void SetToggleState(StateButtonTpye buttonType, bool isActive)
    {
        if (m_toggleStatesMap.ContainsKey(buttonType))
        {
            m_toggleStatesMap[buttonType] = isActive;

            if (m_flagBtnsMap.TryGetValue(buttonType, out var button))
            {
                UpdateButtonColor(button.GetComponent<StateButtonInfo>(), isActive);
            }

            UpdatePlayerState(buttonType, isActive);
        }
    }

    /// <summary>
    /// �Ϲ� ��ư ���� ó��
    /// </summary>
    private void HandleButtonAction(StateButtonTpye buttonType)
    {
        // �Ϲ� ��ư ���� ���� �߰�
        Debug.Log($"Button {buttonType} clicked!");
    }

    /// <summary>
    /// ��ư ���� ������Ʈ
    /// </summary>
    private void UpdateButtonColor(StateButtonInfo flagInfo, bool isActive)
    {
        if (m_flagBtnsMap.TryGetValue(flagInfo.m_StateButtonType, out var button))
        {
            button.image.color = isActive
                ? new Color(165f / 255f, 250f / 255f, 250f / 255f) // Ȱ��ȭ ����
                : Color.white; // ��Ȱ��ȭ ����
        }
    }
}