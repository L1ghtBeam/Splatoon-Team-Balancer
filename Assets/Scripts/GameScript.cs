using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public LeaderboardScript leaderboardScript;
    public GameObject alphaTemplate;
    public GameObject bravoTemplate;
    public GameObject alphaChangeText;
    public GameObject bravoChangeText;

    void OnEnable()
    {
        UpdateGame();
    }

    void UpdateGame()
    {
        // Delete old text objects
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Temp");
        for (int i = 0; i < objs.Length; i++)
        {
            Destroy(objs[i]);
        }

        float largestPosY = 1500;

        // Make new text objects for alpha
        for (int i = 0; i < leaderboardScript.alpha.Count; i++)
        {
            // Find location
            Vector3 pos = new Vector3(alphaTemplate.transform.localPosition.x, alphaTemplate.transform.localPosition.y + i * -200, alphaTemplate.transform.localPosition.z);

            if (pos.y < largestPosY)
            {
                largestPosY = pos.y;
            }

            // Make Object
            GameObject playerText = Instantiate(alphaTemplate, pos, alphaTemplate.transform.rotation);
            playerText.transform.SetParent(gameObject.transform, false);
            playerText.tag = "Temp";

            // Change Text
            string text1 = leaderboardScript.alpha[i].Name;
            playerText.GetComponent<TextMeshProUGUI>().text = text1;

            playerText.transform.SetSiblingIndex(0);
            playerText.SetActive(true);
        }

        // Make new text objects for bravo
        for (int i = 0; i < leaderboardScript.bravo.Count; i++)
        {
            // Find location
            Vector3 pos = new Vector3(bravoTemplate.transform.localPosition.x, bravoTemplate.transform.localPosition.y + i * -200, bravoTemplate.transform.localPosition.z);

            if (pos.y < largestPosY)
            {
                largestPosY = pos.y;
            }

            // Make Object
            GameObject playerText = Instantiate(bravoTemplate, pos, bravoTemplate.transform.rotation);
            playerText.transform.SetParent(gameObject.transform, false);
            playerText.tag = "Temp";

            // Change Text
            string text1 = leaderboardScript.bravo[i].Name;
            playerText.GetComponent<TextMeshProUGUI>().text = text1;

            playerText.transform.SetSiblingIndex(0);
            playerText.SetActive(true);
        }

        // Find scores of teams
        float alphaScore = 0;
        float bravoScore = 0;
        int alphaPlayers = 0;
        int bravoPlayers = 0;

        for (int i = 0; i < leaderboardScript.alpha.Count; i++)
        {
            alphaScore += leaderboardScript.alpha[i].Score;
            alphaPlayers++;
        }
        for (int i = 0; i < leaderboardScript.bravo.Count; i++)
        {
            bravoScore += leaderboardScript.bravo[i].Score;
            bravoPlayers++;
        }
        if (bravoPlayers > alphaPlayers) alphaPlayers = bravoPlayers;
        alphaScore = Mathf.Round((alphaScore / alphaPlayers) * 10) / 10;
        bravoScore = Mathf.Round((bravoScore / alphaPlayers) * 10) / 10;

        GameObject scoreText = GameObject.Find("Score Difference");
        GameObject vsText = GameObject.Find("VS Text");
        GameObject mapText = GameObject.Find("Map Text");
        GameObject mapObject = GameObject.Find("Map");
        GameObject mapImage = GameObject.Find("Map Image");
        GameObject modeText = GameObject.Find("Mode Text");
        vsText.transform.localPosition = new Vector3(vsText.transform.localPosition.x, largestPosY - 100, vsText.transform.localPosition.z);

        scoreText.transform.localPosition = new Vector3(scoreText.transform.localPosition.x, largestPosY - 200, scoreText.transform.localPosition.z);
        scoreText.GetComponent<TextMeshProUGUI>().text = alphaScore + " - " + bravoScore;

        alphaChangeText.transform.localPosition = new Vector3(alphaChangeText.transform.localPosition.x, largestPosY - 250, alphaChangeText.transform.localPosition.z);
        bravoChangeText.transform.localPosition = new Vector3(bravoChangeText.transform.localPosition.x, largestPosY - 250, bravoChangeText.transform.localPosition.z);

        // Get Map Data
        int map = 0;
        string mode = "";
        if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Monday)
        {
            map = MasterScript.rainMaps[Random.Range(0, 8)];
            mode = "Rainmaker";
        }
        else if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Tuesday)
        {
            map = MasterScript.rainMaps[Random.Range(0, 8)];
            mode = "Splat Zones";
        }
        else if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Wednesday)
        {
            map = MasterScript.clamMaps[Random.Range(0, 8)];
            mode = "Clam Blitz";
        }
        else if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Thursday)
        {
            map = MasterScript.towerMaps[Random.Range(0, 8)];
            mode = "Tower Control";
        }
        else
        {
            map = Random.Range(0, MasterScript.mapData.Count);
            float rndMode = Random.Range(0, 10);
            if (rndMode <= 2) mode = "Rainmaker";
            if (rndMode >= 3 && rndMode <= 5) mode = "Splat Zones";
            if (rndMode >= 6 && rndMode <= 8) mode = "Tower Control";
            if (rndMode >= 9) mode = "Clam Blitz";
        }

        // Place text under scores list
        modeText.transform.localPosition = new Vector3(mapText.transform.localPosition.x, largestPosY - 300, mapText.transform.localPosition.z);
        modeText.GetComponent<TextMeshProUGUI>().text = mode;

        mapText.transform.localPosition = new Vector3(mapText.transform.localPosition.x, largestPosY - 400, mapText.transform.localPosition.z);
        mapText.GetComponent<TextMeshProUGUI>().text = MasterScript.mapData[map].Name;

        mapObject.transform.localPosition = new Vector3(mapText.transform.localPosition.x, largestPosY - 800, mapText.transform.localPosition.z);
        mapImage.GetComponent<Image>().sprite = MasterScript.mapData[map].Image;
    }

    public void AlphaWon()
    {
        // Calculate match rewards
        float alphaChange = MasterScript.CalculateMatchRewards(leaderboardScript.alpha, leaderboardScript.bravo, true, MasterScript.matchPower);
        float bravoChange = -alphaChange;

        // Fix for uneven teams
        alphaChange *= 4;
        alphaChange /= leaderboardScript.alpha.Count;
        bravoChange *= 4;
        bravoChange /= leaderboardScript.alpha.Count;

        // Apply to text
        alphaChangeText.GetComponent<TextMeshProUGUI>().text = "+" + Mathf.Round(alphaChange);
        alphaChangeText.SetActive(true);
        bravoChangeText.GetComponent<TextMeshProUGUI>().text = "" + Mathf.Round(bravoChange);
        bravoChangeText.SetActive(true);

        // Update Scores
        UpdateScores(alphaChange, bravoChange, true);
    }

    public void BravoWon()
    {
        float alphaChange = MasterScript.CalculateMatchRewards(leaderboardScript.alpha, leaderboardScript.bravo, false, MasterScript.matchPower);
        float bravoChange = -alphaChange;
        alphaChangeText.GetComponent<TextMeshProUGUI>().text = "" + Mathf.Round(alphaChange);
        alphaChangeText.SetActive(true);
        bravoChangeText.GetComponent<TextMeshProUGUI>().text = "+" + Mathf.Round(bravoChange);
        bravoChangeText.SetActive(true);

        // Update Scores
        UpdateScores(alphaChange, bravoChange, false);
    }

    public void UpdateScores(float alphaChange, float bravoChange, bool alphaWon)
    {
        // Apply score changes
        for (int i = 0; i < leaderboardScript.alpha.Count; i++)
        {
            leaderboardScript.alpha[i].Score += alphaChange;
            if (alphaWon)
            {
                leaderboardScript.alpha[i].Wins++;
            }
            else
            {
                leaderboardScript.alpha[i].Losses++;
            }
        }
        for (int i = 0; i < leaderboardScript.bravo.Count; i++)
        {
            leaderboardScript.bravo[i].Score += bravoChange;
            if (alphaWon)
            {
                leaderboardScript.bravo[i].Losses++;
            }
            else
            {
                leaderboardScript.bravo[i].Wins++;
            }
        }

        // Add to master list
        MasterScript.playerData.Clear();
        for (int i = 0; i < leaderboardScript.alpha.Count; i++)
        {
            MasterScript.playerData.Add(leaderboardScript.alpha[i]);
        }
        for (int i = 0; i < leaderboardScript.bravo.Count; i++)
        {
            MasterScript.playerData.Add(leaderboardScript.bravo[i]);
        }
        for (int i = 0; i < leaderboardScript.unactivePlayers.Count; i++)
        {
            MasterScript.playerData.Add(leaderboardScript.unactivePlayers[i]);
        }
        MasterScript.playerData = MasterScript.SortList(MasterScript.playerData);

        MasterScript.SaveData();
    }
}