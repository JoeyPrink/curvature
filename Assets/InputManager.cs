using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    bool isDragging = false;
    GameObject draggingObject;

    public event Action<GameObject> OnGrabAttractor = delegate { };
    public event Action<GameObject> OnReleaseAttractor = delegate { };

    private bool disabled = false;

    public void Update()
    {
        Input.simulateMouseWithTouches = true; // HACK
        if (Input.GetMouseButton(0) && !disabled)
        {
            if (!isDragging)
            {
                draggingObject = GetObjectFromMouseRaycast();
                if (draggingObject)
                {
                    OnGrabAttractor(draggingObject);
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
            if (isDragging)
            {
                OnReleaseAttractor(draggingObject);
            }
            isDragging = false;
        }
    }

    internal void Enable()
    {
        disabled = false;
    }

    // disables the grab input and releases the currently dragged attractor, if any
    public void Disable()
    {
        disabled = true;
    }

    private GameObject GetObjectFromMouseRaycast()
    {
        GameObject gmObj = null;
        var collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (collider != null && collider.GetComponent<Attractor>() != null)
        {
            gmObj = collider.gameObject;
        }
        return gmObj;
    }
}