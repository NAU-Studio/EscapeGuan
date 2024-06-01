using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using DG.Tweening;

using TMPro;

using UnityEngine;

// See: https://github.com/Auleiy/EscapeGuanMultiplayer
// WIP
namespace EscapeGuan.Network.Starting
{
    public class MultiplayerPage : MonoBehaviour
    {
        public CanvasGroup MainPage, Connection, Login;

        public TMP_InputField IP, Port, Username, Password;

        public Socket Target;

        public void ShowPage()
        {
            MainPage.DOFade(1, .2f);
            MainPage.interactable = true;
        }

        public void TryConnect()
        {
            string addr = IP.text;
            string port = Port.text;

            try
            {
                Uri a = new(addr);
                Target = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress tgt = Dns.GetHostEntry(a.Host).AddressList[0];
                Target.Connect(tgt, int.Parse(port));

                // Processing

                Connection.DOFade(0, .2f);
                Connection.interactable = false;

                Login.DOFade(1, .2f);
                Login.interactable = true;
            }
            catch { }
        }

        public void _Login()
        {
            byte[] c1buf = new byte[4096];
            Target.Receive(c1buf);
            byte[] loginCommand = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            if (c1buf[..4].SequenceEqual(loginCommand))
            {
                string u = Username.text;
                string p = Password.text;
                List<byte> cmd = new() { 0x00, 0x00, 0x00, 0x00, 0x00 };
                cmd.AddRange(Encoding.Default.GetBytes(u));
                cmd.Add(0x00);
                cmd.AddRange(Encoding.Default.GetBytes(p));
                Target.Send(cmd.ToArray());
            }
        }
    }
}
