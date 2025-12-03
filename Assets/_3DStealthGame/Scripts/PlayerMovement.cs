using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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
    private bool playerFreeze = false;
    private int spaceBarPressed = 0;

    // stamina stuff
    private int stamina = 200;
    private bool isSprinting = false;
    public Image circleImage;

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
        if (Random.Range(0,600)==0)
        {
            playerFreeze = true;
            walkSpeed = 0f;
            turnSpeed = 0f;
            spaceBarPressed = 0;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            spaceBarPressed++;
        }

        if (spaceBarPressed > 5)
        {
            playerFreeze = false;
            walkSpeed = 1.0f;
            turnSpeed = 20f;
        }


        // stamina stuff major mod 
        //check for shift
        if (Input.GetKeyDown(KeyCode.LeftShift) && stamina > 50 && !playerFreeze)
        {
            isSprinting = true;
        }

        //check for sprint end early
        if (Input.GetKeyUp(KeyCode.LeftShift) && !playerFreeze)
        {
            walkSpeed = 1.0f;
            isSprinting = false;
        }
        //sprinting loop, drain stam check for low stam
        if (isSprinting == true && !playerFreeze)
        {
            walkSpeed = 2f;
            stamina--;
            if (stamina < 0)
            {
                isSprinting = false;
                walkSpeed = 1.0f;
            }
        }
        //regen stam if not sprinting
        if (isSprinting == false && stamina < 200 && !playerFreeze)
        {
            stamina++;
        }

        //ui circle stuff
        //find circl scale based off curent stam
        float scale = Mathf.Clamp(stamina, 0.1f, 200f);
        scale = scale / 200;
        //scale circle
        circleImage.rectTransform.localScale = new Vector3(scale, scale, 1);

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