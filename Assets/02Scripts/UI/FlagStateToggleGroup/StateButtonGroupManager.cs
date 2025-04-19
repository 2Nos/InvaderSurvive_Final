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
    private Dictionary<StateButtonTpye, bool> m_toggleStatesMap; // 토글 상태 관리
    private Dictionary<StateButtonTpye, Action<bool>> m_buttonInteractions; // Delegate로 상호작용 관리

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

            // 초기 토글 상태 설정 (기본값: false)
            if (flagInfo.m_ButtonType == ButtonType.Toggle)
            {
                m_toggleStatesMap[flagInfo.m_StateButtonType] = false;
            }

            // 리스너 등록
            button.onClick.AddListener(() => OnButtonClicked(flagInfo));
        }

        m_playerCore = FindObjectOfType<PlayerCore>();

        // 버튼 간 상호작용 등록
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
    /// Button 클릭 시 호출되는 메서드
    /// </summary>
    private void OnButtonClicked(StateButtonInfo flagInfo)
    {
        if (flagInfo.m_ButtonType == ButtonType.Toggle)
        {
            // 토글 버튼 동작 처리
            bool newState = !m_toggleStatesMap[flagInfo.m_StateButtonType];
            m_toggleStatesMap[flagInfo.m_StateButtonType] = newState;

            UpdatePlayerState(flagInfo.m_StateButtonType, newState);
            UpdateButtonColor(flagInfo, newState);

            // Delegate를 통해 상호작용 실행
            if (m_buttonInteractions.TryGetValue(flagInfo.m_StateButtonType, out var interaction))
            {
                interaction.Invoke(newState);
            }
        }
        else
        {
            // 일반 버튼 동작 처리
            HandleButtonAction(flagInfo.m_StateButtonType);
        }
    }

    /// <summary>
    /// 플레이어 상태 업데이트
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
    /// 버튼 간 상호작용 등록
    /// </summary>
    private void RegisterButtonInteractions()
    {
        // Run 버튼이 활성화되면 Crouch 버튼을 비활성화
        m_buttonInteractions[StateButtonTpye.Run] = isActive =>
        {
            /*if (isActive)
            {
                SetToggleState(StateButtonTpye.Crouch, false);
            }*/
        };

        // Crouch 버튼이 활성화되면 Run 버튼을 비활성화
        m_buttonInteractions[StateButtonTpye.Crouch] = isActive =>
        {
            /*if (isActive)
            {
                SetToggleState(StateButtonTpye.Run, false);
            }*/
        };
    }

    /// <summary>
    /// 특정 토글 상태를 강제로 설정
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
    /// 일반 버튼 동작 처리
    /// </summary>
    private void HandleButtonAction(StateButtonTpye buttonType)
    {
        // 일반 버튼 동작 로직 추가
        Debug.Log($"Button {buttonType} clicked!");
    }

    /// <summary>
    /// 버튼 색상 업데이트
    /// </summary>
    private void UpdateButtonColor(StateButtonInfo flagInfo, bool isActive)
    {
        if (m_flagBtnsMap.TryGetValue(flagInfo.m_StateButtonType, out var button))
        {
            button.image.color = isActive
                ? new Color(165f / 255f, 250f / 255f, 250f / 255f) // 활성화 색상
                : Color.white; // 비활성화 색상
        }
    }
}