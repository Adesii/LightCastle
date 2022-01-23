using System;
using UnityEngine;

[Serializable]
public class StatHolder
{
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float Strength = 1f;





    public int SoulAmount = 0;
    public int XP = 0;
    public int TotalXP;
    public int XPToNextLevel = 100;
    public int Level = 1;
    public int PermaXP = 0;




}

public static class StatsExtension
{
    public static void LevelUp(this StatHolder stats)
    {
        stats.XPToNextLevel = Mathf.RoundToInt(GameManager.Instance.XPCurve.Evaluate(stats.Level));
        stats.MaxHealth = Mathf.RoundToInt(GameManager.Instance.MaxHealthCurve.Evaluate(stats.Level));
        stats.Health = stats.MaxHealth;
        stats.Strength = Mathf.RoundToInt(GameManager.Instance.DamageCurve.Evaluate(stats.Level));
        stats.Level++;
    }
    public static void LevelDown(this StatHolder stats)
    {
        stats.XPToNextLevel = Mathf.RoundToInt(GameManager.Instance.XPCurve.Evaluate(stats.Level));
        stats.MaxHealth = Mathf.RoundToInt(GameManager.Instance.MaxHealthCurve.Evaluate(stats.Level));
        stats.Health = stats.MaxHealth;
        stats.Strength = Mathf.RoundToInt(GameManager.Instance.DamageCurve.Evaluate(stats.Level));
        stats.Level--;
    }

}

