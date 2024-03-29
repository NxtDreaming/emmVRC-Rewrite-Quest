﻿using System;
using System.Diagnostics;
using UnityEngine;
using System.Threading;

#pragma warning disable 4014

namespace emmVRC.Functions.Other
{
    public class DestructiveActions
    {
        public static void ForceQuit()
        {
            Thread quitThread = new Thread(QuitAfterQuit)
            {
                IsBackground = true,
                Name = "emmVRC Quit Thread"
            };
            quitThread.Start();
        }
        public static void ForceRestart()
        {
            Thread restartThread = new Thread(RestartAfterQuit)
            {
                IsBackground = true,
                Name = "emmVRC Restart Thread"
            };
            restartThread.Start();
        }
        public static void RestartAfterQuit()
        {
            Application.Quit();
            Thread.Sleep(2500);
            try { Process.Start(@Environment.CurrentDirectory + "\\VRChat.exe", Environment.CommandLine.ToString()); } catch (Exception ex) { ex = new Exception(); }
            Process.GetCurrentProcess().Kill();
        }
        public static void QuitAfterQuit()
        {
            Application.Quit();
            Thread.Sleep(2500);
            Process.GetCurrentProcess().Kill();
        }
    }
}
