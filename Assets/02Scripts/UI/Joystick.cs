using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Windows;

public enum JoystickType
{
    Move,
    Rotate
}

namespace DUS.Joystick
{
    //조이스틱 UI를 위한 클래스
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public PlayerInputManager m_PlayerInputManager;

        [Header("Joystick 타입")]
        [SerializeField] JoystickType m_joystickType;

        [Header("Joystick Image")]
        public Image m_joystickBackgroundImage;
        public Image m_joystickLeverImange;
        private RectTransform m_joystickBackGroundRectTr; //조이스틱 레버가 움직일 배경 Rect영역
        private RectTransform m_joystickLeverRectTr;
        private Vector2 m_joystickCenterPos;

        [Range(80, 100)] //조이스틱 움직일 영역 제한
        [Header("Lever 영역 제한")]
        [SerializeField] float m_leverRange;

        private Vector2 inputDirection;

        [HideInInspector]
        public bool isInput;
        private void Awake()
        {
            m_joystickBackGroundRectTr = m_joystickBackgroundImage.GetComponent<RectTransform>();
            m_joystickLeverRectTr = m_joystickLeverImange.GetComponent<RectTransform>();

            //UI의 사이즈가 리사이징되는 순간 다시 계산해야함 그래서 OnPointerDown에서 계산함
            m_joystickCenterPos  = m_joystickBackGroundRectTr.position;

            //m_leverRange = m_joystickBackGroundRectTr.sizeDelta.x / 2; //조이스틱 움직일 영역 제한
        }
        //백그라운드 이미지 클릭에 대한 이벤트 처리
        public void OnPointerDown(PointerEventData eventData) //eventData 스크린 좌표계
        {
            if (m_PlayerInputManager.m_InputType == InputType.Keyboard) return;
            m_joystickCenterPos = m_joystickBackGroundRectTr.position;

            isInput = true;
            CalculateMoveJoystickUI(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_PlayerInputManager.m_InputType == InputType.Keyboard) return;
            CalculateMoveJoystickUI(eventData);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_PlayerInputManager.m_InputType == InputType.Keyboard) return;
            InitializeJoyStick();
        }

        private void CalculateMoveJoystickUI(PointerEventData eventData)
        {
            if (eventData == null) return;
            // 드래그 위치 (스크린 좌표) - 조이스틱 배경 위치 (월드이지만 Canvas모드가 Overay이기에 스크린 좌표와 동일)
            Vector2 inputPos = eventData.position - (Vector2)m_joystickCenterPos; //해상도 상관없이 포지션으로 해결

            // ClampMagnitude 최대 이동 거리 제한해주는 함수, leverRange까지만 이동가능
            Vector2 clampedPos = Vector2.ClampMagnitude(inputPos, m_leverRange);

            // 레버 이동 (localPosition 기준), m_joystickLeverRectTr.localPosition의 첫 시작은 어차피 Vector3.zero이기에
            m_joystickLeverRectTr.localPosition = clampedPos;

            // 정규화된 방향 저장
            inputDirection = clampedPos.normalized;

            switch (m_joystickType)
            {
                case JoystickType.Move:
                    m_PlayerInputManager.SetInput(inputDirection, JoystickType.Move);
                    break;
                case JoystickType.Rotate:
                    m_PlayerInputManager.SetInput(inputDirection, JoystickType.Rotate);
                    break;
            }
            m_joystickLeverImange.color = new Color32(141, 250, 255, 255);
        }

        //손 놓았을때 원래 자리로(그냥 포지션으로 계산 하려면 캔버스 크기(스케일)적용해서 계산해야할듯)
        private void InitializeJoyStick()
        {
            m_joystickLeverRectTr.localPosition = Vector3.zero;
            m_joystickLeverImange.color = Color.white;

            inputDirection = Vector2.zero;
            switch (m_joystickType)
            {
                case JoystickType.Move:
                    m_PlayerInputManager.SetInput(inputDirection, JoystickType.Move);
                    break;
                case JoystickType.Rotate:
                    m_PlayerInputManager.SetInput(inputDirection, JoystickType.Rotate);
                    break;
            }
        }

        private void CalculateMoveJoystickUI_Keyboard()
        {
            m_joystickLeverImange.color = new Color32(141, 250, 255, 255);

            float joystickSpeed = 5;
            // 드래그 위치 (스크린 좌표) - 조이스틱 배경 위치 (월드이지만 Canvas모드가 Overay이기에 스크린 좌표와 동일)
            Vector3 inputPos = m_PlayerInputManager.m_MovementInput; //해상도 상관없이 포지션으로 해결

            if (inputPos == Vector3.zero)
            {
                InitializeJoyStick();
                return;
            }
            float dis = Vector3.Distance(Vector3.zero, m_joystickLeverRectTr.transform.localPosition);

            if (dis < m_leverRange)
            {
                m_joystickLeverRectTr.localPosition = m_joystickLeverRectTr.localPosition + (inputPos * joystickSpeed);
            }
            else if(dis >= m_leverRange)
            {
                Vector2 current = m_joystickLeverRectTr.localPosition;
                Vector2 target = inputPos.normalized * m_leverRange;
                m_joystickLeverRectTr.localPosition = Vector2.Lerp(current, target, Time.deltaTime * joystickSpeed);
            }
        }
        private void CalculateRotJoystickUI_Keyboard()
        {
            m_joystickLeverImange.color = new Color32(141, 250, 255, 255);

            float joystickSpeed = 10f;
            // 드래그 위치 (스크린 좌표) - 조이스틱 배경 위치 (월드이지만 Canvas모드가 Overay이기에 스크린 좌표와 동일)
            Vector3 inputPos = m_PlayerInputManager.m_LookInput; //해상도 상관없이 포지션으로 해결
            if (inputPos == Vector3.zero)
            {
                InitializeJoyStick();
                return;
            }
            float dis = Vector3.Distance(Vector3.zero, m_joystickLeverRectTr.transform.localPosition);

            if (dis < m_leverRange)
            {
                m_joystickLeverRectTr.localPosition = m_joystickLeverRectTr.localPosition + (inputPos/10);
            }
            else if (dis >= m_leverRange)
            {
                Vector2 current = m_joystickLeverRectTr.localPosition;
                Vector2 target = inputPos.normalized * m_leverRange;
                m_joystickLeverRectTr.localPosition = Vector2.Lerp(current, target, Time.deltaTime * joystickSpeed);
            }
        }
        private void Update()
        {
            if (m_PlayerInputManager.m_InputType == InputType.Keyboard)
            {
                if (m_joystickType == JoystickType.Move)
                    CalculateMoveJoystickUI_Keyboard();
                else if (m_joystickType == JoystickType.Rotate)
                    CalculateRotJoystickUI_Keyboard();
            }
        }
    }
}