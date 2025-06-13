using System.Collections.Generic;
using UnityEngine;

public class DeadBodyManager : MonoBehaviour
{
    public static DeadBodyManager Instance { get; private set; }
    public GameObject deadBodyPrefab;
    private List<DeadBody> deadBodies = new List<DeadBody>();

    void Awake()
    {
        Instance = this;
    }

    public void SpawnDeadBody(Vector3 position, string playerID)
    {
        GameObject body = Instantiate(deadBodyPrefab, position, Quaternion.identity);
        body.GetComponent<DeadBody>().Initialize(playerID);
    }

    public string GetClosestDeadBodyID(Vector3 position)
    {
        DeadBody closest = null;
        float minDistance = float.MaxValue;

        foreach (var body in deadBodies)
        {
            if (body == null) continue;

            float dist = Vector3.Distance(position, body.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = body;
            }
        }

        return (closest != null && minDistance <= 2.0f) ? closest.BodyID : null;
    }
}
