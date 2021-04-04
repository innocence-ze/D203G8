using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TalkableEnemy : MonoBehaviour
{
    [ConditionalShow(true)] public EnemyDialogUI uiPanel;
 
    enum BubbleState
    {
        off,
        typing,
        show,
        waiting,
    };

    BubbleState state = BubbleState.off;

    //气泡中的内容
    public DialogData data;

    //气泡显示时间
    public float dialogTime;
    float dialogTimer;

    //气泡显示间隔
    public float minDialogInterval;
    public float maxDialogInterval;

    public float typingSpeed;
    float timerValue;
    int lastTimerValue = 0;
    string targetString;

    public StrEvent OnTyping;
    public SimpleEvent OnShow;
    public SimpleEvent OnOff;


    // Start is called before the first frame update
    void Start()
    {
        uiPanel = transform.GetComponentInChildren<EnemyDialogUI>();
        StartCoroutine(ChangeDialog());
        OnTyping.AddListener(ShowDialog);
    }

    // Update is called once per frame
    void Update()
    {
        if(state == BubbleState.off)
        {
            StartCoroutine(ChangeDialog());
        }
        if (state == BubbleState.typing)
        {
            UpdateContentString();
        }
        if(state == BubbleState.show)
        {
            dialogTimer += Time.deltaTime;
            if (dialogTimer >= dialogTime)
            {
                dialogTimer = 0;
                state = BubbleState.off;
                OnOff?.Invoke();
            }
        }
    }

    //更改内容
    IEnumerator ChangeDialog()
    {
        state = BubbleState.waiting;
        yield return new WaitForSeconds(Random.Range(minDialogInterval, maxDialogInterval));
        targetString = data.contents[Random.Range(0, data.contents.Count)].dialogText;
        state = BubbleState.typing;
        OnShow?.Invoke();
    }

    //显示到ui上
    void ShowDialog(string dialog) 
    {
        uiPanel.dialogContent.text = dialog;
    }

    //打字
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

        if (timer == targetString.Length)
        {
            state = BubbleState.show;
            timerValue = 0;
            return;
        }
    }
}
