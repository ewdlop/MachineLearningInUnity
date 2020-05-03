using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Action
{
    None,
    Right,
    Down,
    Left,
    Up
}
[Serializable]
public class Agent : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private int _step;
    [SerializeField]
    private int _iteration;
    [SerializeField]
    private int _currentGridX;
    [SerializeField]
    private int _currentGridY;
    [SerializeField]
    private (int,int)? _previousState = null;
    [SerializeField]
    private Action? _previousAction = null;
    [SerializeField]
    private float? _previousReward = null;
    [SerializeField]
    private GUIController _gUIController;
    [SerializeField]
    [Range(0f, 1f)]
    private float _learningRate;
    [SerializeField]
    [Range(0f, 1f)]
    private float _discountingFactor;
    //For exploration(shorten gain vs curiosity)
    [SerializeField]
    private int _mimumumStateActionPairFrequencies;
    [SerializeField]
    private float _estimatedBestPossibleRewardValue;
    [SerializeField]
    private Coroutine _waitThenActionCoroutine;
    public int Step { get => _step; set => _step = value; }
    public int Iteration { get => _iteration; set => _iteration = value; }
    public int CurrentGridX { get => _currentGridX; set => _currentGridX = value; }
    public int CurrentGridY { get => _currentGridY; set => _currentGridY = value; }
    public (int, int)? PreviousState { get => _previousState; set => _previousState = value; }
    public Action? PreviousAction { get => _previousAction; set => _previousAction = value; }
    public float? PreviousReward { get => _previousReward; set => _previousReward = value; }
    public GUIController GUIController { get => _gUIController; set => _gUIController = value; }
    public float LearningRate { get => _learningRate; set => _learningRate = value; }
    public float DiscountingFactor { get => _discountingFactor; set => _discountingFactor = value; }
    public int MimumumStateActionPairFrequencies { get => _mimumumStateActionPairFrequencies; set => _mimumumStateActionPairFrequencies = value; }
    public float EstimatedBestPossibleRewardValue { get => _estimatedBestPossibleRewardValue; set => _estimatedBestPossibleRewardValue = value; }
    public Coroutine WaitThenActionCoroutine { get => _waitThenActionCoroutine; set => _waitThenActionCoroutine = value; }

    public (int,int) StartState;
    public (int,int) FinalState = (7,9);
    
    public int StartX;
    public int StartY;
    
    public int GrizSizeX;
    public int GrizSizeY;

    public Dictionary<((int,int),Action),float> StateActionPairQValue;
    public Dictionary<((int,int), Action),int> StateActionPairFrequencies;
    public Dictionary<(int, int), float> StateRewardGrid;
    public Dictionary<Action, System.Action> ActionDelegatesDictonary;
    #endregion

    #region  Q_Learning_Agent
    private Action Q_Learning_Agent((int,int) currentState, float rewardSignal)
    {
        UpdateStep();
        if (PreviousState == FinalState)
        {
            StateActionPairQValue[(PreviousState.Value, Action.None)] = rewardSignal;
        }

        if(PreviousState.HasValue)
        {
            ((int, int), Action) stateActionPair = (PreviousState.Value, PreviousAction.Value);
            StateActionPairFrequencies[stateActionPair]++;
            StateActionPairQValue[stateActionPair] += LearningRate *
                (StateActionPairFrequencies[stateActionPair]) * (PreviousReward.Value + 
                DiscountingFactor * MaxStateActionPairQValue(ref currentState) - StateActionPairQValue[stateActionPair]);
        }
        PreviousState = currentState;
        PreviousAction = ArgMaxActionExploration(ref currentState);
        PreviousReward = rewardSignal;
        return PreviousAction.Value;
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
                
            if (CurrentGridX - 1 < 0 && action == Action.Left)
            {
                continue;
            }
            if (CurrentGridX + 1 >= GrizSizeX && action == Action.Right)
            {
                continue;
            }
            if (CurrentGridY + 1 >= GrizSizeY && action == Action.Up)
            {
                continue;
            }
            if (CurrentGridY - 1 < 0 && action == Action.Down)
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

            if (CurrentGridX - 1 < 0 && action == Action.Left)
            {
                continue;
            }
            if(CurrentGridX + 1 >= GrizSizeX && action == Action.Right)
            {
                continue;
            }
            if (CurrentGridY + 1 >= GrizSizeY && action == Action.Up)
            {
                continue;
            }
            if (CurrentGridY - 1 < 0 && action == Action.Down)
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
        CurrentGridX--;
        WaitThenActionCoroutine = StartCoroutine(WaitThenAction(0.01f, (CurrentGridX, CurrentGridY)));
    }

    private void Right()
    {
        transform.position += new Vector3(1f, 0f, 0f);
        CurrentGridX++;
        WaitThenActionCoroutine = StartCoroutine(WaitThenAction(0.01f, (CurrentGridX, CurrentGridY)));
    }

    private void Up()
    {
        transform.position += new Vector3(0f, 0f, 1f);
        CurrentGridY++;
        WaitThenActionCoroutine = StartCoroutine(WaitThenAction(0.01f, (CurrentGridX, CurrentGridY)));
    }

    private void Down()
    {
        transform.position -= new Vector3(0f, 0f, 1f);
        CurrentGridY--;
        WaitThenActionCoroutine = StartCoroutine(WaitThenAction(0.01f, (CurrentGridX, CurrentGridY)));
    }

    private void None()
    {
        ResetAgentToStart();
        UpdateIteration();
        WaitThenActionCoroutine = StartCoroutine(WaitThenAction(0.01f, StartState));
    }

    private void ResetAgentToStart()
    {
        PreviousAction = null;
        PreviousReward = null;
        PreviousState = null;
        transform.position = new Vector3(StartState.Item1, 1f, StartState.Item2);
        CurrentGridX = StartState.Item1;
        CurrentGridY = StartState.Item2;
        Grid.instance.ClearColors();
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
        FinalState = Grid.instance.goalPosition;
        ActionDelegatesDictonary = new Dictionary<Action, System.Action>();
        ActionDelegatesDictonary[Action.Left] = Left;
        ActionDelegatesDictonary[Action.Right] = Right;
        ActionDelegatesDictonary[Action.Up] = Up;
        ActionDelegatesDictonary[Action.Down] = Down;
        ActionDelegatesDictonary[Action.None] = None;
        Initialized();
    }

    private void Initialized()
    {
        PreviousAction = null;
        PreviousReward = null;
        PreviousState = null;
        Step = 0;
        Iteration = 0;
        transform.position = new Vector3(StartX, 1f, StartY);
        StartState = (StartX, StartY);
        CurrentGridX = StartState.Item1;
        CurrentGridY = StartState.Item2;
        StateActionPairQValue = new Dictionary<((int, int), Action), float>();
        StateActionPairFrequencies = new Dictionary<((int, int), Action), int>();
        StateRewardGrid = new Dictionary<(int, int), float>();

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
                    StateRewardGrid[(i, j)] = 1f;
                }
            }
        }
        StateRewardGrid[FinalState] = 500f;
    }

    private void Update()
    {
        Grid.instance.UpdateColor(CurrentGridX, CurrentGridY);
    }

    public void StartExploring()
    {
        UpdateIteration();
        WaitThenActionCoroutine = StartCoroutine(WaitThenAction(0.1f, StartState));
    }

    public void Stop()
    {
        Initialized();
        StopCoroutine(WaitThenActionCoroutine);
    }

    private void UpdateStep()
    {
        Step++;
        GUIController?.UpdateStepText(Step.ToString());
    }

    private void UpdateIteration()
    {
        Iteration++;
        GUIController?.UpdateInterationText(Iteration.ToString());
    }
    #endregion
}