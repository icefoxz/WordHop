using System.Collections;
using System.Collections.Generic;
using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine;

public class TimerUi : MonoBehaviour
{
    [SerializeField]private View view;
    private TMP_Text tmp_sec { get; set; }

    void Start()
    {
        tmp_sec = view.Get<TMP_Text>("tmp_sec");
        Game.MessagingManager.RegEvent(GameEvents.Stage_Timer_Update, bag => tmp_sec.text = bag.GetString(0));
    }
}
