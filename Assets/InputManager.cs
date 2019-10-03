using UnityEngine;

public class InputManager : MonoBehaviour
{
    bool isDragging = false;
    GameObject draggingObject;

    public void Update()
    {
        Input.simulateMouseWithTouches = true; // HACK
        if (Input.GetMouseButton(0))
        {
            if (!isDragging)
            {
                draggingObject = GetObjectFromMouseRaycast();
                if (draggingObject)
                {
                    isDragging = true;
                }
            }
            else if (draggingObject != null)
            {
                var targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPosition.z = 0f;
                draggingObject.transform.position = targetPosition;
            }
        }
        else
        {
            isDragging = false;
        }
    }

    private GameObject GetObjectFromMouseRaycast()
    {
        GameObject gmObj = null;
        var collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (collider != null)
        {
            gmObj = collider.gameObject;
        }
        return gmObj;
    }
}