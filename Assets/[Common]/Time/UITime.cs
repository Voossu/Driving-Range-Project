using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITime : Singleton<UITime>
{

    #region Members

    [ReadOnly] [SerializeField] private float timerValue;
    [ReadOnly] [SerializeField] private bool timeMove = true;
    [ReadOnly] [SerializeField] private float timeScale = 1;
    public float stepValue = 0.2f;

    [Header("GUI")]
    [SerializeField] private TextMeshProUGUI UIClockLabel;
    [SerializeField] private TextMeshProUGUI UISpeedLabel;
    [SerializeField] private Image UIPlayIcon;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite stopSprite;

    #endregion

    #region Actions

    protected override void Awake()
    {
        base.Awake();
        timeScale = Time.timeScale;
    }

    #endregion

    #region Members

    Coroutine routine = null;

    [Button]
    public void TimerPlay()
    {
        if (routine == null)
        {
            routine = StartCoroutine(Clock());
        }
    }

    [Button]
    public void TimerStop()
    {
        StopCoroutine(routine);
        if (UIClockLabel)
        {
            UIClockLabel.text = "00:00";
        }
    }

    private IEnumerator Clock()
    {
        while (true)
        {
            timerValue += 1;
            if (UIClockLabel)
            {
                UIClockLabel.text = Mathf.RoundToInt(timerValue / 60).ToString("00") + ":" + (timerValue % 60).ToString("00");
            }
            yield return new WaitForSeconds(1);
        }
    }

    [Button]
    public void TimeBrake()
    {
        Time.timeScale = 1.0f;
        timeScale = 1.0f;
        UIUpdate();
    }

    [Button]
    public void TimeSlow()
    {
        if (Time.timeScale - stepValue > 0)
        {
            Time.timeScale -= stepValue;
            timeScale = Time.timeScale;
        }
        UIUpdate();
    }

    [Button]
    public void TimeFast()
    {
        Time.timeScale += stepValue;
        timeScale = Time.timeScale;
        UIUpdate();
    }

    [Button]
    public void TimePlay()
    {
        Time.timeScale = timeMove ? 0 : timeScale;
        timeMove = !timeMove;
        UIUpdate();
    }

    private void UIUpdate()
    {
        if (UISpeedLabel) UISpeedLabel.text = Time.timeScale.ToString("F1") + "x";
        if (UIPlayIcon) UIPlayIcon.sprite = Time.timeScale > 0 ? playSprite : stopSprite;
    }

    #endregion


}
