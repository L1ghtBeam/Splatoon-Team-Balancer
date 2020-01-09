using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MasterScript : MonoBehaviour
{
    public static List<PlayerData> playerData = new List<PlayerData>();
    public static List<MapData> mapData = new List<MapData>();
    public static List<int> rainMaps = new List<int>();
    public static List<int> zoneMaps = new List<int>();
    public static List<int> towerMaps = new List<int>();
    public static List<int> clamMaps = new List<int>();

    public static float balanceIncrement;
    public static float matchPower;

    void Start()
    {
        LoadData();
        playerData = SortList(playerData);
        CreateMapList();
    }

    public static List<PlayerData> SortList(List<PlayerData> playerData)
    {
        List<PlayerData> newPlayerData = new List<PlayerData>();

        // For every item in list
        for (int i = 0; i < playerData.Count; i++)
        {
            float playerScore = playerData[i].Score;
            bool complete = false;

            // Compare with every item in new list
            for (int j = 0; j < newPlayerData.Count; j++)
            {
                float newPlayerScore = newPlayerData[j].Score;

                // If higher than current compared score, replace it
                if (playerScore > newPlayerScore && !complete)
                {
                    newPlayerData.Insert(j, playerData[i]);
                    complete = true;
                }
            }

            // If lowest then add to bottom
            if (!complete)
            {
                newPlayerData.Add(playerData[i]);
            }
        }

        // Return new list
        return newPlayerData;
    }

    public static List<PlayerData> ShuffleList(List<PlayerData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            PlayerData temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    public static void PrintList(List<PlayerData> playerData)
    {
        for (int i = 0; i < playerData.Count; i++)
        {
            print(playerData[i].Name + " - " + playerData[i].Score + " (" + playerData[i].Wins + "-" + playerData[i].Losses + ")");
        }
    }

    public static void BalanceTeams(List<PlayerData> playerData, out List<PlayerData> alpha, out List<PlayerData> bravo)
    {
        alpha = new List<PlayerData>();
        bravo = new List<PlayerData>();

        bool balanced = false;
        float scoreDifference = 0;
        float allowedScoreDifference = balanceIncrement;

        while (!balanced) {
            alpha = new List<PlayerData>();
            bravo = new List<PlayerData>();
            float alphaScore = 0;
            float bravoScore = 0;
            playerData = ShuffleList(playerData);

            for (int i = 0; i < playerData.Count; i++)
            {
                if ((alphaScore <= bravoScore && alpha.Count < 4) || bravo.Count >= 4)
                {
                    alpha.Add(playerData[i]);
                    alphaScore += playerData[i].Score;
                }
                else
                {
                    bravo.Add(playerData[i]);
                    bravoScore += playerData[i].Score;
                }
            }
            scoreDifference = Mathf.Abs(alphaScore - bravoScore);
            if (scoreDifference <= allowedScoreDifference)
            {
                balanced = true;
            }
            else
            {
                allowedScoreDifference += balanceIncrement;
            }
        }
        print("Team balance finished with " + scoreDifference + " score difference");
    }

    public static float CalculateMatchRewards(List<PlayerData> alpha, List<PlayerData> bravo, bool alphaWon, float maxReward)
    {
        float alphaScore = 0;
        float bravoScore = 0;

        // Get alpha score
        for (int i = 0; i < alpha.Count; i++)
        {
            alphaScore += alpha[i].Score;
        }
        // Get bravo score
        for (int i = 0; i < bravo.Count; i++)
        {
            bravoScore += bravo[i].Score;
        }
        // Find count of smallest team
        int minPlayers = alpha.Count;
        if (bravo.Count < minPlayers) minPlayers = bravo.Count;
        // Find count of biggest team
        int maxPlayers = alpha.Count;
        if (bravo.Count > maxPlayers) maxPlayers = bravo.Count;

        // Get average of scores
        alphaScore /= minPlayers;
        bravoScore /= minPlayers;

        // Predict outcome
        float alphaChance = 1 / (1 + Mathf.Pow(10, (bravoScore - alphaScore) / (400 * maxPlayers)));
        Debug.Log("Alpha had a " + alphaChance * 100 + "% change of winning");

        if (alphaWon)
        {
            return maxReward * (1 - alphaChance);
        }
        else
        {
            return maxReward * (0 - alphaChance);
        }
    }

    public static void SaveData()
    {
        PlayerPrefs.SetInt("count", playerData.Count);
        for (int i = 0; i < playerData.Count; i++)
        {
            PlayerPrefs.SetString("name " + i, playerData[i].Name);
            PlayerPrefs.SetFloat("score " + i, playerData[i].Score);
            PlayerPrefs.SetInt("wins " + i, playerData[i].Wins);
            PlayerPrefs.SetInt("losses " + i, playerData[i].Losses);
        }
        PlayerPrefs.SetFloat("balanceIncrement", balanceIncrement);
        PlayerPrefs.SetFloat("matchPower", matchPower);

        PlayerPrefs.Save();
    }

    public static void LoadData()
    {
        playerData.Clear();
        for (int i = 0; i < PlayerPrefs.GetInt("count", 0); i++)
        {
            string name = PlayerPrefs.GetString("name " + i, "?????");
            float score = PlayerPrefs.GetFloat("score " + i, 1500f);
            int wins = PlayerPrefs.GetInt("wins " + i, 0);
            int losses = PlayerPrefs.GetInt("losses " + i, 0);
            playerData.Add(new PlayerData { Name = name, Score = score, Wins = wins, Losses = losses });
        }

        balanceIncrement = PlayerPrefs.GetFloat("balanceIncrement", 25f);
        matchPower = PlayerPrefs.GetFloat("matchPower", 50f);
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();

        playerData.Clear();
        /*
        playerData.Add(new PlayerData { Name = "«Beam»", Score = 1500f });
        playerData.Add(new PlayerData { Name = "«chiken»", Score = 1500f });
        playerData.Add(new PlayerData { Name = "«T4tbear»", Score = 1500f });
        playerData.Add(new PlayerData { Name = "Chris", Score = 1500f });
        playerData.Add(new PlayerData { Name = "FoundIT", Score = 1500f });
        playerData.Add(new PlayerData { Name = "«Beak»", Score = 1500f });
        playerData.Add(new PlayerData { Name = "Psyk_200", Score = 1500f });
        playerData.Add(new PlayerData { Name = "Titanium_R", Score = 1500f });
        playerData.Add(new PlayerData { Name = "Hopper", Score = 1500f });
        */

        balanceIncrement = 25f;
        matchPower = 50f;

        SaveData();
    }

    void CreateMapList()
    {
        mapData.Clear();
        rainMaps.Clear();
        zoneMaps.Clear();
        towerMaps.Clear();
        clamMaps.Clear();

        mapData.Add(new MapData { Name = "The Reef", Image = Resources.Load<Sprite>("Sprites/S2_Stage_The_Reef") });
        mapData.Add(new MapData { Name = "Musselforge Fitness", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Musselforge_Fitness") });
        mapData.Add(new MapData { Name = "Starfish Mainstage", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Starfish_Mainstage") });
        mapData.Add(new MapData { Name = "Humpback Pump Track", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Humpback_Pump_Track") });
        mapData.Add(new MapData { Name = "Inkblot Art Academy", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Inkblot_Art_Academy") });
        mapData.Add(new MapData { Name = "Sturgeon Shipyard", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Sturgeon_Shipyard") });
        mapData.Add(new MapData { Name = "Moray Towers", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Moray_Towers") });
        mapData.Add(new MapData { Name = "Port Mackerel", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Port_Mackerel") });
        mapData.Add(new MapData { Name = "Manta Maria", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Manta_Maria") });
        mapData.Add(new MapData { Name = "Kelp Dome", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Kelp_Dome") });
        mapData.Add(new MapData { Name = "Snapper Canal", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Snapper_Canal") });
        mapData.Add(new MapData { Name = "Blackbelly Skatepark", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Blackbelly_Skatepark") });
        mapData.Add(new MapData { Name = "MakoMart", Image = Resources.Load<Sprite>("Sprites/S2_Stage_MakoMart") });
        mapData.Add(new MapData { Name = "Walleye Warehouse", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Walleye_Warehouse") });
        mapData.Add(new MapData { Name = "Shellendorf Institute", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Shellendorf_Institute") });
        mapData.Add(new MapData { Name = "Arowana Mall", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Arowana_Mall") });
        mapData.Add(new MapData { Name = "Goby Arena", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Goby_Arena") });
        mapData.Add(new MapData { Name = "Piranha Pit", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Piranha_Pit") });
        mapData.Add(new MapData { Name = "Camp Triggerfish", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Camp_Triggerfish") });
        mapData.Add(new MapData { Name = "Wahoo World", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Wahoo_World") });
        mapData.Add(new MapData { Name = "New Albacore Hotel", Image = Resources.Load<Sprite>("Sprites/S2_Stage_New_Albacore_Hotel") });
        mapData.Add(new MapData { Name = "Ancho-V Games", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Ancho-V_Games") });
        mapData.Add(new MapData { Name = "Skipper Pavilion", Image = Resources.Load<Sprite>("Sprites/S2_Stage_Skipper_Pavilion") });

        rainMaps.Add(0);
        rainMaps.Add(1);
        rainMaps.Add(3);
        rainMaps.Add(13);
        rainMaps.Add(17);
        rainMaps.Add(18);
        rainMaps.Add(20);
        rainMaps.Add(21);

        zoneMaps.Add(0);
        zoneMaps.Add(4);
        zoneMaps.Add(5);
        zoneMaps.Add(8);
        zoneMaps.Add(11);
        zoneMaps.Add(12);
        zoneMaps.Add(14);
        zoneMaps.Add(15);

        towerMaps.Add(1);
        towerMaps.Add(4);
        towerMaps.Add(6);
        towerMaps.Add(8);
        towerMaps.Add(10);
        towerMaps.Add(16);
        towerMaps.Add(19);
        towerMaps.Add(22);

        clamMaps.Add(2);
        clamMaps.Add(3);
        clamMaps.Add(7);
        clamMaps.Add(9);
        clamMaps.Add(12);
        clamMaps.Add(17);
        clamMaps.Add(21);
        clamMaps.Add(22);
    }
}

public class PlayerData
{
    public string Name { get; set; }
    public float Score { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
}

public class MapData
{
    public string Name { get; set; }
    public Sprite Image { get; set; }
}