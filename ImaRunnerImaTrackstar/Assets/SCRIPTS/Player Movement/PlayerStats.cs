using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[CreateAssetMenu (menuName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public int LookType = 1;
    public float LookSens = 10f;
    public float moveSpeed = 8f;
    public float speedMultiplier = 10f;
    public float SideSpeed = 0.75f;
    public float rbdrag = 5f;
    public float airdrag = 2f;
    public float jumpforce = 30f;
    public float Grav = 5f;
    public float WallJumpPercent = 0.75f;


    public bool CheckPointBool = false;
    public Vector3 ChkTransform;
}
