using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EscapeGuan.Entities;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

using Attribute = EscapeGuan.Entities.Attribute;

namespace EscapeGuan
{
    public class Console : MonoBehaviour
    {
        public List<ConsoleLine> Lines = new();
        public TMP_Text TextUI;
        public TMP_InputField Command;
        [Multiline(6)]
        public string HelpText;

        public List<CommandAction> InstructionSet = new();

        private bool Showed = false;

        private void Start()
        {
            // Base instruction set
            List<CommandAction> bis = new()
            {
                new(new string[] { "help" }, GetHelp),
                new(new string[] { "echo" }, Echo),
                new(new string[] { "idself" }, GetIdSelf),
                new(new string[] { "getattrlist"}, GetAttributeList),
                new(new string[] { "getattr" }, GetAttribute),
                new(new string[] { "setattr" }, SetAttribute),
                new(new string[] { "destroyentity"}, DestroyEntity),
            };

            bis.AddRange(InstructionSet);
            InstructionSet = bis;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            GameManager.Pause();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            GameManager.Continue();
        }

        public void Toggle()
        {
            Showed = !Showed;
            if (Showed)
                Show();
            else
                Hide();
        }

        public void WriteLine(string value, ConsoleLine.Types type = ConsoleLine.Types.NormalText)
        {
            Lines.Add(new(value, type));
        }

        public void ExecuteCommand(string cmd)
        {
            Lines.Add(new(cmd, ConsoleLine.Types.Command));
            string[] spl = cmd.Split(' ');
            string cn = spl[0];
            List<string> param = new(spl[1..]);
            foreach (string p in param)
            {
                if (string.IsNullOrEmpty(p))
                    param.Remove(p);
            }
            foreach (CommandAction c in InstructionSet)
            {
                if (c.Names.Contains(cn))
                    c.Action.Invoke(param.ToArray(), this);
            }
        }

        public void GetHelp(string[] param, Console con)
        {
            string[] x = con.HelpText.Split('\n');
            foreach (string s in x)
                con.WriteLine(s);
        }

        public void Echo(string[] param, Console con)
        {
            int useOneLine = Array.IndexOf(param, "-o");
            if (useOneLine < 0)
            {
                foreach (string x in param)
                    con.WriteLine(x);
            }
            else
            {
                StringBuilder sb = new();
                for (int i = 0; i < param.Length; i++)
                {
                    if (i == useOneLine)
                        continue;
                    sb.Append(param[i] + ' ');
                }
                con.WriteLine(sb.ToString());
            }
        }

        public void GetIdSelf(string[] param, Console con)
        {
            con.WriteLine(GameManager.Main.ControlledEntityId.ToString(), ConsoleLine.Types.True);
        }

        public void GetAttributeList(string[] param, Console con)
        {
            if (param.Length != 1)
            {
                con.Lines.Add(new("GetAttributeList needs 1 arguments.", ConsoleLine.Types.False));
                return;
            }
            if (!int.TryParse(param[0], out int id))
            {
                con.Lines.Add(new("id parameter is not a number.", ConsoleLine.Types.False));
                return;
            }
            if (!GameManager.Main.EntityPool.ContainsKey(id))
            {
                con.WriteLine("Entity not exists.", ConsoleLine.Types.False);
                return;
            }
            AttributeList attributes = GameManager.Main.EntityPool[id].Attributes;
            foreach (Attribute a in attributes)
                con.WriteLine($"{a.Name}: {a.Type}");
        }

        public void GetAttribute(string[] param, Console con)
        {
            if (param.Length != 2)
            {
                con.Lines.Add(new("GetAttribute needs 2 arguments.", ConsoleLine.Types.False));
                return;
            }
            if (!int.TryParse(param[0], out int id))
            {
                con.Lines.Add(new("id parameter is not a number.", ConsoleLine.Types.False));
                return;
            }
            if (!GameManager.Main.EntityPool.ContainsKey(id))
            {
                con.WriteLine("Entity not exists.", ConsoleLine.Types.False);
                return;
            }
            AttributeList attributes = GameManager.Main.EntityPool[id].Attributes;
            if (!attributes.ContainsName(param[1]))
            {
                con.WriteLine("Attribute not exists.", ConsoleLine.Types.False);
                return;
            }
            con.WriteLine(
                ((Delegate)attributes[param[1]]
                .GetType()
                .GetField("Getter")
                .GetValue(attributes[param[1]]))
                .DynamicInvoke().ToString(), ConsoleLine.Types.True
                );
        }

