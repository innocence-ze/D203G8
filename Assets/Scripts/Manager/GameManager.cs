using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class GameManager : MonoBehaviour
{
    private PlayerCharacter pc;


    private InputManager input;
    ICommand jump, stopJump, dash, stopDash, hor, ver;

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
        input = GetComponent<InputManager>();
        jump = new JumpCommand(pc);
        stopJump = new StopJumpCommand(pc);
        dash = new DashCommand(pc);
        stopDash = new StopDashCommand(pc);
        hor = new MoveHorCommand(pc);
        ver = new MoveVerCommand(pc);
    }

    void UpdateInputManager()
    {
        input.AddCommand(hor, Input.GetAxis("Horizontal"));
        input.AddCommand(ver, Input.GetAxis("Vertical"));

        if (Input.GetAxis("Jump") > 0 && !pc.JumpCommand)
        {
            input.AddCommand(jump, 1);
        }
        if (Input.GetAxis("Jump") == 0 && pc.JumpCommand)
        {
            input.AddCommand(stopJump, 0);
        }

        if (Input.GetAxis("Dash") > 0 && !pc.DashCommand)
        {
            input.AddCommand(dash, 1);
        }
        if (Input.GetAxis("Dash") == 0 && pc.DashCommand)
        {
            input.AddCommand(stopDash, 0);
        }


        input.ExecuteCommands();
    }

}
