using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;
using System;

public class BoidAgent : Agent
{
    private Rigidbody2D rb;
    private Color directionGizmoColor = Color.red;
    private Vector3 spawnPos;

    [Header("Boid Variables")]
    [SerializeField] private float thrustMultiplierBase;
    private float thrustMultiplier;
    [SerializeField] private float rotationMultiplier;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float tiltCoefficient;

    [Header("Debug Render Info")]
    [SerializeField] private TMPro.TextMeshPro[] UIInfo;

    [Header("Dependencies")]
    [SerializeField] RewardSpawner rewardSpawner;
    [SerializeField] SpriteRenderer background;

    public override void Initialize()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        spawnPos = transform.position;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = spawnPos;
        rb.velocity = Vector3.zero;
        rb.rotation = 0;
        rb.angularVelocity = 0;
        rewardSpawner.SpawnReward();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(rewardSpawner.Reward.transform.position);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        bool lkey = Keyboard.current.aKey.isPressed; // -
        bool rkey = Keyboard.current.dKey.isPressed; // +
        bool tkey = Keyboard.current.spaceKey.isPressed;

        //continuousActions[0] = (float)(Convert.ToInt32(tkey));
        continuousActions[0] = (float)(Convert.ToInt32(lkey) - Convert.ToInt32(rkey));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float rotation = actions.ContinuousActions[0];

        float thrustForce = 1f;
        rb.AddForce(transform.up * thrustForce * thrustMultiplierBase);

        rb.rotation += rotation * rotationMultiplier * 100 * Time.deltaTime;

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        GrantRewards();

        UpdateUi(thrustForce, rotation);
    }

    public void GrantRewards()
    {
        Vector3 targetDir = (rewardSpawner.Reward.transform.position - transform.position).normalized;

        if (Mathf.Sign(targetDir.x) == Mathf.Sign(transform.up.x) && Mathf.Sign(targetDir.y) == Mathf.Sign(transform.up.y))
        {
            directionGizmoColor = Color.green;
            AddReward(+0.05f);
        }
        else
        {
            directionGizmoColor = Color.red;
            AddReward(-0.1f);
        }
    }

    public void UpdateUi(float thrust, float rotation)
    {
        UIInfo[0].text = "Gravity Scale: " + rb.gravityScale;
        UIInfo[1].text = "Thrust: " + thrust;
        UIInfo[2].text = "Velocity: ";
        UIInfo[3].text = "x: " + rb.velocity.x;
        UIInfo[4].text = "y: " + rb.velocity.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Reward"))
        {
            AddReward(+100.0f);
            background.color = Color.green * new Color(1, 1, 1, 0.1f);
            EndEpisode();
        }

        if (collision.CompareTag("Boundary"))
        {
            AddReward(-100.0f);
            background.color = Color.red * new Color(1,1,1,0.1f);
            EndEpisode();
        }
    }

    private void OnDrawGizmos()
    {
        if (rewardSpawner.Reward != null)
        {
            Vector3 targetDir = (rewardSpawner.Reward.transform.position - transform.position).normalized;
            Gizmos.color = directionGizmoColor;
            Gizmos.DrawLine(transform.position, transform.position + targetDir);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + transform.up);
        }
    }
}
