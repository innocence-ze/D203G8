using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
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


    private InputManager ioM;
    ICommand jump, stopJump, dash, left, right, stopHor, up, down, stopVer, attack;

    // Start is called before the first frame update
    void Start()
    {
        pc = FindObjectOfType<PlayerCharacter>();
        InitInputManager();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputManager();
    }

    void InitInputManager()
    {
        ioM = GetComponent<InputManager>();
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
    }

    void UpdateInputManager()
    {
        var h = Input.GetAxis(ioM.horName);
        var v = Input.GetAxis(ioM.verName);

        if (h > 0.05f) ioM.AddCommand(right);
        else if (h < -0.05f) ioM.AddCommand(left);
        else ioM.AddCommand(stopHor);

        if (v > 0.05f) ioM.AddCommand(up);
        else if (v < -0.05f) ioM.AddCommand(down);
        else ioM.AddCommand(stopVer);

        if (Input.GetAxis(ioM.jumpName) > 0)
        {
            if (!pc.JumpCommand)
            {
                ioM.AddCommand(jump);
            }
        }
        else
        {
            if (pc.JumpCommand)
            {
                ioM.AddCommand(stopJump);
            }
        }

        if (Input.GetButtonDown(ioM.dashName))
        {
            ioM.AddCommand(dash);
        }
        if (Input.GetButtonDown(ioM.attackName))
        {
            ioM.AddCommand(attack);
        }

        ioM.ExecuteCommands();
    }

}
