using System;

namespace Tools.Logger
{
    public interface ILogger
    {
        void Log(string info);
        void LogException(Exception e);
        void LogError(string errorText);
        void LogWarning(string warning);
    }
}