using UnityEngine;

public class SimpleAgent : MonoBehaviour
{
    public Transform player;

    public float detectionRange = 10f;
    public float fleeRange = 3f;
    public float speed = 3f;

    enum State
    {
        Idle,
        Chase,
        Flee
    }

    State currentState;

    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        currentState = State.Idle;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // 🔹 SENSING → bepaalt state
        if (distance < fleeRange)
        {
            currentState = State.Flee;
        }
        else if (distance < detectionRange)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Idle;
        }

        // 🔹 ACT → gedrag uitvoeren
        switch (currentState)
        {
            case State.Idle:
                rend.material.color = Color.green;
                break;

            case State.Chase:
                rend.material.color = Color.red;
                MoveTowards(player.position);
                break;

            case State.Flee:
                rend.material.color = Color.blue;
                MoveAway(player.position);
                break;
        }
    }

    void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }

    void MoveAway(Vector3 target)
    {
        Vector3 direction = (transform.position - target).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}