using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class StatHolder
{
    [JsonIgnore]
    public float Health = 100f;
    [JsonIgnore]
    public float MaxHealth = 100f;
    [JsonIgnore]
    public float Strength = 1f;
    [JsonIgnore]
    public int XP = 0;
    [JsonIgnore]
    public int XPInRun = 0;
    [JsonIgnore]
    public int XPToNextLevel = 100;
    [JsonIgnore]
    public int Level = 1;

    [JsonProperty]
    public int TotalXP { get; set; }
    [JsonProperty]
    public int PermaXP { get; set; } = 0;
    [JsonProperty]
    public bool FirstTime { get; set; } = true;
    [JsonProperty]
    public int HighestAscention { get; set; } = 0;


    [JsonIgnore]
    public int CurrentAscention = 0;

    internal static StatHolder LoadPlayerStats()
    {
        if (PlayerPrefs.HasKey("PlayerStats"))
        {
            var stats = JsonConvert.DeserializeObject<StatHolder>(PlayerPrefs.GetString("PlayerStats"));
            stats.XP = stats.PermaXP;

            return stats;
        }
        else
        {
            return new StatHolder();
        }
    }

}

public static class StatsExtension
{
    public static void LevelUp(this StatHolder stats)
    {
        stats.Level++;
        stats.XPToNextLevel = Mathf.RoundToInt(GameManager.Instance.XPCurve.Evaluate(stats.Level - 1));
        stats.MaxHealth = Mathf.RoundToInt(GameManager.Instance.MaxHealthCurve.Evaluate(stats.Level - 1));
        stats.Health = stats.MaxHealth;
        stats.Strength = Mathf.RoundToInt(GameManager.Instance.DamageCurve.Evaluate(stats.Level - 1));
    }
    public static void LevelDown(this StatHolder stats)
    {
        stats.Level--;
        stats.XPToNextLevel = Mathf.RoundToInt(GameManager.Instance.XPCurve.Evaluate(stats.Level - 1));
        stats.MaxHealth = Mathf.RoundToInt(GameManager.Instance.MaxHealthCurve.Evaluate(stats.Level - 1));
        stats.Health = stats.MaxHealth;
        stats.Strength = Mathf.RoundToInt(GameManager.Instance.DamageCurve.Evaluate(stats.Level - 1));
    }

    public static void RefreshStats(this StatHolder stats, GameManager gm)
    {
        stats.XPToNextLevel = Mathf.RoundToInt(gm.XPCurve.Evaluate(stats.Level - 1));
        stats.MaxHealth = Mathf.RoundToInt(gm.MaxHealthCurve.Evaluate(stats.Level - 1));
        stats.Health = stats.MaxHealth;
        stats.Strength = Mathf.RoundToInt(gm.DamageCurve.Evaluate(stats.Level - 1));
    }

    public static void SavePlayerStats(this StatHolder stats)
    {
        PlayerPrefs.SetString("PlayerStats", JsonConvert.SerializeObject(stats));

        Debug.Log("Saved Player Stats");
        Debug.Log(JsonConvert.SerializeObject(stats));
    }

}

