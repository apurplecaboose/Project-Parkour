using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BoxPlayerMovement : MonoBehaviour
{
    public PlayerLookScript PlayerLookScript;
    public PlayerStats PlayerStats;

    //Varibles
    float playerHeight = 1f;
    private float SidewaysInput;
    private float VerticalInput;
    public bool isGrounded;
    public bool Jumpable;
    public bool OnLeftWall;
    public bool OnRightWall;
    public bool Sonic = false;

    float coyoteTime = 0.1f;
    float coyoteTimecounter;

    public TMP_Text TextfieldTMP;

    Vector3 VeloVec;
    float AvgVeloxz;
    float AvgVelo;


    //Raycasts
    public RaycastHit lefthitinfo;
    public RaycastHit righthitinfo;
    public RaycastHit GroundedRay;

    public Animator animate;
    public Rigidbody rb;
    
    private void Start()
    {
        rb.freezeRotation = true;
    }
    /**************************************/
    public void Update()
    {
        //lv failed 
        if (PlayerStats.LvlFailed)
        {
            PlayerStats.LvlFailed = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        TextfieldTMP.SetText(PlayerStats.CurrTime);
        
        SunshineRays();
        ControlDrag();
        JumpCheck();
        //velocity readout
        VeloVec = rb.velocity;
        AvgVeloxz = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(VeloVec.x), 2f) + Mathf.Pow(Mathf.Abs(VeloVec.z), 2f));
        AvgVelo = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(VeloVec.x), 2f) + Mathf.Pow(Mathf.Abs(VeloVec.y), 2f) + Mathf.Pow(Mathf.Abs(VeloVec.z), 2f));
        //print(VeloVec.y);
        //print(coyoteTimecounter);
        //respawn script
        if (transform.position.y < -10f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    void SunshineRays()
    {
        LayerMask GroundandWall = 1 << 7 | 1 << 6 | 1 << 4 ;

        OnLeftWall = Physics.Raycast(transform.position, -transform.right, out lefthitinfo , 2f, 1<<6);
        OnRightWall = Physics.Raycast(transform.position, transform.right, out righthitinfo, 2f, 1<<6);
        Jumpable = Physics.Raycast(transform.position - new Vector3(0, 0, 0.6f), Vector3.down, playerHeight / 2 + 0.25f, 1 << 8);
        if (!Sonic)
        {
            isGrounded = Physics.Raycast(transform.position - new Vector3(0, 0, 0.6f), Vector3.down, playerHeight / 2 + 0.25f, GroundandWall);
            Physics.Raycast(transform.position, Vector3.down, out GroundedRay, playerHeight / 2 + 5, GroundandWall);
        }
        else isGrounded = false;
    }
    void JumpCheck()
    {
        if (isGrounded | Jumpable)
        {
            coyoteTimecounter = coyoteTime;
        }
        else
        {
            coyoteTimecounter -= Time.deltaTime;
        }

        if(VeloVec.y > 10f)
        {
            PlayerStats.jumpforce = 0f;
        }
        else
        {
            PlayerStats.jumpforce = 30f;
        }
        if (Input.GetKeyDown(KeyCode.Space)| Input.GetKeyDown(KeyCode.Mouse0))// (Input.GetKey(KeyCode.Space) | Input.GetKey(KeyCode.Mouse0))
        {
            if (coyoteTimecounter > 0f)//(isGrounded | Jumpable)
            {
                rb.AddForce(Vector3.up * PlayerStats.jumpforce, ForceMode.Impulse);
                coyoteTimecounter = 0f;
            }

            if (Sonic)
            {
                if(OnLeftWall)
                    {
                        rb.AddForce(PlayerStats.WallJumpPercent * 30f * transform.up + lefthitinfo.normal * 30f * 1.75f, ForceMode.Impulse);
                        Sonic = false;
                    }
                else
                    {
                        rb.AddForce(PlayerStats.WallJumpPercent * 30f * transform.up + righthitinfo.normal * 30f * 1.75f, ForceMode.Impulse);
                        Sonic = false;
                    }
            
            }
        }
    }
    void ControlDrag()
    {
        if (isGrounded) rb.drag = PlayerStats.rbdrag;
        else rb.drag = PlayerStats.airdrag;
    }
    private void FixedUpdate()
    {
        MovePlayer();   
        Gravity();
    }
    void MovePlayer()
    {
        //rb.AddForce(transform.forward * PlayerStats.moveSpeed * PlayerStats.speedMultiplier, ForceMode.Acceleration); ///Constant forward force
        //rb.AddForce(transform.forward.normalized * (PlayerStats.moveSpeed * PlayerStats.speedMultiplier * 2f - AvgVelo), ForceMode.Acceleration);
        rb.AddForce(transform.forward.normalized * (100f - AvgVelo), ForceMode.Acceleration);
        

        SidewaysInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        if (SidewaysInput != 0)
        {
            rb.AddForce(PlayerStats.moveSpeed * PlayerStats.SideSpeed * PlayerStats.speedMultiplier * transform.forward + PlayerStats.moveSpeed * PlayerStats.SideSpeed * SidewaysInput * PlayerStats.speedMultiplier * transform.right, ForceMode.Acceleration);
        }
        /*
        //HandBrake ***NOT WORKING RN***
        if(VerticalInput<0)
        {
            //rb.AddForce(transform.forward * AvgVelo * -1f, ForceMode.Acceleration);
            rb.AddForce(-transform.forward.normalized * AvgVelo * 0.1f, ForceMode.Impulse);
        }
        */
    }
    void Gravity()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.down * PlayerStats.Grav * 0.5f *PlayerStats.speedMultiplier, ForceMode.Acceleration); // FLOOR GRAVITY
            Sonic = false;

        }
        else
        {
            if (OnRightWall && ((Input.GetAxisRaw("Horizontal")) > 0.75f))
            {
                rb.AddForce(-5f * PlayerStats.speedMultiplier * righthitinfo.normal, ForceMode.Acceleration);
                Sonic = true;

            }
            else if (OnLeftWall && ((Input.GetAxisRaw("Horizontal")) < -0.75f))
            {
                rb.AddForce(-5f * PlayerStats.speedMultiplier * lefthitinfo.normal, ForceMode.Acceleration);
                Sonic = true;

            }
            else
            {
                //Regular Gravity
                Sonic = false; // if: angle is looking up extra strong gravity function
                if(PlayerLookScript.xRot < 0f)
                {
                    rb.AddForce(Vector3.down * (Mathf.Pow(Mathf.Abs(PlayerLookScript.xRot), 1f) + 75f), ForceMode.Acceleration);
                }
                else
                {
                    rb.AddForce(Vector3.down * PlayerStats.Grav * PlayerStats.speedMultiplier, ForceMode.Acceleration); // else: normal grav
                }
            }
        }
    }

    private void LateUpdate()
    {
      if(Sonic)
      {
            if(OnLeftWall)
            {
               animate.SetBool("Wall Left POV", true);
               animate.SetBool("Wall Right POV", false);
            }
            else
            {
               animate.SetBool("Wall Right POV", true);
               animate.SetBool("Wall Left POV", false);
            }
      }
      else
      {
        animate.SetBool("Wall Right POV", false);
        animate.SetBool("Wall Left POV", false);
      }
    }
}
