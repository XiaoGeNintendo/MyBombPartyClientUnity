using System;
using UnityEngine;

public class ArrangeInRing : MonoBehaviour
{
    public float radius = 100f; // Radius of the ring

    void Start()
    {
        ArrangeChildrenInRing();
    }

    private void OnTransformChildrenChanged()
    {
        ArrangeChildrenInRing();
    }

    void ArrangeChildrenInRing()
    {
        // Calculate the angle between each item
        float angleStep = 360f / transform.childCount;

        // Loop through each child
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // Calculate the angle for this item
            float angle = i * angleStep;

            // Convert the angle to radians
            float radian = angle * Mathf.Deg2Rad;

            // Calculate the x and y position
            float x = Mathf.Cos(radian) * radius;
            float y = Mathf.Sin(radian) * radius;

            // Set the position of the child
            child.localPosition = new Vector3(x, y, 0f);
        }
    }
}