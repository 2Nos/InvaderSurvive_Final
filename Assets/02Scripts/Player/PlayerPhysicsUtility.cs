// ======================================== 250430
// 물리 연산 관리 클래스
// ========================================
using UnityEngine;

namespace DUS.Player {
    public static class PlayerPhysicsUtility
    {
        public static void SetVelocity(Rigidbody rigid, Vector3 velocity)
        {
            rigid.linearVelocity = velocity;
        }

        public static void SetVelocityY(Rigidbody rigid, float velocityY)
        {
            Vector3 v = rigid.linearVelocity;
            v.y = velocityY;
            rigid.linearVelocity = v;
        }

        public static Vector3 ClampVelocityY(Rigidbody rigid, float minY, float maxY)
        {
            Vector3 v = rigid.linearVelocity;
            v.y = Mathf.Clamp(v.y, minY, maxY);
            rigid.linearVelocity = v;
            return v;
        }

        // 질량
        public static void ApplyGravity(Rigidbody rigid, Vector3 gravity)
        {
            // 질량까지 적용되는 자연스러운 힘
            rigid.AddForce(gravity * rigid.mass, ForceMode.Force);

            /*var velocity = rigid.linearVelocity;
            var acceleration = Physics.gravity;
            velocity += acceleration * Time.fixedDeltaTime;
            rigid.linearVelocity = velocity;*/
        }
    }
}
