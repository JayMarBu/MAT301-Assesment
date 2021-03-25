using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class BoidAgent : Agent
{
    public bool displayDebugUI = false;

    private Rigidbody2D rb;
    private Vector3 spawnPos;

    [Header("Head Variables")]
    [SerializeField] private float thrustMultiplier;
    [SerializeField] private float rotationMultiplier;
    public int id;

    [Header("Debug Render Info")]
    [SerializeField] private TMPro.TextMeshPro[] UIInfo;

    [Header("Dependencies")]
    [SerializeField] RewardSpawner rewardSpawner;
    [SerializeField] SpriteRenderer background;
    [SerializeField] GameObject bodySegmentPrefab;
    [SerializeField] Tail tail;
    [SerializeField] GameController gameController;


    public Tail Tail
    {
        get { return tail; }
    }


    public override void Initialize()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        spawnPos = transform.position;
    }

    public override void OnEpisodeBegin()
    {
        if(gameController != null)
            tail.SetTailColour(gameController.snakeColours[id]);
        transform.position = spawnPos;
        rb.velocity = Vector3.zero;
        rb.rotation = 0;
        rb.angularVelocity = 0;
        rewardSpawner.SpawnReward();

        tail.ResetTail();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 targetDir = (rewardSpawner.Reward.transform.position - transform.position).normalized;
        sensor.AddObservation(targetDir);
        sensor.AddObservation(transform.up);
        sensor.AddObservation(Vector3.Distance(transform.position, rewardSpawner.Reward.transform.position));
        sensor.AddObservation(tail.m_length);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        bool lkey = Keyboard.current.aKey.isPressed; // -
        bool rkey = Keyboard.current.dKey.isPressed; // +

        continuousActions[0] = (float)(Convert.ToInt32(lkey) - Convert.ToInt32(rkey));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float rotation = Mathf.Clamp( actions.ContinuousActions[0], -1f,1f);

        rb.velocity = (transform.up * thrustMultiplier);
        tail.m_speed = thrustMultiplier;

        rb.rotation += rotation * rotationMultiplier * 100 * Time.deltaTime;

        GrantRewards();

        if (displayDebugUI)
            UpdateUi(rotation);
        else
            UpdateFinalUi(rotation);
    }

    public void GrantRewards()
    {
        Vector3 targetDir = (rewardSpawner.Reward.transform.position - transform.position).normalized;

        if(MaxStep > 0) AddReward(-1f/MaxStep);
    }

    public void UpdateUi(float input)
    {
        UIInfo[0].text = "Length: " + tail.m_length;
        UIInfo[1].text = "Rotation: " + input;
        UIInfo[2].text = "Velocity: ";
        UIInfo[3].text = "x: " + rb.velocity.x;
        UIInfo[4].text = "y: " + rb.velocity.y;
    }

    public void UpdateFinalUi(float input)
    {
        UIInfo[0].text = tail.m_length.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Reward"))
        {
            AddReward(+1.0f);
            tail.AddSegement();
            rewardSpawner.SpawnReward();
            StartCoroutine(ChangeBackground(Color.green * new Color(1, 1, 1, 0.1f)));
            if (gameController != null)
                gameController.AgentCollectedFood(id);
            //EndEpisode();
        }

        if (collision.CompareTag("Boundary")||collision.CompareTag("Body"))
        {
            AddReward(-10.0f);
            StartCoroutine(ChangeBackground(Color.red * new Color(1, 1, 1, 0.1f)));
            if (gameController != null)
                gameController.AgentDied(id);
            else
                EndEpisode();
        }
    }

    IEnumerator ChangeBackground(Color colour)
    {
        background.color = colour;
        yield return new WaitForSeconds(2);
        background.color = new Color(1,1,1,0.105f);
    }
}
