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

public class Agent : MonoBehaviour
{
    public int count = 0;

    public (int,int)? previousState = null;
    public Action? previousAction = null;
    public float? previousReward = null;

    //Between 0 and 1
    public float LEARNINGRATE;
    public float DISCOUNTINGFACTOR;
    public (int,int) START;
    public (int,int) FINALSTATE = (7,9);

    //For exploration(greed vs curiosity)
    public float EstimatedBestPossibleRewardValue;
    public int MimumumStateActionPairFrequencies;

    public Dictionary<((int,int),Action),float> StateActionPairQValue;
    public Dictionary<((int,int), Action),int> StateActionPairFrequencies;
    public Dictionary<Action, System.Action> ActionDelegatesDictonary;

    public int GridX;
    public int GridY;

    public int STARTX;
    public int STARTY;

    public int GrizSizeX;
    public int GrizSizeY;
    public Dictionary<(int, int), float> StateRewardGrid;

    private Action Q_Learning_Agent((int,int) currentState, float rewardSignal)
    {
        count++;
        if (previousState == FINALSTATE)
        {
            StateActionPairQValue[(previousState.Value, Action.None)] = rewardSignal;
        }
        if(previousState.HasValue)
        {
            ((int, int), Action) stateActionPair = (previousState.Value, previousAction.Value);
            StateActionPairFrequencies[stateActionPair]++;
            StateActionPairQValue[stateActionPair] += LEARNINGRATE *
                (StateActionPairFrequencies[stateActionPair]) * (previousReward.Value + 
                DISCOUNTINGFACTOR * MaxStateActionPairQValue(ref currentState) - StateActionPairQValue[stateActionPair]);
        }
        previousState = currentState;
        //if(currentState == FINALSTATE)
        //{
        //    return Action.None;
        //}
        //else
            previousAction = ArgMaxActionExploration(ref currentState);
        previousReward = rewardSignal;
        return previousAction.Value;
    }

    //Page 844
    private float MaxStateActionPairQValue(ref (int, int) currentState)
    {
        float max = float.NegativeInfinity;
        foreach (Action action in Enum.GetValues(typeof(Action)))
        {
            if (currentState == FINALSTATE)
                return StateActionPairQValue[(currentState, Action.None)];
            
            if (action == Action.None)
                continue;
                
            if (GridX - 1 < 0 && action == Action.Left)
            {
                continue;
            }
            if (GridX + 1 >= GrizSizeX && action == Action.Right)
            {
                continue;
            }
            if (GridY + 1 >= GrizSizeY && action == Action.Up)
            {
                continue;
            }
            if (GridY - 1 < 0 && action == Action.Down)
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
            if (currentState == FINALSTATE)
                return Action.None;

            if (action == Action.None)
                continue;

            if (GridX - 1 < 0 && action == Action.Left)
            {
                continue;
            }
            if(GridX + 1 >= GrizSizeX && action == Action.Right)
            {
                continue;
            }
            if (GridY + 1 >= GrizSizeY && action == Action.Up)
            {
                continue;
            }
            if (GridY - 1 < 0 && action == Action.Down)
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

    private void Start()
    {
        transform.position = new Vector3(STARTX, 0f, STARTY);
        
        START = (STARTX, STARTY);

        GridX = START.Item1;
        GridY = START.Item2;

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
                    StateActionPairQValue[((i,j), action)] = 0;
                    StateActionPairFrequencies[((i, j), action)] = 0;
                }
                if (i == 0 || j == 0 || i == GrizSizeX - 1 || j == GrizSizeY - 1)
                {
                    StateRewardGrid[(i, j)] = -1f;
                }
                else
                {
                    StateRewardGrid[(i, j)] = 0f;
                }
            }
        }
        StateRewardGrid[FINALSTATE] = 500f;
        StartCoroutine(WaiThenAction(0.1f, START));
    }

    private void Update()
    {

    }

    private void Left()
    {
        transform.position -= new Vector3(1f, 0f, 0f);
        GridX--;
        //if(GridX < 0 || GridX >= GrizSizeX)
        //{
        //    GridX = START.Item1;
        //    GridY = START.Item2;
        //    transform.position = new Vector3(START.Item1, 1f, START.Item2);
        //}
        StartCoroutine(WaiThenAction(0.1f, (GridX, GridY)));
    }

    private void Right()
    {
        transform.position += new Vector3(1f, 0f, 0f);
        GridX++;
        //if (GridX < 0 || GridX >= GrizSizeX)
        //{
        //    GridX = START.Item1;
        //    GridY = START.Item2;
        //    transform.position = new Vector3(START.Item1, 1f, START.Item2);
        //}
        StartCoroutine(WaiThenAction(0.1f, (GridX, GridY)));
    }

    private void Up()
    {
        transform.position += new Vector3(0f, 0f, 1f);
        GridY++;
        //if (GridY < 0 || GridY >= GrizSizeY)
        //{
        //    GridX = START.Item1;
        //    GridY = START.Item2;
        //    transform.position = new Vector3(START.Item1, 1f, START.Item2);
        //}
        StartCoroutine(WaiThenAction(0.1f, (GridX, GridY)));
    }

    private void Down()
    {
        transform.position -= new Vector3(0f, 0f, 1f);
        GridY--;
        //if (GridY < 0 || GridY >= GrizSizeY)
        //{
        //    GridX = START.Item1;
        //    GridY = START.Item2;
        //    transform.position = new Vector3(START.Item1, 1f, START.Item2);
        //}
        StartCoroutine(WaiThenAction(0.1f, (GridX, GridY)));
    }

    private void None()
    {
        previousAction = null;
        previousReward = null;
        previousState = null;
        transform.position = new Vector3(START.Item1, 1f, START.Item2);
        GridX = START.Item1;
        GridY = START.Item2;
        StartCoroutine(WaiThenAction(0.1f, START));
    }

    private IEnumerator WaiThenAction(float waitTime, (int,int) GridCoordinate)
    {
        yield return new WaitForSeconds(waitTime);
        ActionDelegatesDictonary[Q_Learning_Agent(GridCoordinate, StateRewardGrid[GridCoordinate])]();
    }
}