using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    // Start is called before the first frame update
    void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch(state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        // no more actions that enemy can take
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }

    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionCompleted)
    {
        Debug.Log("take enemy ai action");
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionCompleted))
            {
                return true;
            }
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionCompleted)
    {
        SpinAction spinAction = enemyUnit.GetSpinAction();

        GridPosition actionGridPosiion = enemyUnit.GetGridPosition();

        if (!spinAction.IsValidActionGridPosition(actionGridPosiion))
        {
            return false;
        }

        if (!enemyUnit.TrySpendActionPointsToTakeAction(spinAction))
        {
            return false;
        }

        Debug.Log(" enemy spin action");
        spinAction.TakeAction(actionGridPosiion, onEnemyAIActionCompleted);
        return true;
    }
}
