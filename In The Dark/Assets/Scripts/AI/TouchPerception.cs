using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPerception : MonoBehaviour
{
    // Event that is called when the touch perception of an option is triggered
    public delegate void OnObjectPerceptionUpdated(GameObject detectedObject, float side);
    public OnObjectPerceptionUpdated OnPerceptionUpdated;

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject touchedObject = collision.gameObject;
        if (gameObject.isStatic)
        {
            return;
        }

        Rigidbody2D rigidBody = touchedObject.GetComponent<Rigidbody2D>();
        if (!rigidBody)
        {
            return;
        }

        // Find which side we were touched
        float side = (touchedObject.transform.position.x - transform.position.x);

        // Face the direction we were touched TODO: Should really make this a helper function
        if (side > 0f)
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        else
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);

        if (OnPerceptionUpdated != null)
            OnPerceptionUpdated.Invoke(touchedObject, Mathf.Sign(side));
    }
}
