using System.Net.Security;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] float Speed;

    private void Update()
    {
        Vector3 direction = Vector3.Normalize(Player.transform.position - transform.position);
        direction.y = 0;
        //Debug.Log($"Direction: {direction}");
        if(direction.x != 0 && direction.z != 0)
        {
            transform.forward = direction;
            transform.position += direction * Time.deltaTime * Speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "player") return;
        Destroy(gameObject);
    }
}
