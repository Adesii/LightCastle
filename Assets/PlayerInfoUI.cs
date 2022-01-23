using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    public StatHolder PlayerCharacter => GameManager.Instance.PlayerStats;
    public TextMeshProUGUI HealthText;
    public Image HealthBar;
    public TextMeshProUGUI XPText;
    public Image XpBar;

    public TextMeshProUGUI LevelText;
    // Start is called before the first frame update
    void Start()
    {
        XpBar.fillAmount = (float)PlayerCharacter.XP / PlayerCharacter.XPToNextLevel;
        HealthBar.fillAmount = (float)PlayerCharacter.Health / PlayerCharacter.MaxHealth;
        LevelText.text = PlayerCharacter.Level.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        HealthText.text = $"{PlayerCharacter.Health:0}/{PlayerCharacter.MaxHealth:0}";
        HealthBar.fillAmount = Mathf.Lerp(HealthBar.fillAmount, (float)PlayerCharacter.Health / PlayerCharacter.MaxHealth, Time.deltaTime * 10f);

        XPText.text = $"{PlayerCharacter.XP}/{PlayerCharacter.XPToNextLevel}";
        //lerp XpBar.fillAmount over time
        XpBar.fillAmount = Mathf.Lerp(XpBar.fillAmount, (float)PlayerCharacter.XP / PlayerCharacter.XPToNextLevel, Time.deltaTime * 10f);


        LevelText.text = PlayerCharacter.Level.ToString();



    }
}
