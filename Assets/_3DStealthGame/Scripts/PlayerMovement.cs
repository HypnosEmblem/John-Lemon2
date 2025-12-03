using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;


public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;
    public InputAction MoveAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;
    public bool playerFreeze = false;
    public int unFreeze = 0;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        MoveAction.Enable();
        m_Animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        m_Rigidbody.MoveRotation(m_Rotation);
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime);

        if (Random.Range(0, 1000) == 0)
        {
            walkSpeed = 0;
            turnSpeed = 0;
            playerFreeze = true;
            unFreeze = 0;
        }
        if (unFreeze == 5)
        {
            walkSpeed = 1.0f;
            turnSpeed = 20f;
            playerFreeze = false;
            unFreeze = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            unFreeze++;
        }
    }

}