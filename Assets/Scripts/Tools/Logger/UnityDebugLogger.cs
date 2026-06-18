using System;
using UnityEngine;

namespace Tools.Logger
{
    public class UnityDebugLogger : ILogger
    {
        public void Log(string info)
        {
            Debug.Log(info);
        }

        public void LogException(Exception e)
        {
            Debug.LogException(e);
        }

        public void LogError(string errorText)
        {
            Debug.LogError(errorText);
        }

        public void LogWarning(string warning)
        {
            Debug.LogWarning(warning);
        }
    }
}