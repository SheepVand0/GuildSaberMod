using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;

<<<<<<< Updated upstream
namespace CSProfile.API
{
    public static class Authentification
    {

        public static string GetPlayerIdFromSteam()
        {
            if (!SteamManager.Initialized)
            {
                Plugin.Log.Error("Steam Manager not initialized");
                return Plugin.m_notDefined;
            }

            CSteamID l_steamIdBase = SteamUser.GetSteamID();
=======
namespace CSProfile.API;
>>>>>>> Stashed changes

public static class Authentification
{ 
    public static string GetPlayerIdFromSteam()
    {
       if (!SteamManager.Initialized)
       {
           Plugin.Log.Error("Steam Manager not initialized");
           return Plugin.NOT_DEFINED;
       }

       CSteamID l_steamIdBase = SteamUser.GetSteamID();
       string l_steamId = l_steamIdBase.ToString();

       if (string.IsNullOrEmpty(l_steamId)) return Plugin.NOT_DEFINED;    
         Plugin.Log.Info($"Player Id : {l_steamId}");
         return l_steamId;    
    }
}

