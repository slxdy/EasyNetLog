using System;
using System.Threading;

namespace EasyNetLog.Test;

internal class Program
{
    private static void Main()
    {
        var logger = new EasyNetLogger((x) => $"[<color=magenta>{DateTime.Now:HH:mm:ss.fff}</color>] <color=gray>{x}</color>", true, new string[]
        {
            @"test.log"
        }, null);

        for (; ; )
        {
            logger.Log("A cool <color=#e63d6d>fuity</color> <color=#94ff2a>log</color> :)");
            Thread.Sleep(1000);
        }
    }
}