using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] PlayerCore m_PlayerCore;

    [Header("타겟 설정")]
    [SerializeField] Transform m_aimPos;       // 캐릭터 Transform
    [SerializeField] Transform m_target;
    [SerializeField] LayerMask m_aimMask;

    [Header("카메라 위치 설정")]
    private Vector3 m_cameraOffset = new Vector3(0.4f, 0.4f, -1.5f); // 오른쪽 어깨 뒤
    private Vector3 m_lookOffset = new Vector3(0.4f, 0.4f, 0f); // 카메라 시선 대상 (가슴~머리 위)

    [Header("카메라 회전 설정")]
    public float m_rotationSpeed = 0.1f;             // 카메라 회전 부드러움 정도
    float m_minVerticalAngle = -30f;              // 수직 회전 최소 각도
    float m_maxVerticalAngle = 45f;               // 수직 회전 최대 각도

    private float m_pitch = 0f;// 현재 X축 회전값
    public float m_yaw { get; private set; }// 현재 Y축 회전값

    

    private void LateUpdate()
    {
        if (m_target == null) return;

        HandleRotation();
        HandlePosition();
        HandleAim();
    }
    private void HandlePosition()
    {
        Quaternion rotation = Quaternion.Euler(m_pitch, m_yaw, 0f);

        // 회전된 오프셋 방향으로 카메라 위치 설정
        Vector3 targetPosition = m_target.position + rotation * m_cameraOffset;
        transform.position = targetPosition;

        // 카메라는 타겟이 바라보는 위치를 LookAt
        Vector3 lookTarget = m_target.position + m_target.rotation * m_lookOffset;
        transform.LookAt(lookTarget);
    }

    private void HandleRotation()
    {
        float mouseX = m_PlayerCore.m_InputManager.LookInput.x;
        float mouseY = m_PlayerCore.m_InputManager.LookInput.y;

        m_yaw += mouseX * m_rotationSpeed;
        m_pitch -= mouseY * m_rotationSpeed;
        m_pitch = Mathf.Clamp(m_pitch, m_minVerticalAngle, m_maxVerticalAngle);
    }
    private void HandleAim()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_aimMask))
        {
            m_aimPos.position = Vector3.Lerp(m_aimPos.position, hit.point, Time.deltaTime * 20f);
        }
        else
        {
            // 카메라 정면으로 10미터
            m_aimPos.position = transform.position + transform.forward * 10f;
        }
    }
}