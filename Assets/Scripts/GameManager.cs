using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Game Manager", menuName = "Singletons/GameManager")]
public class GameManager : yaSingleton.Singleton<GameManager>
{
    public PlayerCharacter Player;

    public StatHolder PlayerStats;


    protected override void Initialize()
    {
        base.Initialize();
        Player = FindObjectOfType<PlayerCharacter>();

    }
}
