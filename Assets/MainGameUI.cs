using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUI : MonoBehaviour
{
    public Image TransitionPanel;

    public TextMeshProUGUI LevelText;

    public TextMeshProUGUI StartText;

    public MiniMapManager MiniMap;

    internal async void StartUI()
    {
        if (!StatHolder.LoadPlayerStats().FirstTime)
        {
            StartText.DOFade(0, 0);
            TransitionPanel.DOFade(1, 0f);

            var idks = DOTween.Sequence();
            idks.Append(StartText.DOFade(0, 1));
            idks.Append(StartText.DOFade(1, 1));
            idks.Join(StartText.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1).SetEase(Ease.Linear));
            idks.Append(StartText.DOFade(0, 1));
            idks.Join(StartText.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 1).SetEase(Ease.OutQuad));
            await idks.Play().AsyncWaitForCompletion();
            await TransitionPanel.DOFade(0, 1).AsyncWaitForCompletion();
            GameManager.Instance.Player.UnlockPlayer();
            return;
        }

        StartText.DOFade(0, 0);
        TransitionPanel.DOFade(1, 0f);

        var idk = DOTween.Sequence();
        idk.Append(StartText.DOFade(0, 3));
        idk.Append(StartText.DOFade(1, 3));
        idk.Join(StartText.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 3).SetEase(Ease.Linear));
        idk.Append(StartText.DOFade(0, 5));
        idk.Join(StartText.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 5).SetEase(Ease.OutQuad));
        await idk.Play().AsyncWaitForCompletion();
        await TransitionPanel.DOFade(0, 3).AsyncWaitForCompletion();
        GameManager.Instance.Player.UnlockPlayer();
    }

    public async void faststart()
    {
        var idk = DOTween.Sequence();
        idk.Append(TransitionPanel.DOFade(1, 0f));
        idk.Append(TransitionPanel.DOFade(1, 2f));
        //idk.Append(LevelText.DOFade(0, 0f));
        //idk.Append(LevelText.DOFade(1, 2f));
        //idk.Append(LevelText.DOFade(0, 1f));
        idk.Append(TransitionPanel.DOFade(0, 2f));
        await idk.Play().AsyncWaitForCompletion();
        GameManager.Instance.Player.UnlockPlayer();
    }

    internal void ClearDict()
    {
        MiniMap.clearDict();
    }
}
