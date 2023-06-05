using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

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

    public Animator anim;

    [Header("Inventory")]
    public GameObject[] invMenu;
    public GameObject craftMenu;
    public PlayerCam camScript;
    public GameObject escapeMenu;

    public bool craftOpen = false;

    public bool invOpen = false;
    public bool escMenu = false;
    public bool chestOpen = false;

    public InventoryManager manager;

    [Header("Stats")]
    public int Health;
    public int maxHealth;
    public int Hunger;
    public int maxHunger;
    public int Water;
    public int maxWater;
    public int oxygen;
    public int maxOxygen;

    private float nextActionTime = 0.0f;
    public float period = 1f;

    public bool hungry;
    public bool thirsty;
    public bool drowning;

    private Slider hpBar;
    private Slider hungerBar;
    private Slider thirstBar;
    private Slider o2Bar;

    private AudioSource ambience;
    private AudioSource sfx;

    private void Start()
    {
        hpBar = GameObject.FindGameObjectWithTag("hpBar").GetComponent<Slider>();
        hungerBar = GameObject.FindGameObjectWithTag("hungerBar").GetComponent<Slider>();
        thirstBar = GameObject.FindGameObjectWithTag("thirstBar").GetComponent<Slider>();
        o2Bar = GameObject.FindGameObjectWithTag("o2Bar").GetComponent<Slider>();
        defaultGravity = Physics.gravity;

        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        ambience = GameObject.FindGameObjectWithTag("ambience").GetComponent<AudioSource>();
        sfx = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();

        ambience.clip = land;
        ambience.Play(0);

        readyToJump = true;
        anim.enabled = false;
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
                if (hit.collider.GetComponent<timer>() != null)
                {
                    if (hit.collider.GetComponent<timer>().TimerFinished == false)
                    {
                        hit.collider.GetComponent<timer>().StartTimer();
                    }
                    else
                    {
                        chest = hit.transform.GetChild(0);
                        manager.currentChest = chest.transform.GetChild(0);
                        manager.SetChestIDs();
                        chest.gameObject.SetActive(true);
                        chestOpen = true;
                        OpenInv();
                    }
                }
                else
                {
                    chest = hit.transform.GetChild(0);
                    manager.currentChest = chest.transform.GetChild(0);
                    manager.SetChestIDs();
                    chest.gameObject.SetActive(true);
                    chestOpen = true;
                    OpenInv();
                }
            }
        }

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        if (!invOpen || !escMenu)
        {
            MyInput();
            SpeedControl();
        }

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!escMenu)
            {
                escapeMenu.SetActive(true);
                camScript.GetComponent<PlayerCam>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                StartCoroutine(escOpen());
            }
            if (escMenu)
            {
                CloseEscapeMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !escMenu)
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

        if (swimming)
        {
            jumpForce = 2f;
            o2Bar.transform.parent.gameObject.SetActive(true);
        }
        if (!swimming)
        {
            jumpForce = 8f;
            o2Bar.value = o2Bar.maxValue;
            o2Bar.transform.parent.gameObject.SetActive(false);
        }

        hpBar.value = Health;
        hungerBar.value = Hunger;
        thirstBar.value = Water;
        o2Bar.value = oxygen;

        if (Time.time > nextActionTime)
        {
            nextActionTime += period;

            if (!thirsty || !hungry)
            {
                Hunger -= 1;
                Water -= 1;
            }
            if (swimming)
            {
                oxygen -= 1;
            }
            if (!swimming)
            {
                oxygen = maxOxygen;
            }
            if (thirsty || hungry || drowning)
            {
                Health -= 1;
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
        if (oxygen <= 0)
        {
            drowning = true;
        }
        if (Water > 0)
        {
            thirsty = false;
        }
        if (Hunger > 0)
        {
            hungry = false;
        }
        if (oxygen > 0)
        {
            drowning = false;
        }

        if (Water > maxWater)
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
        if(Health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    void Die()
    {
        anim.enabled = true;
        anim.SetTrigger("die");
        Invoke("resetGame", 10f);
    }

    void resetGame()
    {
        SceneManager.LoadScene("Menu");
    }

    public void CloseEscapeMenu()
    {
        escapeMenu.SetActive(false);
        camScript.GetComponent<PlayerCam>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(escClose());
    }

    IEnumerator escOpen()
    {
        yield return new WaitForEndOfFrame();
        escMenu = true;
    }

    IEnumerator escClose()
    {
        yield return new WaitForEndOfFrame();
        escMenu = false;
    }

    private void FixedUpdate()
    {
        if (!invOpen)
        {
            MovePlayer();
        }
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
        if (Input.GetKey(KeyCode.LeftControl) && readyToJump && swimming)
        {
            readyToJump = false;

            ReverseJump();

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
    private void ReverseJump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(-transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    public bool isInWater = false;
    public bool isInLab = false;
    private Vector3 defaultGravity;

    public GameObject entranceTrigger;
    public GameObject exitTrigger;

    public GameObject waterVolume;

    public AudioClip land, underwater, labs, waterSplash;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;
            ActivateWaterEffects();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
            if (!isInLab)
            {
                DeactivateWaterEffects();
            }
        }

        if (other.CompareTag("labsEntry"))
        {
            isInLab = true;
            entranceTrigger.SetActive(false);
            exitTrigger.SetActive(true);
            ActivateLabEffects();
        }

        if (other.CompareTag("labsExit"))
        {
            isInLab = false;
            entranceTrigger.SetActive(true);
            exitTrigger.SetActive(false);
            DeactivateLabEffects();
        }
    }

    private void ActivateWaterEffects()
    {
        Debug.Log("Water effects activated");
        Physics.gravity = Vector3.zero;
        ambience.clip = underwater;
        ambience.Play(0);
        sfx.clip = waterSplash;
        sfx.Play(0);
        swimming = true;
    }

    private void DeactivateWaterEffects()
    {
        Debug.Log("Water effects deactivated");
        Physics.gravity = defaultGravity;
        ambience.clip = land;
        ambience.Play(0);
        sfx.clip = waterSplash;
        sfx.Play(0);
        swimming = false;
    }

    private void ActivateLabEffects()
    {
        Debug.Log("Lab effects activated");
        Physics.gravity = defaultGravity;
        waterVolume.GetComponent<PostProcessVolume>().enabled = false;
        ambience.clip = labs;
        ambience.Play(0);
        sfx.clip = waterSplash;
        sfx.Play(0);
        swimming = false;
    }

    private void DeactivateLabEffects()
    {
        Debug.Log("Lab effects deactivated");
        Physics.gravity = Vector3.zero;
        waterVolume.GetComponent<PostProcessVolume>().enabled = true;
        ambience.clip = underwater;
        ambience.Play(0);
        sfx.clip = waterSplash;
        sfx.Play(0);
        swimming = true;
    }
}