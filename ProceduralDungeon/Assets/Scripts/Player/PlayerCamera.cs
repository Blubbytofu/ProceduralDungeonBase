using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private LayerMask envMask;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform orientation;

    private float xInput;
    private float yInput;
    private float xRot;
    private float yRot;
    [SerializeField] private float xSens;
    [SerializeField] private float ySens;
    [SerializeField] private float sensMultiplier;
    [Range(0f, 90f)] [SerializeField] private float absLookLimit;
    [SerializeField] private float interactRange;

    private float initialY;
    RaycastHit hit;

    public void SetInitialRot(float y)
    {
        initialY = y;
    }

    private void Start()
    {
        LockCursor(true);
    }

    private void Update()
    {
        Look();
        Interact();
    }

    private void LateUpdate()
    {
        transform.position = cameraPosition.position;
    }

    private void LockCursor(bool state)
    {
        Cursor.visible = !state;
        if (state)
        {
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }
        Cursor.lockState = CursorLockMode.None;
    }

    private void Look()
    {
        xInput = Input.GetAxisRaw("Mouse X");
        yInput = Input.GetAxisRaw("Mouse Y");

        xRot += xInput * ySens * sensMultiplier;
        yRot -= yInput * xSens * sensMultiplier;

        yRot = Mathf.Clamp(yRot, -absLookLimit, absLookLimit);

        transform.rotation = Quaternion.Euler(yRot, initialY + xRot, 0);
        orientation.rotation = Quaternion.Euler(0, initialY + xRot, 0);
    }

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange, envMask))
            {
                // do something
            }
        }
    }
}
