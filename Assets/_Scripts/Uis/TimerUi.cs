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
    private Animator animator { get; set; }

    void Start()
    {
        tmp_sec = view.Get<TMP_Text>("tmp_sec");
        animator = view.GetComponent<Animator>();
        Game.MessagingManager.RegEvent(GameEvents.Stage_Timer_Update, bag => ApplyCountdown(bag.GetInt(0)));
    }

    private void ApplyCountdown(int time)
    {
        var isHurry = time <= 5;
        tmp_sec.text = time.ToString();
        var state = isHurry ? "Hurry" : "Countdown";
        // reset animator play index
        animator.Play(state, 0, 0);
    }
}
