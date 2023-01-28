﻿using System;

namespace GuildSaber.Logger;

internal class GSLogger
{

    internal static GSLogger Instance;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Init
    /// </summary>
    /// <param name="p_Logger"></param>
    public GSLogger(IPA.Logging.Logger p_Logger)
    {
        IPALogger = p_Logger;
        Instance = this;
    }
    private IPA.Logging.Logger IPALogger { get; }

    /// <summary>
    ///     Log something
    /// </summary>
    /// <param name="p_Message"></param>
    /// <param name="p_LogType"></param>
    internal void Log(object p_Message, IPA.Logging.Logger.LogLevel p_LogType)
    {
        switch (p_LogType) {
            case IPA.Logging.Logger.LogLevel.InfoUp:
                IPALogger.Info(p_Message.ToString());
                break;
            case IPA.Logging.Logger.LogLevel.NoticeUp:
                IPALogger.Notice(p_Message.ToString());
                break;
            case IPA.Logging.Logger.LogLevel.WarningUp:
                IPALogger.Warn(p_Message.ToString());
                break;
            case IPA.Logging.Logger.LogLevel.ErrorUp:
                IPALogger.Error(p_Message.ToString());
                break;
            case IPA.Logging.Logger.LogLevel.DebugUp:
                IPALogger.Debug(p_Message.ToString());
                break;
        }
    }

    internal void Log(bool p_Value, IPA.Logging.Logger.LogLevel p_LogType) { Log(p_Value.ToString(), p_LogType); }

    /// <summary>
    ///     Log Error
    /// </summary>
    /// <param name="p_E"></param>
    /// <param name="p_Class"></param>
    /// <param name="p_Function"></param>
    internal void Error(Exception p_E, string p_Class, string p_Function)
    {
        Log($"[GuildSaber.{p_Class}][{p_Function}] : {p_E}", IPA.Logging.Logger.LogLevel.ErrorUp);
        Log($"[GuildSaber.{p_Class}][{p_Function}] Source : {p_E.Source}", IPA.Logging.Logger.LogLevel.ErrorUp);
        Log($"[GuildSaber.{p_Class}][{p_Function}] TargetSite : {p_E.TargetSite}", IPA.Logging.Logger.LogLevel.ErrorUp);

    }
}