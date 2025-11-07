using System.Net.Security;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] PlayerData playerData;
    [SerializeField] float BasicSpeed;

    float speedToleranz = 2.5f;
    float speed;

    private void Start()
    {
        speed = Random.Range(BasicSpeed - speedToleranz, BasicSpeed + speedToleranz * 0.1f);
    }

    private void Update()
    {
        Vector3 direction = Vector3.Normalize(playerData.playerPosition - transform.position);
        direction.y = 0;
        //Debug.Log($"Direction: {direction}");
        if(direction.x != 0 && direction.z != 0)
        {
            transform.forward = direction;
            transform.position += direction * Time.deltaTime * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "player") return;
        Destroy(gameObject);
    }
}
