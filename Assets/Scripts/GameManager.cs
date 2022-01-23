using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using Lean.Pool;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Manager", menuName = "Singletons/GameManager")]
public partial class GameManager : yaSingleton.Singleton<GameManager>
{
    public PlayerCharacter Player;

    public StatHolder PlayerStats;

    public GameObject SoulDropPrefab;

    public AnimationCurve XPCurve;
    public AnimationCurve MaxHealthCurve;
    public AnimationCurve DamageCurve;
    public List<GameObject> SmallObjects;
    public List<GameObject> MediumObjects;
    public List<GameObject> LargeObjects;


    protected override void Initialize()
    {
        base.Initialize();
        Player = FindObjectOfType<PlayerCharacter>();


        PlayerStats = new StatHolder(); //TODO Replace with Permanent stats

        GenerateDungeon();
    }



    public void AddXP(int amount)
    {
        PlayerStats.XP += amount;
        PlayerStats.TotalXP += amount;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (PlayerStats.XP >= PlayerStats.XPToNextLevel)
        {
            PlayerStats.XP -= PlayerStats.XPToNextLevel;
            PlayerStats.LevelUp();
        }
        else if (PlayerStats.XP < 0)
        {
            PlayerStats.XP += PlayerStats.XPToNextLevel;
            PlayerStats.LevelDown();

        }
    }

    public void DropSouls(int AmountToDrop, Enemy by)
    {
        var randomAmount = Random.Range(2, 4);
        int[] dropAmounts = RandomParts(AmountToDrop, randomAmount);
        int max = dropAmounts.Max();
        foreach (var item in dropAmounts)
        {

            var dropAmount = item;
            Debug.Log($"Drop Amount: {dropAmount}");


            var soulDrop = LeanPool.Spawn(SoulDropPrefab, by.transform.position, Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0));
            var sould = soulDrop.GetComponent<SoulDrop>();
            sould.DropAmount = dropAmount; //TODO:Replace this with Proper Drop Calculations
            sould.DropDirection = Random.insideUnitSphere;
            sould.DropDirection.y = 0;

            var dropscale = Mathf.Lerp(0.75f, 2f, dropAmount / max);

            soulDrop.transform.localScale = new Vector3(dropscale, dropscale, dropscale);
        }
    }

    //split Amount into random sized parts
    //so that the Sum of all ints is M
    public int[] RandomParts(int Amount, int parts)
    {
        int[] splits = new int[parts];
        int sum = 0;
        for (int i = 0; i < splits.Length; i++)
        {
            splits[i] = Random.Range(1, Amount);
            sum += splits[i];
            Amount -= splits[i];
        }
        for (int i = 0; i < splits.Length; i++)
        {
            splits[i] += Amount / splits.Length;
        }
        splits[0] += Amount % splits.Length;
        return splits;
    }

    internal void GameOver()
    {
        Debug.Log("Game Over");
    }
}
