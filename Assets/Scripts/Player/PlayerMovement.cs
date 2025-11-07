using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] PlayerData data;
    Vector3 movement = new Vector3();

    private void Update()
    {
        transform.position += movement * Time.deltaTime * speed;
        data.playerPosition = transform.position;
    }

    public void GetPlayerMovement(InputAction.CallbackContext context)
    {
        movement.x = context.ReadValue<Vector2>().x;
        movement.z = context.ReadValue<Vector2>().y;
    }
}
