using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private List<ICommand> commands = new List<ICommand>();
    private List<float> values = new List<float>();

    public void AddCommand(ICommand c, float v)
    {
        commands.Add(c);
        values.Add(v);
    }

    public void ExecuteCommands()
    {
        for(int i = 0; i < commands.Count; i++)
        {
            commands[i].Execute(values[i]);
        }
        commands.Clear();
        values.Clear();
    }
    
}
