using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    [SerializeField] GameObject Enemy;
    [SerializeField] float cooldown;
    float remainingTime;
    float randomX,randomZ;
    Vector3 randomPosition;

    private void Update()
    {
        if (remainingTime <= 0)
        {
            randomX = Random.Range(-transform.localScale.x/2, transform.localScale.x/2);
            randomZ = Random.Range(-transform.localScale.z/2, transform.localScale.z/2);
            randomPosition = new Vector3(randomX,0,randomZ);
            Instantiate(Enemy, randomPosition, Quaternion.identity);
            remainingTime = cooldown;

        }
        remainingTime -= Time.deltaTime;
    }
}
