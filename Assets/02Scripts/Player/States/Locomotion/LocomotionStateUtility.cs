// MainState : ���� ���� ����
// SubFlags : ���� ���� ����
// Main + Sub

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUS.Player.Locomotion
{
    public enum LocomotionMainState
    {
        Idle = 0,           // Idle
        Move = 1,           // �⺻ �̵�
        Jump = 2,          // ����
        InAir = 3,          // ���� (����/����)
        Land = 4,           // ����
        Dodge = 5,          // ������(ȸ�Ǳ�)
        Slide = 6,          // �����̵�
        Climb = 7,          // ���
        WallRun = 8,         // �� �޸���
        Staggered = 9,         // �ǰ� ���� ���� ����
        Knockback = 10,         // �˹�
    }

    /// <summary>
    /// Flags�� ������ ������ ������ ������ ���. ��, ���� ���°� ����
    /// Flags�� 2�� �������� 2�� ������ ������ ����Ͽ� �����ؾ���
    /// None =0, FalgButtonGroupManager = 1, Croucning = 2, 4, 8 �̷��� ���ٴ� ����Ʈ����)
    /// </summary>
    [Flags]
    public enum LocomotionSubFlags
    {
        None = 0,
        Run = 1 << 1,           // �޸���
        Crouch = 1 << 2,        // �ɱ�
        CrouchRun = 1 << 3      // �ɾƼ� �޸��� 
    }

    public class LocomotionStateUtility
    {
        #region ======================================== MainState ����

        // ========== Map ==========
        public Dictionary<LocomotionMainState, LocomotionStrategyState> m_MainStrategyMap = new();
        private readonly Dictionary<LocomotionMainState, string> m_MainStateAniMap = new()
        {
            { LocomotionMainState.Idle, "IsIdle" },
            { LocomotionMainState.Move, "IsMove" },
            { LocomotionMainState.Jump, "IsJump" },
            { LocomotionMainState.InAir, "IsInAir" },
            { LocomotionMainState.Land, "IsLand" },
            { LocomotionMainState.Slide, "IsSlide" },
            { LocomotionMainState.Climb, "IsClimb" },
            { LocomotionMainState.WallRun, "IsWallRun" }
        };
        private readonly Dictionary<LocomotionMainState, string> m_MainTriggerAniMap = new()
        {
            { LocomotionMainState.Idle, "Idle" },
            { LocomotionMainState.Move, "Move" },
            { LocomotionMainState.Jump, "Jump" },
            { LocomotionMainState.InAir, "InAir" },
            { LocomotionMainState.Land, "Land" },
            { LocomotionMainState.Slide, "Slide" },
            { LocomotionMainState.Climb, "Climb" },
            { LocomotionMainState.WallRun, "WallRun" }
        }; // Ʈ���ŵ� �ʿ��� ���

        public void InitializeCreateMainStateMap(PlayerCore player)
        {
            m_MainStrategyMap[LocomotionMainState.Idle] = new IdleState(player);
            m_MainStrategyMap[LocomotionMainState.Move] = new MoveState(player);
            m_MainStrategyMap[LocomotionMainState.Jump] = new JumpState(player);
            m_MainStrategyMap[LocomotionMainState.InAir] = new InAirState(player);
            m_MainStrategyMap[LocomotionMainState.Land] = new LandState(player);
            m_MainStrategyMap[LocomotionMainState.Slide] = new SlideState(player);
            m_MainStrategyMap[LocomotionMainState.Climb] = new ClimbState(player);
            m_MainStrategyMap[LocomotionMainState.WallRun] = new WallRunState(player);
        }

        public void SetMainStateAnimation(LocomotionMainState locomotionMainState, Animator animator, AniParmType aniParmType, bool isPlay = false)
        {
            switch (aniParmType)
            {
                case AniParmType.SetBool:
                    animator.SetBool(m_MainStateAniMap[locomotionMainState], isPlay);
                    break;
                case AniParmType.SetTrigger:
                    animator.SetTrigger(m_MainTriggerAniMap[locomotionMainState]);
                    break;
            }
        }
        #endregion ======================================== /MainSate ����

        #region ======================================== SubFlags ����
        private HashSet<LocomotionSubFlags> m_CurrentFlags = new();
        private readonly Dictionary<LocomotionSubFlags, string> m_FlagAniMap = new()
        {
            { LocomotionSubFlags.Run, "IsRun" },
            { LocomotionSubFlags.Crouch, "IsCrouch" },
            { LocomotionSubFlags.CrouchRun, "IsCrouchRun" },
        };

        //HashSet�� Add �ߺ� �ڵ� ����
        /// <summary>
        /// Flag + Ani ��� ����
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="animator"></param>
        public void SetLocomotionFlag(LocomotionSubFlags flag, Animator animator)
        {
            m_CurrentFlags.Add(flag); 
            animator.SetBool(m_FlagAniMap[flag], true);
        }

        public void RemoveLocomotionFlag(LocomotionSubFlags flag, Animator animator)
        {
            animator.SetBool(m_FlagAniMap[flag], false);
            m_CurrentFlags.Remove(flag);
        }
        public bool HasLocomotionFlag(LocomotionSubFlags flag) => m_CurrentFlags.Contains(flag);
        public void AllClearFlags(Animator animator)
        {
            m_CurrentFlags.Clear();
            foreach(var flag in m_FlagAniMap.Keys)
            {
                animator.SetBool(m_FlagAniMap[flag], false);
            }
        } 
        #endregion ======================================== /SubFlags ����


    }
}
