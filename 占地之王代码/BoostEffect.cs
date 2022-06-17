using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostEffect : MonoBehaviour
{
    private Vector2 Forward;
    private Quaternion rotation;
    // Start is called before the first frame update
    private void Start()
    {
        rotation = transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        Forward = transform.parent.GetComponent<BasicPlayer>().Forward;
        float angle1 = Vector2.SignedAngle(Vector2.right, Forward);
        if (angle1 > -45 && angle1 <= 45) Forward = Vector2.right;
        else if (angle1 > 45 && angle1 <= 135) Forward = Vector2.up;
        else if (angle1 > 135 || angle1 <= -135) Forward = Vector2.left;
        else if (angle1 > -135 && angle1 <= -45) Forward = Vector2.down;
        float angle = Vector2.SignedAngle(transform.parent.right, Forward);
        transform.rotation = rotation* Quaternion.Euler(0, 0, angle);
    }
}
