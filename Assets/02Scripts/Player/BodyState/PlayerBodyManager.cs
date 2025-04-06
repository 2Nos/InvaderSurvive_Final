using UnityEngine;

public class PlayerBodyManager : MonoBehaviour
{
    [Header("Body Settings")]
    [SerializeField] private Transform m_upperBodyTransform;
    [SerializeField] private Transform m_weaponHolder;
    [SerializeField] private float m_upperBodyRotationSpeed = 10f;
    [SerializeField] private float m_lowerBodyRotationSpeed = 10f;

    [Header("Aiming Settings")]
    [SerializeField] private Vector3 m_normalWeaponPosition = new Vector3(0.5f, 1.2f, 0.5f);
    [SerializeField] private Vector3 m_aimingWeaponPosition = new Vector3(0.5f, 1.5f, 0.5f);
    [SerializeField] private float m_weaponPositionLerpSpeed = 10f;

    private Animator m_animator;
    private bool m_isAiming;
    private Vector3 m_targetWeaponPosition;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_targetWeaponPosition = m_normalWeaponPosition;
    }

    private void Update()
    {
        // 무기 홀더 위치 부드럽게 전환
        if (m_weaponHolder != null)
        {
            m_weaponHolder.localPosition = Vector3.Lerp(
                m_weaponHolder.localPosition, 
                m_targetWeaponPosition, 
                Time.deltaTime * m_weaponPositionLerpSpeed
            );
        }
    }


    public void UpdateUpperBodyRotation(Vector2 lookInput)
    {
        if (m_animator == null) return;

        // 상체 회전 처리
        float yaw = lookInput.x * m_upperBodyRotationSpeed;
        float pitch = lookInput.y * m_upperBodyRotationSpeed;
        
        // 애니메이터 파라미터 업데이트
        m_animator.SetFloat("UpperBodyYaw", yaw);
        m_animator.SetFloat("UpperBodyPitch", pitch);
    }


    public void UpdateLowerBodyRotation(Vector2 moveInput)
    {
        if (m_animator == null) return;

        // 하체 회전 처리
        float yaw = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
        m_animator.SetFloat("LowerBodyYaw", yaw);
    }

    public void SetAiming(bool isAiming)
    {
        m_isAiming = isAiming;
        
        if (m_animator != null)
        {
            m_animator.SetBool("IsAiming", isAiming);
        }

        // 조준 상태에 따라 무기 홀더 위치 설정
        m_targetWeaponPosition = isAiming ? m_aimingWeaponPosition : m_normalWeaponPosition;
    }

    public bool IsAiming() => m_isAiming;

    // 무기 홀더 Transform 가져오기
    public Transform GetWeaponHolder() => m_weaponHolder;

    // 상체 Transform 가져오기
    public Transform GetUpperBodyTransform() => m_upperBodyTransform;

}
