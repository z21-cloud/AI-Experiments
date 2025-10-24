using System;
using UnityEngine;

public class PlayerConrtoller : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float lookSensitivity = 2f;
    
    private float xRotation = 0f;
    private Vector2 move;
    private Vector2 look;

    private void Update()
    {
        GetInput();
        Movement();
        CameraTurn();
    }

    private void GetInput()
    {
        move = InputManager.Instance.MoveInput;
        look = InputManager.Instance.LookInput;
    }

    private void Movement()
    {
        Vector3 moveDirection = transform.forward * move.y + transform.right * move.x;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

   private void CameraTurn()
   {
        transform.Rotate(Vector3.up * look.x * lookSensitivity);
        xRotation -= look.y * lookSensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
   }
}
