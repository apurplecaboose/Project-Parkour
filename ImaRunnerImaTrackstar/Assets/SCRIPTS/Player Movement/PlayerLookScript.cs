using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerLookScript : MonoBehaviour
{
    public PlayerStats PlayerStats;
    BoxPlayerMovement BoolCheck;
    float mouseX;
    float mouseY;

    //float multiplier = 0.1f;
    float multiplier = 0.05f;

    public float xRot;
    //public float yRot; //Changed to a function fo PlayerStats scriptable object for easy access for Checkpoint script.
    float zRot;


    float Megatron;
    float WallAngle;
    float Dot;
    
    float yChange;

    float FloorCheckY;
    float GroundMagnitudeY;
    float GroundAngleY;
    float FloorCheckZ;
    float GroundMagnitudeZ;
    float GroundAngleZ;
    private void Awake()
    {
        BoolCheck = GetComponent<BoxPlayerMovement>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        MouseInput();
        PlayerStats.LookSens = Mathf.Clamp(PlayerStats.LookSens, 0f, 100f);
    }
    private void FixedUpdate()
    {     
        if (BoolCheck.isGrounded)
        {
            //SLOPE CHECK
            FloorCheckY = Vector3.Dot(BoolCheck.GroundedRay.normal, transform.forward);
            GroundMagnitudeY = Vector3.Magnitude(BoolCheck.GroundedRay.normal) * Vector3.Magnitude(transform.forward);
            GroundAngleY = Mathf.Acos(FloorCheckY / GroundMagnitudeY) * Mathf.Rad2Deg;

            FloorCheckZ = Vector3.Dot(BoolCheck.GroundedRay.normal, transform.right);
            GroundMagnitudeZ = Vector3.Magnitude(BoolCheck.GroundedRay.normal) * Vector3.Magnitude(transform.right);
            GroundAngleZ = Mathf.Acos(FloorCheckZ / GroundMagnitudeZ) * Mathf.Rad2Deg;

            if (Mathf.Abs(FloorCheckY) > 0.01)
            {
                if (GroundAngleY > 90f)
                {
                    xRot -= 0.01f * Mathf.Pow(Mathf.Abs(GroundAngleY - 90), 1.95f);
                }
                else
                {
                    xRot += 0.01f * Mathf.Pow(Mathf.Abs(GroundAngleY - 90), 1.95f);
                }
            }
            if (Mathf.Abs(FloorCheckZ) > 0.01)
            {
                if (GroundAngleZ > 90f)
                {
                    zRot += 0.01f * Mathf.Pow(Mathf.Abs(GroundAngleZ - 90), 1.95f);
                }
                else
                {
                    zRot -= 0.01f * Mathf.Pow(Mathf.Abs(GroundAngleZ - 90), 1.95f);
                }
            }
        }

        //Camera Movement
        if (BoolCheck.Sonic)
        {
            WallAngleCompensation();
            transform.rotation = Quaternion.Euler(xRot, PlayerStats.yRot, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(xRot, PlayerStats.yRot, zRot);
        }
    }
    void LateUpdate()
    {
        switch (PlayerStats.LookType)
        {
            case 1:
                NormalLook();
                break;
            case 2:
                LeftRightReversed();
                break;
            case 3:
                UpDownReversed();
                break;
            case 4:
                EverythingReversed();
                break;
        }
    }
    void MouseInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
    }    
    void NormalLook()
    {
        if (BoolCheck.Sonic)
        {
            xRot -= mouseY * PlayerStats.LookSens * multiplier;
            xRot = Mathf.Clamp(xRot, -60f, 60f);
        }
        else if (!BoolCheck.isGrounded)
        {
            xRot -= mouseY * PlayerStats.LookSens * multiplier;
            xRot = Mathf.Clamp(xRot, -35f, 40f);
            PlayerStats.yRot += mouseX * PlayerStats.LookSens * multiplier;
        }
        else
        {
            PlayerStats.yRot += mouseX * PlayerStats.LookSens * multiplier;
        }
    }
    void LeftRightReversed()
    {
        if (BoolCheck.Sonic)
        {
            xRot -= mouseY * PlayerStats.LookSens * multiplier;
            xRot = Mathf.Clamp(xRot, -60f, 60f);
        }
        else if (!BoolCheck.isGrounded)
        {
            xRot -= mouseY * PlayerStats.LookSens * multiplier;
            xRot = Mathf.Clamp(xRot, -35f, 40f);
            PlayerStats.yRot -= mouseX * PlayerStats.LookSens * multiplier;

        }
        else
        {
            PlayerStats.yRot -= mouseX * PlayerStats.LookSens * multiplier;
        }
    }
    void UpDownReversed()
    {
        if (BoolCheck.Sonic)
        {
            xRot += mouseY * PlayerStats.LookSens * multiplier;
            xRot = Mathf.Clamp(xRot, -60f, 60f);
        }
        else if (!BoolCheck.isGrounded)
        {
            xRot += mouseY * PlayerStats.LookSens * multiplier;
            xRot = Mathf.Clamp(xRot, -35f, 40);
            PlayerStats.yRot += mouseX * PlayerStats.LookSens * multiplier;
        }
        else
        {
            PlayerStats.yRot += mouseX * PlayerStats.LookSens * multiplier;
        }
    }
    void EverythingReversed()
    {
        if (BoolCheck.Sonic)
        {
            xRot += mouseY * PlayerStats.LookSens * multiplier;
            xRot = Mathf.Clamp(xRot, -60f, 60f);
        }
        else if (!BoolCheck.isGrounded)
        {
            xRot += mouseY * PlayerStats.LookSens * multiplier;
            xRot = Mathf.Clamp(xRot, -35f, 40f);
            PlayerStats.yRot -= mouseX * PlayerStats.LookSens * multiplier;
        }
        else
        {
            PlayerStats.yRot -= mouseX * PlayerStats.LookSens * multiplier;
        }
    }
    void WallAngleCompensation()
    {
        if (BoolCheck.OnLeftWall)
        {
            Dot = Vector3.Dot(BoolCheck.lefthitinfo.normal, transform.forward);
            Megatron = Vector3.Magnitude(BoolCheck.lefthitinfo.normal) * Vector3.Magnitude(transform.forward);
            WallAngle = Mathf.Acos(Dot / Megatron) * Mathf.Rad2Deg;

            if (Mathf.Abs(Dot) > 0.01f)
            {
                if (WallAngle > 90f)
                {
                    yChange = 0.025f * Mathf.Pow(Mathf.Abs(WallAngle - 90), 2.2f);
                    yChange = Mathf.Clamp(yChange, 0f, 30f);
                    PlayerStats.yRot += yChange;
                }
                else
                {
                    yChange = 0.025f * Mathf.Pow(Mathf.Abs(WallAngle - 90), 2.2f);
                    yChange = Mathf.Clamp(yChange, 0f, 25f);
                    PlayerStats.yRot -= yChange;
                }
            }
        }
        else if (BoolCheck.OnRightWall)
        {
            Dot = Vector3.Dot(BoolCheck.righthitinfo.normal, transform.forward);
            Megatron = Vector3.Magnitude(BoolCheck.righthitinfo.normal) * Vector3.Magnitude(transform.forward);
            WallAngle = Mathf.Acos(Dot / Megatron) * Mathf.Rad2Deg;

            if (Mathf.Abs(Dot) > 0.01f)
            {
                if (WallAngle > 90f)
                {
                    yChange = 0.025f * Mathf.Pow(Mathf.Abs(WallAngle - 90), 2.2f);
                    yChange = Mathf.Clamp(yChange, 0f, 30f);
                    PlayerStats.yRot -= yChange;
                }
                else
                {
                    yChange = 0.025f * Mathf.Pow(Mathf.Abs(WallAngle - 90), 2.2f);
                    yChange = Mathf.Clamp(yChange, 0f, 30f);
                    PlayerStats.yRot += yChange;
                }
            }
        }
    }
} 