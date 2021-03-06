﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField]
    private NewGameManager GameManager;
    //[SerializeField]
    //private NewCoinController Coin;


    public CharacterController controller;
    public Transform cam;

    public float gravity = -9.81f;
    public float jumpHeight = 3;
    bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float speed = 5f;
    public float secondJumpSpeed = 0.6f;
    public float belokSmootingTime = 0.1f;

    float turnSmoothVelocity;

    Vector3 velocity,arah;

    public AudioSource pickedSound;

    // Update is called once per frame
    void Update()
    {
        MovementHandeler();
       
    }

    private void MovementHandeler()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        arah = new Vector3(hAxis, 0f, vAxis).normalized;

        if (arah.magnitude >= 0.1f)
        {
            float radiusPutar = Mathf.Atan2(arah.x, arah.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float radiusSmooth = Mathf.SmoothDampAngle(transform.eulerAngles.y, radiusPutar, ref turnSmoothVelocity, belokSmootingTime);
            transform.rotation = Quaternion.Euler(0f, radiusSmooth, 0f);

            Vector3 arahGerak = Quaternion.Euler(0f, radiusPutar, 0f) * Vector3.forward;
            controller.Move(arahGerak.normalized * speed * Time.deltaTime);
            controller.Move(velocity * Time.deltaTime);
        }

        //jump
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        //gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider trigger)
    {
        // Triggered by touch Coin
        if (trigger.gameObject.tag == "Coin")
        {
            Debug.Log("Yey Coin");
            GameManager.IncreceScoring(1);
            GameManager.RefreshScoreUI();
            pickedSound.Play();

            Destroy(trigger.gameObject);

        }
        else
        // Triggered by touch the Enemy
        if (trigger.gameObject.tag == "Enemy")
        {
            //Game Over
            GameManager.GameOver();
        } else
        // Triggered By touching the Goal
        if(trigger.gameObject.tag == "Goal")
        {
            GameManager.IncreseLevel();
            GameManager.Reset();
        }

    }

}
