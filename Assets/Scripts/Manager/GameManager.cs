using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(DialogManager))]
[RequireComponent(typeof(RoomInfo))]
public class GameManager : MonoBehaviour
{
    public static GameManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType<GameManager>();
            }
            if (singleton == null)
            {
                Debug.LogError("Cannot find Game Manager");
            }
            return singleton;
        }
    }
    private static GameManager singleton = null;


    public PlayerCharacter pc;


    private InputManager ioMgr;
    ICommand jump, stopJump, dash, left, right, stopHor, up, down, stopVer, attack, interact;

    public UIManager uiMgr;

    public DialogManager dialogMgr;

    public RoomInfo room;

    // Start is called before the first frame update
    void Awake()
    {
        pc = FindObjectOfType<PlayerCharacter>();
        InitInputManager();
        InitDialogManager();

        InitRoomInfo();

        pc.SetData();
        if (room.roomEntranceDic.ContainsKey(RoomInfo.lastRoom))
            pc.transform.position = room.roomEntranceDic[RoomInfo.lastRoom].transform.position;
        

        InitUIManager();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputManager();
    }

    void InitInputManager()
    {
        ioMgr = GetComponent<InputManager>();
        jump = new JumpCommand(pc);
        stopJump = new StopJumpCommand(pc);
        dash = new DashCommand(pc);
        left = new MoveLeftCommand(pc);
        right = new MoveRightCommand(pc);
        stopHor = new StopMoveHorCommand(pc);
        up = new MoveUpCommand(pc);
        down = new MoveDownCommand(pc);
        stopVer = new StopMoveVerCommand(pc);
        attack = new AttackCommand(pc);
        interact = new InteractCommand(pc);
    }

    void UpdateInputManager()
    {
        var h = Input.GetAxis(ioMgr.horName);
        var v = Input.GetAxis(ioMgr.verName);

        if (h > 0.05f) ioMgr.AddCommand(right);
        else if (h < -0.05f) ioMgr.AddCommand(left);
        else ioMgr.AddCommand(stopHor);

        if (v > 0.05f) ioMgr.AddCommand(up);
        else if (v < -0.05f) ioMgr.AddCommand(down);
        else ioMgr.AddCommand(stopVer);

        if (Input.GetAxis(ioMgr.jumpName) > 0)
        {
            if (!pc.JumpCommand)
            {
                ioMgr.AddCommand(jump);
            }
        }
        else
        {
            if (pc.JumpCommand)
            {
                ioMgr.AddCommand(stopJump);
            }
        }

        if (Input.GetButtonDown(ioMgr.dashName))
        {
            ioMgr.AddCommand(dash);
        }
        if (Input.GetButtonDown(ioMgr.attackName))
        {
            ioMgr.AddCommand(attack);
        }
        if (Input.GetButtonDown(ioMgr.interactName))
        {
            ioMgr.AddCommand(interact);
        }
        ioMgr.ExecuteCommands();
    }

    void InitUIManager()
    {
        uiMgr = FindObjectOfType<UIManager>();
        uiMgr.canvas = uiMgr.transform;
        uiMgr.Init();
        uiMgr.ShowPanel<HealthPanel>();
    }

    void InitDialogManager()
    {
        dialogMgr = GetComponent<DialogManager>();
    }

    void InitRoomInfo()
    {
        room = GetComponent<RoomInfo>();
        room.InitRoomInfo();
    }
}
