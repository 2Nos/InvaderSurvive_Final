//다음 상태 전이에 대한 우선 순위부여
public interface IStateTransitionCondition
{
    int Priority { get; }
    bool ShouldTransition();
    LocomotionStateBase GetNextState(); // or IPlayerState, if unified
}