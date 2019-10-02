using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 startPosition;

    private Rigidbody2D rb;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("begin");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("on");

        // TODO, HACK
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("end");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startPosition = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        var position2d = new Vector2(transform.position.x, transform.position.y);
        var v = startPosition - position2d;
        var force = v * v * Time.deltaTime;

        //Debug.Log(force);

        rb.AddForce(force);
    }
}
