using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class JumpCommand : ICommand
{
    private readonly PlayerCharacter pc;

    public JumpCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute()
    {
        pc.JumpCommand = true;
    }

    public void Undo(){ }
}

public class StopJumpCommand : ICommand
{
    private readonly PlayerCharacter pc;

    public StopJumpCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute()
    {
        pc.JumpCommand = false;
    }

    public void Undo(){ }
}

public class DashCommand : ICommand
{
    private readonly PlayerCharacter pc;

    public DashCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute()
    {
        pc.DashCommand = true;
    }

    public void Undo() { }
}

public class StopDashCommand : ICommand
{
    private readonly PlayerCharacter pc;

    public StopDashCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute()
    {
        pc.DashCommand = false;
    }

    public void Undo() { }
}

public class MoveLeftCommand : ICommand
{
    private readonly PlayerCharacter pc;

    public MoveLeftCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute()
    {
        pc.MoveLeftCommand = true;
    }

    public void Undo() { }
}

public class MoveRightCommand : ICommand
{
    private readonly PlayerCharacter pc;

    public MoveRightCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute()
    {
        pc.MoveRightCommand = true;
    }

    public void Undo() { }
}

public class StopMoveHorCommand : ICommand
{
    private readonly PlayerCharacter pc;

    public StopMoveHorCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute()
    {
        pc.MoveLeftCommand = false;
        pc.MoveRightCommand = false;
    }

    public void Undo() { }
}

public class MoveUpCommand : ICommand
{
    private readonly PlayerCharacter pc;

    public MoveUpCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute()
    {
        pc.MoveUpCommand = true;
    }

    public void Undo() { }
}

public class MoveDownCommand : ICommand
{
    private readonly PlayerCharacter pc;

    public MoveDownCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute()
    {
        pc.MoveDownCommand = true;
    }

    public void Undo() { }
}

public class StopMoveVerCommand : ICommand
{
    private readonly PlayerCharacter pc;

    public StopMoveVerCommand(PlayerCharacter pc)
    {
        this.pc = pc;
    }

    public void Execute()
    {
        pc.MoveUpCommand = false;
        pc.MoveDownCommand = false;
    }

    public void Undo() { }
}