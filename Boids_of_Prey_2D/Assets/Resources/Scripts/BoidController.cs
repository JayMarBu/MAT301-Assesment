using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class BoidController : MonoBehaviour
{
    private Rigidbody2D rb;

    private float thrust;
    private float rotation;

    [Header("Boid Variables")]
    [SerializeField] private float thrustMultiplier;
    [SerializeField] private float rotationMultiplier;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float tiltCoefficient;

    [Header("Debug Render Info")]
    [SerializeField] private TMPro.TextMeshPro[] UIInfo;

    [Header("Dependencies")]
    [SerializeField] RewardSpawner rewardSpawner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void GetInputs()
    {
        bool lkey = Keyboard.current.aKey.isPressed; // -
        bool rkey = Keyboard.current.dKey.isPressed; // +
        bool tkey = Keyboard.current.spaceKey.isPressed;

        rotation = (float)(Convert.ToInt32(lkey) - Convert.ToInt32(rkey));
        thrust = (float)(Convert.ToInt32(tkey));
        
    }

    private void Update()
    {
        float tilt = (transform.up.y/2)+0.5f;
        //rb.gravityScale = (tilt*tiltCoefficient)+(1-tiltCoefficient);
        rb.gravityScale = 1-(tilt*tiltCoefficient);

        UIInfo[0].text = "Gravity Scale: " + rb.gravityScale;
        UIInfo[1].text = "Thrust: " + thrust;
        UIInfo[2].text = "Velocity: ";
        UIInfo[3].text = "x: " + rb.velocity.x;
        UIInfo[4].text = "y: " + rb.velocity.y;

        GetInputs();
    }

    private void FixedUpdate()
    {
        float thrustForce = thrust * thrustMultiplier;
        rb.AddForce(transform.up * thrustForce);
        rb.AddTorque(rotation * rotationMultiplier);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Reward"))
        {
            GainReward();
        }
    }

    private void GainReward()
    {
        rewardSpawner.SpawnReward();
    }
}
