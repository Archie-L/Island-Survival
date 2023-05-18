using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    bool swimming;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Inventory")]
    public GameObject[] invMenu;
    public GameObject craftMenu;
    public PlayerCam camScript;

    public bool craftOpen = false;

    public bool invOpen = false;
    public bool chestOpen = false;

    public InventoryManager manager;

    [Header("Stats")]
    public int Health;
    public int maxHealth;
    public int Hunger;
    public int maxHunger;
    public int Water;
    public int maxWater;

    private float nextActionTime = 0.0f;
    public float period = 1f;

    public bool hungry;
    public bool thirsty;


    private Slider hpBar;
    private Slider hungerBar;
    private Slider thirstBar;

    private void Start()
    {
        hpBar = GameObject.FindGameObjectWithTag("hpBar").GetComponent<Slider>();
        hungerBar = GameObject.FindGameObjectWithTag("hungerBar").GetComponent<Slider>();
        thirstBar = GameObject.FindGameObjectWithTag("thirstBar").GetComponent<Slider>();

        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    public Transform chest;

    private void Update()
    {
        Transform camTransform = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 6))
        {
            if (hit.collider.tag == ("Chest") && Input.GetKeyDown(KeyCode.E))
            {
                chest = hit.transform.GetChild(0);
                manager.currentChest = chest.transform.GetChild(0);
                manager.SetChestIDs();
                chest.gameObject.SetActive(true);
                chestOpen = true;
                OpenInv();
            }
        }

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        if (!invOpen)
        {
            MyInput();
            SpeedControl();
        }

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!invOpen)
            {
                OpenInv();
            }
            if (invOpen)
            {
                CloseInv();
            }
        }

        craftMenu = GameObject.FindGameObjectWithTag("craftMenu");

        if (craftMenu == enabled)
        {
            craftOpen = true;
        }
        else
        {
            craftOpen = false;
        }


        hpBar.value = Health;
        hungerBar.value = Hunger;
        thirstBar.value = Water;

        if (Time.time > nextActionTime)
        {
            nextActionTime += period;

            if (!thirsty || !hungry)
            {
                Hunger -= 1;
                Water -= 1;
            }
            if(thirsty || hungry)
            {
                Health -= 5;
            }
        }

        if(Water <= 0)
        {
            thirsty = true;
        }
        if (Hunger <= 0)
        {
            hungry = true;
        }
        if (Water > 0)
        {
            thirsty = false;
        }
        if (Hunger > 0)
        {
            hungry = false;
        }

        if(Water > maxWater)
        {
            Water = maxWater;
        }
        if (Hunger > maxHunger)
        {
            Hunger = maxHunger;
        }
        if (Health > maxHealth)
        {
            Health = maxHealth;
        }
    }

    private void FixedUpdate()
    {
        if (!invOpen)
        {
            MovePlayer();
        }
    }

    void HungerManager()
    {
        
    }

    void WaterManager()
    {

    }

    public void OpenInv()
    {
        for (int i = 0; i < invMenu.Length; i++)
        {
            invMenu[i].SetActive(true);
        }
        camScript.GetComponent<PlayerCam>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(Open());

        Debug.Log("pressed");
    }
    IEnumerator Open()
    {
        yield return new WaitForEndOfFrame();
        invOpen = true;
    }

    void CloseInv()
    {
        if (craftOpen)
        {
            craftMenu.SetActive(false);
        }
        if (chestOpen)
        {
            chestOpen = false;
            chest.gameObject.SetActive(false);
            manager.RemoveChest();

            for (int i = 0; i < invMenu.Length; i++)
            {
                invMenu[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < invMenu.Length; i++)
            {
                invMenu[i].SetActive(false);
            }
        }
        camScript.GetComponent<PlayerCam>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(Close());

        Debug.Log("pressed2");
    }
    IEnumerator Close()
    {
        yield return new WaitForEndOfFrame();
        invOpen = false;
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded || Input.GetKey(jumpKey) && readyToJump && swimming)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Water")
        {
            swimming = true;
            jumpForce = 1;
            Physics.gravity = new Vector3(0, 0f, 0);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
        {
            swimming = false;
            jumpForce = 8;
            Physics.gravity = new Vector3(0, -9.81F, 0);
        }
    }
}