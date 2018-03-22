using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishController : MonoBehaviour
{
    public LayerMask jellyfishMask;
    public Rigidbody2D jellyfishBody;
    public Transform jellyfishTransform;
    public float speed;
    private float jellyfishWidth;

    void Start()
    {
        jellyfishTransform = this.transform;
        jellyfishBody = this.GetComponent<Rigidbody2D>();
        jellyfishWidth = this.GetComponent<SpriteRenderer>().bounds.extents.x;
    }

    void Update()
    {
        Vector2 lineCastPos = jellyfishTransform.position - jellyfishTransform.right * jellyfishWidth;
        Debug.DrawLine(lineCastPos, lineCastPos + Vector2.down);

        Vector2 jellyVel = jellyfishBody.velocity;
        jellyVel.x = speed;
        jellyfishBody.velocity = jellyVel;
    }
}
