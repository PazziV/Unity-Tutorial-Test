using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    [SerializeField] PlayerData data;
    private void Update()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.value);
        if(Physics.Raycast(ray,out RaycastHit hitInfo))
        {
            Vector3 lookPoint = hitInfo.point;
            lookPoint.y = 0;    
            data.lookPoint = lookPoint;
        }
    }
}
