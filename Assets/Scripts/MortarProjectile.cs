using UnityEngine;

public class MortarProjectile : MonoBehaviour
{
    public float damage = 50f;
    public float aoeRadius = 3f;
    public float arcHeight = 10f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float travelTime = 2f;
    private float elapsedTime = 0f;
    private bool hasLanded = false;

    public void SetTarget(Vector3 target, float time = 2f)
    {
        startPos = transform.position;
        targetPos = target;
        travelTime = time;
    }

    void Update()
    {
        // If the projectile has already landed, do nothing
        if (hasLanded)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / travelTime;

        if (t >= 1f)
        {
            transform.position = targetPos;
            Land();
            return;
        }

        // Calculate the parabolic arc position
        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
        currentPos.y += arcHeight * Mathf.Sin(Mathf.PI * t);
        transform.position = currentPos;
    }

    void Land()
    {
        hasLanded = true;

        // Find all enemies in the projectiles radius and apply damage
        Collider[] hits = Physics.OverlapSphere(targetPos, aoeRadius);
        foreach (Collider hit in hits)
        {
            EnemyManager enemy = hit.GetComponent<EnemyManager>();
            if (enemy == null)
            {
                enemy = hit.GetComponentInParent<EnemyManager>();
            }
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);
            }
        }
        
        Destroy(gameObject);
    }
}
