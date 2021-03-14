using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JaysUnityUtils;
using UnityEngine.InputSystem;

public class RewardSpawner : MonoBehaviour
{
    private Transform reward;
    private Bounds spawningBounds;
    private SpriteRenderer spawningBoundsSprite;

    public Transform Reward
    {
        get
        {
            return reward;
        }
    }


    private void Awake()
    {
        reward = gameObject.FindComponentInChildWithTag<Transform>( "Reward");
        if(reward == null) Debug.LogError("Reward object not found");

        spawningBoundsSprite = transform.Find("Bounds").GetComponent<SpriteRenderer>();
        if (reward == null) Debug.LogError("Spawning bounds SpriteRenderer not found");

        spawningBounds = spawningBoundsSprite.bounds;

        spawningBoundsSprite.enabled = false;
    }

    public void SpawnReward()
    {
        float x_local = Random.value;
        float y_local = Random.value;

        x_local = Mathf.Lerp(spawningBounds.max.x, spawningBounds.min.x, x_local);
        y_local = Mathf.Lerp(spawningBounds.max.y, spawningBounds.min.y, y_local);

        reward.position = new Vector3(x_local, y_local, reward.position.z);
    }

    private void Update()
    {
        //if (Keyboard.current.qKey.wasPressedThisFrame)
        //{
        //    SpawnReward();
        //}
    }
}
