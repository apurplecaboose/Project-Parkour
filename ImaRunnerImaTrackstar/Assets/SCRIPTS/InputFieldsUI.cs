using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldsUI : MonoBehaviour
{
    public PlayerStats PlayerStats;

    public TMP_InputField StartingLookSens;
    public TMP_InputField StartingmoveSpeed;
    public TMP_InputField StartingSideSpeed;
    public TMP_InputField Startingrbdrag;
    public TMP_InputField Startingairdrag;
    public TMP_InputField Startingjumpforce;
    public TMP_InputField StartingStartingGrav;
    public TMP_InputField StartingWallJumpPercent;

    public void SensitivityValue(string Sensitivity)
    {
        PlayerStats.LookSens = float.Parse(Sensitivity);
    }
    public void MoveEdit(string msValue)
    {
        PlayerStats.moveSpeed = float.Parse(msValue);
    }
    public void Drag1Edit(string Drag1Val)
    {
        PlayerStats.rbdrag = float.Parse(Drag1Val);
    }
    public void Drag2Edit(string Drag2Val)
    {
        PlayerStats.airdrag = float.Parse(Drag2Val);
    }
    public void GravEdit(string GravVal)
    {
        PlayerStats.Grav = float.Parse(GravVal);
    }
    public void JumpEdit(string jumpVal)
    {
        PlayerStats.jumpforce = float.Parse(jumpVal);
    }
    public void WallJumpPercentEdit(string WallJumpVal)
    {
        PlayerStats.WallJumpPercent = float.Parse(WallJumpVal);
    }
    public void SideSpeedPercentEdit(string SideSpeedVal)
    {
        PlayerStats.SideSpeed = float.Parse(SideSpeedVal);
    }


    private void Start()
    {
        StartingmoveSpeed.text = PlayerStats.moveSpeed.ToString("0.0");
        Startingrbdrag.text = PlayerStats.rbdrag.ToString("0.0");
        Startingairdrag.text = PlayerStats.airdrag.ToString("0.0");
        StartingStartingGrav.text = PlayerStats.Grav.ToString("0.0");
        StartingLookSens.text = PlayerStats.LookSens.ToString("0.00");
        Startingjumpforce.text = PlayerStats.jumpforce.ToString("0.00");

        StartingWallJumpPercent.text = PlayerStats.WallJumpPercent.ToString("0.00");
        StartingSideSpeed.text = PlayerStats.SideSpeed.ToString("0.00");
    }
}
