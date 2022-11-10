using IPA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.Logger
{
    internal class GSLogger
    {
        private IPA.Logging.Logger IPALogger { get; set; } = null;

        internal static GSLogger Instance = null;

        public GSLogger(IPA.Logging.Logger p_Logger)
        {
            IPALogger = p_Logger;
            Instance = this;
        }

        internal void Log(string p_Message, StandardLogger.LogLevel p_LogType)
        {
            switch (p_LogType)
            {
                case StandardLogger.LogLevel.InfoUp:
                    IPALogger.Info(p_Message);
                    break;
                case StandardLogger.LogLevel.WarningUp:
                    IPALogger.Warn(p_Message);
                    break;
                case StandardLogger.LogLevel.ErrorUp:
                    IPALogger.Error(p_Message);
                    break;
                case StandardLogger.LogLevel.DebugUp:
                    IPALogger.Debug(p_Message);
                    break;
            }
        }

        internal void Error(Exception p_E, string p_Class, string p_Function)
        {
            Log($"[GuildSaber.{p_Class}][{p_Function}] : {p_E}", StandardLogger.LogLevel.ErrorUp);
        }

    }
}
