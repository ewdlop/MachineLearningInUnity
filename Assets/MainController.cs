using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Start,
    Move,
    End
}

public enum Action
{
    None,
    Left,
    Right,
    Up,
    Down
}

public class MainController : MonoBehaviour
{
    public State? previousState = null;
    public Action? previousAction = null;
    public float? previousReward = null;

    //Between 0 and 1
    private const float LEARNINGRATE = 0.1f;
    private const float DISCOUNTINGFACTOR = 0f;
    private const State FINALSTATE = State.End;

    //For exploration(greed vs curiosity)
    private const float BestPossibleRewardOptismisticEstimate = 0f;
    private const int MimumumStateActionStateActionPairFrequencies = 1;

    public Dictionary<(State, Action), float> StateActionPairQValue;
    public Dictionary<(State, Action), int> StateActionPairFrequencies;
    public Dictionary<Action, System.Action> ActionDelegatesDictonary;

    public Action Q_Learning_Agent(State currentState, float rewardSignal)
    {
        if(currentState == FINALSTATE)
        {
            StateActionPairQValue[(State.End, Action.None)] = rewardSignal;
        }
        if(previousState.HasValue)
        {
            (State, Action) stateActionPair = (previousState.Value, previousAction.Value);
            StateActionPairFrequencies[stateActionPair]++;
            StateActionPairQValue[stateActionPair] += LEARNINGRATE *
                (StateActionPairFrequencies[stateActionPair]) * (previousReward.Value + 
                DISCOUNTINGFACTOR * MaxStateActionPairQValue(ref currentState) - StateActionPairQValue[stateActionPair]);
        }
        previousState = currentState;
        previousAction = ArgMaxActionExploration(ref currentState);
        previousReward = rewardSignal;
        return previousAction.Value;
    }

    //Page 844
    private float MaxStateActionPairQValue(ref State currentState)
    {
        float max = 0f;
        foreach (Action action in Enum.GetValues(typeof(Action)))
        {
            max = Mathf.Max(StateActionPairQValue[(currentState, action)]);
        }
        return max;
    }
    //Is this also the "Expected" value?
    private Action ArgMaxActionExploration(ref State currentState)
    {
        Action argMaxAction = Action.None;
        float max = 0f;
        
        foreach (Action action in Enum.GetValues(typeof(Action)))
        {
            float value = ExplorationFunction(ref currentState, action);
            if(value >= max)
            {
                max = value;
                argMaxAction = action;
            }
        }
        return argMaxAction;
    }
    //Page 842, this function is not well defined apparently
    //Give the agent the option to have the incentives to explore more?
    private float ExplorationFunction(ref State currentState, Action choice)
    {
        if(StateActionPairFrequencies[(currentState,choice)] < MimumumStateActionStateActionPairFrequencies)
        {
            return BestPossibleRewardOptismisticEstimate;
        }
        return StateActionPairQValue[(currentState, choice)];
    }

    private void Start()
    {
        foreach (State state in Enum.GetValues(typeof(State)))
        {
            foreach (Action action in Enum.GetValues(typeof(Action)))
            {
                StateActionPairQValue[(state, action)] = 0;
                StateActionPairFrequencies[(state, action)] = 0;
            }
        }
        ActionDelegatesDictonary = new Dictionary<Action, System.Action>();
        ActionDelegatesDictonary[Action.Left] = Left;
        ActionDelegatesDictonary[Action.Right] = Right;
        ActionDelegatesDictonary[Action.Up] = Up;
        ActionDelegatesDictonary[Action.Down] = Down;
        ActionDelegatesDictonary[Action.Left]();
    }

    private void Update()
    {

    }

    private void Left()
    {

    }

    private void Right()
    {

    }

    private void Up()
    {

    }

    private void Down()
    {

    }
}