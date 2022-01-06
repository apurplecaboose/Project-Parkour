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
    public float moveSpeed = 7f;
    public float speedMultiplier = 10f;
    public float SideSpeed = 0.5f;
    public float rbdrag = 4f;
    public float airdrag = 2f;
    public float jumpforce = 30f;
    public float Grav = 5f;
    public float WallJumpPercent = 0.75f;


    public int CheckPointNumber = 0;
    public Vector3 ChkTransform;
}
