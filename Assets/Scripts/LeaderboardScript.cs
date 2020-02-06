using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour
{
    public GameObject samplePlayerText;

    public List<PlayerData> alpha = new List<PlayerData>();
    public List<PlayerData> bravo = new List<PlayerData>();
    public List<PlayerData> unactivePlayers = new List<PlayerData>();

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        UpdateLeaderboard();
    }

    private void OnEnable()
    {
        UpdateLeaderboard();
    }

    private void Update()
    {
        // Dont let start button be pressed with <2 players
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Toggle");
        int readyPlayers = 0;
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].GetComponent<Toggle>().isOn)
            {
                readyPlayers++;
            }
        }
        if (readyPlayers >= 2 && readyPlayers <= 8)
        {
            GameObject.Find("Start Button").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("Start Button").GetComponent<Button>().interactable = false;
        }
    }

    public void UpdateLeaderboard()
    {
        // Delete old text objects
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Temp");
        for (int i = 0; i < objs.Length; i++)
        {
            Destroy(objs[i]);
        }
        
        // Make new text objects
        for (int i = 0; i < MasterScript.playerData.Count; i++)
        {
            // Find location
            RectTransform rect = samplePlayerText.GetComponent<RectTransform>();
            Vector2 pos = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + (i * -150));

            // Make Object
            GameObject playerText = Instantiate(samplePlayerText, pos, samplePlayerText.transform.rotation);
            
            playerText.transform.SetParent(gameObject.transform.Find("Players").transform, false);
            playerText.tag = "Temp";

            // Change Text
            string text1 = MasterScript.playerData[i].Name;
            string text2 = Mathf.Round(MasterScript.playerData[i].Score * 10) / 10 + "";
            playerText.transform.Find("Sample Name").GetComponent<TextMeshProUGUI>().text = text1;
            playerText.transform.Find("Sample Score").GetComponent<TextMeshProUGUI>().text = text2;
            playerText.transform.Find("Seperator").GetComponent<TextMeshProUGUI>().text = "(" + MasterScript.playerData[i].Wins + "-" + MasterScript.playerData[i].Losses + ")";

            playerText.transform.Find("Toggle").name = i.ToString();

            playerText.SetActive(true);
        }
    }

    public void MakeTeams()
    {
        List<PlayerData> activePlayers = new List<PlayerData>();
        alpha.Clear();
        bravo.Clear();
        unactivePlayers.Clear();

        // Seperate selected and unselected players
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Toggle");
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].GetComponent<Toggle>().isOn)
            {
                activePlayers.Add(MasterScript.playerData[i]);
            }
            else
            {
                unactivePlayers.Add(MasterScript.playerData[i]);
            }
        }

        // Balance teams
        MasterScript.BalanceTeams(activePlayers, out alpha, out bravo);
    }
}