        public void SetAttribute(string[] param, Console con)
        {
            if (param.Length != 3)
            {
                con.Lines.Add(new("SetAttribute needs 3 arguments.", ConsoleLine.Types.False));
                return;
            }
            if (!int.TryParse(param[0], out int id))
            {
                con.Lines.Add(new("id parameter is not a number.", ConsoleLine.Types.False));
                return;
            }
            if (!GameManager.Main.EntityPool.ContainsKey(id))
            {
                con.WriteLine("Entity not exists.", ConsoleLine.Types.False);
                return;
            }
            AttributeList attributes = GameManager.Main.EntityPool[id].Attributes;
            if (!attributes.ContainsName(param[1]))
            {
                con.WriteLine("Attribute not exists.", ConsoleLine.Types.False);
                return;
            }
            Type t = attributes[param[1]].Type;

            if (t == typeof(float))
            {
                if (!float.TryParse(param[2], out float v))
                {
                    con.WriteLine($"The type of value dosen't match the type of the attribute ({t})", ConsoleLine.Types.False);
                    return;
                }
                ((Delegate)attributes[param[1]]
                .GetType()
                .GetField("Setter")
                .GetValue(attributes[param[1]]))
                .DynamicInvoke(v);
            }
            con.WriteLine(
                ((Delegate)attributes[param[1]]
                .GetType()
                .GetField("Getter")
                .GetValue(attributes[param[1]]))
                .DynamicInvoke().ToString(), ConsoleLine.Types.True
                );
        }

        public void DestroyEntity(string[] param, Console con)
        {
            if (param.Length != 1)
            {
                con.Lines.Add(new("DestroyEntity needs 1 arguments.", ConsoleLine.Types.False));
                return;
            }
            if (!int.TryParse(param[0], out int id))
            {
                con.Lines.Add(new("id parameter is not a number.", ConsoleLine.Types.False));
                return;
            }
            if (!GameManager.Main.EntityPool.ContainsKey(id))
            {
                con.WriteLine("Entity not exists.", ConsoleLine.Types.False);
                return;
            }
            if (id == GameManager.Main.ControlledEntityId)
            {
                con.WriteLine("<color=#FFE97F>WARNING: You are destroying controlled entity. Destroy it may cause problems.</color>");
            }
            Destroy(GameManager.Main.EntityPool[id].gameObject);
        }
        /*
<color=#AAFF7F>EscapeGuan Debug Console Help</color>
Help
- Syntax: help
- Usage:  Show the help of EscapeGuan Debug Console.

Echo
- Syntax:     echo [-o] <content>
- Usage:      Print content to the screen.
- Parameters: -o:      Use one line.
              content: Print content.
                       Multi parameter means multi-line or space.

GetIdSelf
- Syntax:  idself
- Usage:   Get the id of self (player).
- Returns: The id of self (player).

GetIdNear
- Syntax:     idnear [mdist=5]
- Usage:      Get the id of near entity in range mdist.
- Parameters: mdist: Range.
- Returns:    The id of target entity.

GetAttribute
- Syntax:     getattribtype <id> <attributeName>
- Usage:      Get the attribute of target attribute.
- Parameters: id:            The attribute owner's id.
              attributeName: The name of target attribute.
- Returns:    The type and the value of target attribute.


SetAttribute
- Syntax:     attribute <id> [attributeName] [value]
- Usage:      Set the attribute's value.
- Parameters: id:            The attribute owner's id.
              attributeName: The name of target attribute.
              value:         The value wanted to set
*/

        private bool CommandAllowEnter = false;

        private void Update()
        {
            StringBuilder sb = new();
            foreach (ConsoleLine l in Lines)
                sb.AppendLine(l.ToString());
            TextUI.text = sb.ToString();

            if (CommandAllowEnter && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
            {
                if (!string.IsNullOrEmpty(Command.text))
                {
                    ExecuteCommand(Command.text);
                    Command.text = string.Empty;
                }
                CommandAllowEnter = false;
            }
            else
            {
                CommandAllowEnter = Command.isFocused;
            }
        }
    }

    [Serializable]
    public class ConsoleLine
    {
        public string Content;
        public enum Types
        {
            NormalText,
            Command,
            True,
            False
        }
        public Types Type;

        public ConsoleLine(string content, Types type)
        {
            Content = content;
            Type = type;
        }

        public override string ToString()
        {
            return Type switch
            {
                Types.NormalText => $"  {Content}",
                Types.Command => $"> {Content}",
                Types.True => $"<color=#AAFF7F>¡Ñ</color> {Content}",
                Types.False => $"<color=#FF7F7F>¡Á</color> {Content}",
                _ => $"  {Content}"
            };
        }
    }

    [Serializable]
    public class CommandAction
    {
        public List<string> Names;
        public UnityEvent<string[], Console> Action;

        public CommandAction(string[] names, UnityEvent<string[], Console> action)
        {
            Names = names.ToList();
            Action = action;
        }

        public CommandAction(string[] names, UnityAction<string[], Console> action)
        {
            Names = names.ToList();
            Action = new();
            Action.AddListener(action);
        }
    }
}
