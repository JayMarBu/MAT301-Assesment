using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;
using Unity.MLAgents.Policies;
using Unity.Barracuda;

public class GameController : MonoBehaviour
{
    public enum BrainType
    {
        Player,
        RL,
        Imitation
    }

    [SerializeField] TMPro.TMP_Dropdown[] brainSelection;
    [SerializeField] NNModel[] brains;
    [SerializeField] BoidAgent[] agents;
    [SerializeField] int agentsAlive;

    public Color[] snakeColours;

    bool gameInProgress = false;

    public void Start()
    {
        agentsAlive = agents.Length;
        foreach(BoidAgent agent in agents)
        {
            agent.gameObject.SetActive(false);
        }
    }

    public void AgentCollectedFood(int index)
    {
        foreach(BoidAgent agent in agents)
        {
            if (agent.id != index)
                agent.AddReward(-1f);
        }
    }

    public void AgentDied(int index)
    {
        agentsAlive--;

        if(agentsAlive <= 1)
        {
            foreach (BoidAgent agent in agents)
            {
                agent.EndEpisode();
            }

            foreach (BoidAgent agent in agents)
            {
                agent.UpdateFinalUi(0);
                agent.Tail.m_length = 10;
                agent.gameObject.SetActive(false);
                gameInProgress = false;
            }
        }
    }

    public void StartGame()
    {
        if (gameInProgress == false)
        {
            gameInProgress = true;
            int index = 0;
            foreach (BoidAgent agent in agents)
            {
                agent.gameObject.SetActive(true);

                switch (brainSelection[index].value)
                {
                    case 0:
                        agent.gameObject.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.HeuristicOnly;
                        break;

                    case 1:
                        agent.gameObject.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
                        agent.gameObject.GetComponent<BehaviorParameters>().Model = brains[0];
                        break;

                    case 2:
                        agent.gameObject.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
                        agent.gameObject.GetComponent<BehaviorParameters>().Model = brains[1];
                        break;

                    default:
                        agent.gameObject.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
                        agent.gameObject.GetComponent<BehaviorParameters>().Model = brains[0];
                        break;
                }

                index++;
            }
        }
    }
}
