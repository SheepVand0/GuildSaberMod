using Steamworks;
using Oculus.Platform;
using Oculus.Platform.Models;

namespace GuildSaberProfile.API;

public static class Authentication
{
    public static string GetPlayerIdFromSteam()
    {
        if (!SteamManager.Initialized)
        {
            Plugin.Log.Error("Steam Manager not initialized");
            return Plugin.NOT_DEFINED;
        }

        if (SteamUser.GetHSteamUser() == null) return Plugin.NOT_DEFINED;

        if (!SteamUser.BLoggedOn()) return Plugin.NOT_DEFINED;

        CSteamID l_SteamIdBase = SteamUser.GetSteamID();
        string l_SteamId = l_SteamIdBase.ToString();

        if (string.IsNullOrEmpty(l_SteamId)) return Plugin.NOT_DEFINED;
        //Plugin.Log.Info($"Player Id : {l_SteamId}");
        return l_SteamId;
    }

    public static string GetPlayerIdFromOculus()
    {
        string l_OculusId = string.Empty;
        Request<User> l_TargetUser = Oculus.Platform.Users.GetLoggedInUser();
        l_TargetUser.OnComplete((Message p_Msg) =>
        {
            if (p_Msg.IsError)
            {
                Plugin.Log.Error($"Unable to get oculus id : {p_Msg.GetError()}");
                l_OculusId = Plugin.NOT_DEFINED;
            } else
            {
                l_OculusId = p_Msg.GetUser().OculusID;
            }
        });

        return l_OculusId;
    }
}
