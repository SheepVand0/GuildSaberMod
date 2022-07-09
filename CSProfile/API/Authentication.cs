using Steamworks;

namespace CSProfile.API;

public static class Authentication
{

    public static string GetPlayerIdFromSteam()
    {
        if (!SteamManager.Initialized)
        {
            Plugin.Log.Error("Steam Manager not initialized");
            return Plugin.NOT_DEFINED;
        }

        CSteamID l_SteamIdBase = SteamUser.GetSteamID();
        string l_SteamId = l_SteamIdBase.ToString();

        if (string.IsNullOrEmpty(l_SteamId)) return Plugin.NOT_DEFINED;
        //Plugin.Log.Info($"Player Id : {l_SteamId}");
        return l_SteamId;
    }
}