//���� ���� ���̿� ���� �켱 �����ο�
public interface IStateTransitionCondition
{
    int Priority { get; }
    bool ShouldTransition();
    LocomotionBaseState GetNextState(); // or IPlayerState, if unified
}