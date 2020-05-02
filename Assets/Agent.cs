using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action
{
    None,
    Left,
    Right,
    Up,
    Down
}
[Serializable]
public class Agent : MonoBehaviour
{
    [SerializeField]
    private int _step;
    [SerializeField]
    private int _currentGridX;
    [SerializeField]
    private int _currentGridY;
    private (int,int)? _previousState = null;
    private Action? _previousAction = null;
    [SerializeField]
    private float? _previousReward = null;

    //Between 0 and 1
    [Range(0f,1f)]
    public float LearningRate;
    [Range(0f, 1f)]
    public float DiscountingFactor;
    public (int,int) StartState;
    public (int,int) FinalState = (7,9);
    
    public int StartX;
    public int StartY;
    
    public int GrizSizeX;
    public int GrizSizeY;
    
    //For exploration(shorten gain vs curiosity)
    public int MimumumStateActionPairFrequencies;
    public float EstimatedBestPossibleRewardValue;

    public Dictionary<((int,int),Action),float> StateActionPairQValue;
    public Dictionary<((int,int), Action),int> StateActionPairFrequencies;
    public Dictionary<(int, int), float> StateRewardGrid;
    public Dictionary<Action, System.Action> ActionDelegatesDictonary;

    #region  Q_Learning_Agent
    private Action Q_Learning_Agent((int,int) currentState, float rewardSignal)
    {
        _step++;
        if (_previousState == FinalState)
        {
            StateActionPairQValue[(_previousState.Value, Action.None)] = rewardSignal;
        }

        if(_previousState.HasValue)
        {
            ((int, int), Action) stateActionPair = (_previousState.Value, _previousAction.Value);
            StateActionPairFrequencies[stateActionPair]++;
            StateActionPairQValue[stateActionPair] += LearningRate *
                (StateActionPairFrequencies[stateActionPair]) * (_previousReward.Value + 
                DiscountingFactor * MaxStateActionPairQValue(ref currentState) - StateActionPairQValue[stateActionPair]);
        }
        _previousState = currentState;
        _previousAction = ArgMaxActionExploration(ref currentState);
        _previousReward = rewardSignal;
        return _previousAction.Value;
    }

    //Page 844
    private float MaxStateActionPairQValue(ref (int, int) currentState)
    {
        float max = float.NegativeInfinity;
        foreach (Action action in Enum.GetValues(typeof(Action)))
        {
            if (currentState == FinalState)
                return StateActionPairQValue[(currentState, Action.None)];
            
            if (action == Action.None)
                continue;
                
            if (_currentGridX - 1 < 0 && action == Action.Left)
            {
                continue;
            }
            if (_currentGridX + 1 >= GrizSizeX && action == Action.Right)
            {
                continue;
            }
            if (_currentGridY + 1 >= GrizSizeY && action == Action.Up)
            {
                continue;
            }
            if (_currentGridY - 1 < 0 && action == Action.Down)
            {
                continue;
            }
            max = Mathf.Max(StateActionPairQValue[(currentState, action)]);
        }
        return max;
    }

    private Action ArgMaxActionExploration(ref (int, int) currentState)
    {
        Action argMaxAction = Action.None;
        float max = float.NegativeInfinity;
        
        foreach (Action action in Enum.GetValues(typeof(Action)))
        {
            if (currentState == FinalState)
                return Action.None;

            if (action == Action.None)
                continue;

            if (_currentGridX - 1 < 0 && action == Action.Left)
            {
                continue;
            }
            if(_currentGridX + 1 >= GrizSizeX && action == Action.Right)
            {
                continue;
            }
            if (_currentGridY + 1 >= GrizSizeY && action == Action.Up)
            {
                continue;
            }
            if (_currentGridY - 1 < 0 && action == Action.Down)
            {
                continue;
            }
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
    private float ExplorationFunction(ref (int, int) currentState, Action choice)
    {
        if(StateActionPairFrequencies[(currentState,choice)] < MimumumStateActionPairFrequencies)
        {
            return EstimatedBestPossibleRewardValue;
        }
        return StateActionPairQValue[(currentState, choice)];
    }

    private void Left()
    {
        transform.position -= new Vector3(1f, 0f, 0f);
        _currentGridX--;
        StartCoroutine(WaitThenAction(0.01f, (_currentGridX, _currentGridY)));
    }

    private void Right()
    {
        transform.position += new Vector3(1f, 0f, 0f);
        _currentGridX++;
        StartCoroutine(WaitThenAction(0.01f, (_currentGridX, _currentGridY)));
    }

    private void Up()
    {
        transform.position += new Vector3(0f, 0f, 1f);
        _currentGridY++;
        StartCoroutine(WaitThenAction(0.01f, (_currentGridX, _currentGridY)));
    }

    private void Down()
    {
        transform.position -= new Vector3(0f, 0f, 1f);
        _currentGridY--;
        StartCoroutine(WaitThenAction(0.01f, (_currentGridX, _currentGridY)));
    }

    private void None()
    {
        _previousAction = null;
        _previousReward = null;
        _previousState = null;
        transform.position = new Vector3(StartState.Item1, 1f, StartState.Item2);
        _currentGridX = StartState.Item1;
        _currentGridY = StartState.Item2;
        StartCoroutine(WaitThenAction(0.01f, StartState));
    }

    private IEnumerator WaitThenAction(float waitTime, (int,int) GridCoordinate)
    {
        yield return new WaitForSeconds(waitTime);
        ActionDelegatesDictonary[Q_Learning_Agent(GridCoordinate, StateRewardGrid[GridCoordinate])]();
    }
    #endregion

    #region Unity
    private void Start()
    {
        transform.position = new Vector3(StartX, 0f, StartY);

        StartState = (StartX, StartY);

        _currentGridX = StartState.Item1;
        _currentGridY = StartState.Item2;

        ActionDelegatesDictonary = new Dictionary<Action, System.Action>();
        StateActionPairQValue = new Dictionary<((int, int), Action), float>();
        StateActionPairFrequencies = new Dictionary<((int, int), Action), int>();
        StateRewardGrid = new Dictionary<(int, int), float>();
        ActionDelegatesDictonary[Action.Left] = Left;
        ActionDelegatesDictonary[Action.Right] = Right;
        ActionDelegatesDictonary[Action.Up] = Up;
        ActionDelegatesDictonary[Action.Down] = Down;
        ActionDelegatesDictonary[Action.None] = None;

        for (int i = 0; i < GrizSizeX; i++)
        {
            for (int j = 0; j < GrizSizeY; j++)
            {
                foreach (Action action in Enum.GetValues(typeof(Action)))
                {
                    StateActionPairQValue[((i, j), action)] = 0;
                    StateActionPairFrequencies[((i, j), action)] = 0;
                }
                if (i == 0 || j == 0 || i == GrizSizeX - 1 || j == GrizSizeY - 1)
                {
                    StateRewardGrid[(i, j)] = -10f;
                }
                else
                {
                    StateRewardGrid[(i, j)] = 0f;
                }
            }
        }
        StateRewardGrid[FinalState] = 500f;

    }

    public void StartExploring()
    {
        StartCoroutine(WaitThenAction(0.1f, StartState));
    }
    #endregion
}