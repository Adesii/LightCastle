using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsViewManager : MonoBehaviour
{

    public StatHolder PlayerCharacter => GameManager.Instance.PlayerStats;
    public TextMeshProUGUI TotalXPText;
    public TextMeshProUGUI PermaXPText;
    public TextMeshProUGUI HighestAscention;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance == null || PlayerCharacter == null) return;
        TotalXPText.text = $"{PlayerCharacter.TotalXP:0}";
        PermaXPText.text = $"{PlayerCharacter.PermaXP:0}";
        HighestAscention.text = $"{PlayerCharacter.HighestAscention:0}";

    }
}
