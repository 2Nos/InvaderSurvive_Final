//���� ���� ���̿� ���� �켱 �����ο�
public interface IStateTransitionCondition
{
    int Priority { get; }
    bool ShouldTransition();
    LocomotionStateBase GetNextState(); // or IPlayerState, if unified
}