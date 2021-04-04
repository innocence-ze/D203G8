using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public enum State
    {
        off,
        typing,
        pause,
    }

    [ConditionalShow(true)]public DialogData data;
    [ConditionalShow(true)]public DialogPanel ui;

    int currentLine;
    string targetString;

    public float typingSpeed;
    float timerValue;
    int lastTimerValue = 0;

    public float dialogShowTime;
    WaitForSeconds showTimer;

    public State state;

    public StrEvent OnTyping;
    public SimpleEvent OnNextSentence;
    public SimpleEvent OnOff;

    bool bStartPause = false;
    bool isShowing = false;  //是否展示过,用于关闭

    // Start is called before the first frame update
    void Start()
    {
        state = State.off;
        showTimer = new WaitForSeconds(dialogShowTime);
    }

    private void Update()
    {
        if(state == State.typing)
        {
            UpdateContentString();
        }
        if(state == State.pause && !bStartPause)
        {
            StartCoroutine(DialogShowCoroutine());
        }
        if(state == State.off && isShowing)
        {
            Debug.Log("off");
            OnOff?.Invoke();
            isShowing = false;
        }
    }

    IEnumerator DialogShowCoroutine()
    {
        bStartPause = true;
        yield return showTimer;
        bStartPause = false;
        if (NextSentence())
        {
            state = State.typing;
        }
        else
        {
            state = State.off;
        }
    }

    public void StartDialog(DialogData dlgData, int index = 0)
    {
        isShowing = true;
        data = dlgData;
        currentLine = index;
        targetString = data.contents[index].dialogText;
        ui = GameManager.Singleton.uiMgr.GetPanel<DialogPanel>();
        if(ui == null)
        {
            ui = GameManager.Singleton.uiMgr.Open<DialogPanel>();
        }
        state = State.typing;
    }

    void UpdateContentString()
    {
        timerValue += Time.deltaTime * typingSpeed;
        int timer = Mathf.Min(Mathf.FloorToInt(timerValue), targetString.Length);

        if (timer != lastTimerValue)
        {
            lastTimerValue = timer;
            var tempString = targetString.Substring(0, timer);
            OnTyping?.Invoke(tempString);
        }

        if(timer == targetString.Length)
        {
            timerValue = 0;
            lastTimerValue = 0;
            state = State.pause;
        }
    }

    bool NextSentence()
    {
        if(currentLine < data.contents.Count-1)
        {
            currentLine++;
            targetString = data.contents[currentLine].dialogText;
            return true;
        }
        return false;
    }
}
