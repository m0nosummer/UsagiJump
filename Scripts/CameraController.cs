using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        transform.position = new Vector3(desiredPosition.x, transform.position.y, transform.position.z);
    }
}