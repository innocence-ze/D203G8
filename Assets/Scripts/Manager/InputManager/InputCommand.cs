using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    void Execute(float value = 0);
    void Undo();
}

public class JumpCommand : ICommand
{
    private PlayerCharacter pc;

    public JumpCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute(float value = 0)
    {
        pc.JumpCommand = true;
    }

    public void Undo(){ }
}

public class StopJumpCommand : ICommand
{
    private PlayerCharacter pc;

    public StopJumpCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute(float value = 0)
    {
        pc.JumpCommand = false;
    }

    public void Undo(){ }
}

public class DashCommand : ICommand
{
    private PlayerCharacter pc;

    public DashCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute(float value = 0)
    {
        pc.DashCommand = true;
    }

    public void Undo() { }
}

public class StopDashCommand : ICommand
{
    private PlayerCharacter pc;

    public StopDashCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute(float value = 0)
    {
        pc.DashCommand = false;
    }

    public void Undo() { }
}

public class MoveHorCommand : ICommand
{
    private PlayerCharacter pc;

    public MoveHorCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute(float value = 0)
    {
        pc.MoveHorCommand = value;
    }

    public void Undo() { }
}

public class MoveVerCommand : ICommand
{
    private PlayerCharacter pc;

    public MoveVerCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute(float value = 0)
    {
        pc.MoveVerCommand = value;
    }

    public void Undo() { }
}