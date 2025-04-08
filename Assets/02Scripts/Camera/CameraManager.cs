using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] PlayerInputManager m_inputManager;

    [Header("타겟 설정")]
    [SerializeField] Transform m_aimPos;       // 캐릭터 Transform
    [SerializeField] Transform m_target;
    [SerializeField] LayerMask m_aimMask;

    [Header("카메라 위치 설정")]
    [SerializeField] Vector3 m_offset = new Vector3(0.4f, 0.4f, -1.5f);  // 카메라 위치 오프셋
    [SerializeField] float m_smoothSpeed = 10f;

    [Header("카메라 회전 설정")]
    public float m_rotationSmoothSpeed;             // 카메라 회전 부드러움 정도
    [SerializeField] float m_minVerticalAngle = -15f;              // 수직 회전 최소 각도
    [SerializeField] float m_maxVerticalAngle = 60f;               // 수직 회전 최대 각도

    private float m_currentRotationX = 0f;                                 // 현재 X축 회전값
    public float m_currentRotationY { get; private set; }                                 // 현재 Y축 회전값
    private Quaternion rotation;
    private void LateUpdate()
    {
        if (m_target == null) return;
        Aim();
        CameraRotate();
    }

    private void CameraRotate()
    {
        // 마우스 입력 처리 (축 기준이기에)
        float mouseX = m_inputManager.LookInput.y;
        float mouseY = m_inputManager.LookInput.x;

        // 회전 업데이트
        m_currentRotationX -= mouseX * m_rotationSmoothSpeed;
        m_currentRotationY += mouseY * m_rotationSmoothSpeed;
        m_currentRotationX = Mathf.Clamp(m_currentRotationX, m_minVerticalAngle, m_maxVerticalAngle);

        // 회전 계산
        Quaternion rotation = Quaternion.Euler(m_currentRotationX, m_currentRotationY, 0);

        // 🧠 어깨 너머 위치 계산
        Vector3 rotatedOffset = m_target.rotation * m_offset;
        Vector3 desiredPosition = m_target.position + rotatedOffset;

        // 카메라 위치 갱신
        transform.position = desiredPosition;

        Vector3 m_lookAtOffset = m_offset; // 카메라가 바라볼 지점 (플레이어 앞)
        m_lookAtOffset.z = m_offset.z * -1; //바라보는 방향이 카메라가 아닌 반대로 되어야 함

        // 카메라는 타겟의 시선 중심을 바라본다
        Vector3 lookTarget = m_target.position + (rotation * m_lookAtOffset);
        transform.LookAt(lookTarget);
    }

    private void Aim()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_aimMask))
        {
            m_aimPos.position = Vector3.Lerp(m_aimPos.position, hit.point, m_smoothSpeed * Time.deltaTime);
        }
        else
        {
            m_aimPos.position = m_target.position + (Camera.main.transform.forward * (m_offset.z * -1));
            //m_aimPos.position = Vector3.Lerp(m_aimPos.position, Camera.main.transform.forward +m_aimOffset * Time.deltaTime,1);
        }

    }
}