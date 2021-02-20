using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public string jumpName = "Jump";
    public string dashName = "Dash";
    public string horName = "Horizontal";
    public string verName = "Vertical";


    private readonly List<ICommand> commands = new List<ICommand>();

    public void AddCommand(ICommand c)
    {
        commands.Add(c);
    }

    public void ExecuteCommands()
    {
        for(int i = 0; i < commands.Count; i++)
        {
            commands[i].Execute();
        }
        commands.Clear();
    }
    
}
