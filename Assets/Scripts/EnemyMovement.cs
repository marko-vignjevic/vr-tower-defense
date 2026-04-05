using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public float speed = 3.0f;
    public int waypointIndex = 0;
    private Transform[] waypoints;

    private float currentSpeed;
    private float slowTimer = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waypoints = PathManager.Instance.waypoints;
        transform.position = waypoints[0].position;
        currentSpeed = speed;

    }

    // Update is called once per frame
    void Update()
    {
        // If the enemy is currently slowed, update the 
        // slow timer and reset speed when the slow effect wears off
        if (slowTimer > 0.0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                currentSpeed = speed;
            }
        }

        if (waypointIndex >= waypoints.Length)
        {
            return;
        }

        // Move towards the next waypoint
        Transform nextWaypoint = waypoints[waypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, nextWaypoint.position, currentSpeed * Time.deltaTime);
        transform.LookAt(nextWaypoint);

        if (Vector3.Distance(transform.position, nextWaypoint.position) < 0.1f)
        {
            waypointIndex++;
        }

        // If we've reached the end of the path,
        // damage the player and destroy this enemy
        if (waypointIndex >= waypoints.Length)
        {
            EnemyManager enemy = GetComponent<EnemyManager>();
            PlayerManager.Instance.TakeDamage(enemy.damage);
            Destroy(gameObject);
        }
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        // Apply a slow effect to the enemy by
        // reducing its speed
        currentSpeed = speed * (1.0f - slowAmount);
        slowTimer = duration;
    }

}
