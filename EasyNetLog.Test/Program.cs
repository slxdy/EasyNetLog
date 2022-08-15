﻿using System;
using System.Threading;

namespace EasyNetLog.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var logger = new EasyNetLogger((x) => $"[<color=magenta>{DateTime.Now:HH:mm:ss.fff}</color>] <color=gray>{x}</color>", true, new string[]
            {
                @"test.log"
            }, null);

            for (; ; )
            {
                logger.Log("A cool <color=#e63d6d>Juicy</color> <color=#94ff2a>Looking</color> log :)");
                Thread.Sleep(1000);
            }
        }
    }
}