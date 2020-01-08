using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    public TMP_InputField balanceIncrement;
    public TMP_InputField matchPower;
    public TMP_InputField resetData;
    public Button resetButton;
    public TMP_InputField addPlayer;

    public void UpdateResetButton()
    {
        if (resetData.text == "RESET")
        {
            resetButton.interactable = true;
        }
        else
        {
            resetButton.interactable = false;
        }
    }

    public void AddPlayer()
    {
        string inputName = addPlayer.text;
        MasterScript.playerData.Add(new PlayerData { Name = inputName, Score = 1500 });
        MasterScript.SaveData();
    }

    private void OnEnable()
    {
        transform.SetSiblingIndex(100);
        balanceIncrement.text = MasterScript.balanceIncrement.ToString();
        matchPower.text = MasterScript.matchPower.ToString();
        addPlayer.text = "";
    }

    public void ResetData()
    {
        resetData.text = "";
        UpdateResetButton();
        MasterScript.ResetData();
    }

    public void UpdateBalanceIncrement(string text)
    {
        MasterScript.balanceIncrement = float.Parse(text);
        MasterScript.SaveData();
    }

    public void UpdateMatchPower(string text)
    {
        MasterScript.matchPower = float.Parse(text);
        MasterScript.SaveData();
    }
}