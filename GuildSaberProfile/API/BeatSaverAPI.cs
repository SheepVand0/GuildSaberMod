using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GuildSaberProfile.API;

internal static class BeatSaverAPI
{
    public static BeatSaverFormat FetchBeatMapByKey(string p_Key)
    {
        BeatSaverFormat l_BeatSaverFormat = null;
        using HttpClient l_HttpClient = new HttpClient();
        try
        {
            Task<string> l_Response = l_HttpClient.GetStringAsync($"https://api.beatsaver.com/maps/id/{p_Key}");
            l_Response.Wait();
            l_BeatSaverFormat = JsonConvert.DeserializeObject<BeatSaverFormat>(l_Response.Result);
        }
        catch (AggregateException l_AggregateException)
        {
            if (l_AggregateException.InnerException is HttpRequestException l_HttpRequestException)
            {
                /*switch (l_HttpRequestException.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        Logs.Info.Log("The Map do not exist");
                        return null;
                    case HttpStatusCode.TooManyRequests:
                        Logs.Info.Log("The bot got rate-limited on BeatSaver, Try later");
                        return null;
                    case HttpStatusCode.BadGateway:
                        Logs.Info.Log("Server BadGateway");
                        return null;
                    case HttpStatusCode.InternalServerError:
                        Logs.Info.Log("InternalServerError");
                        return null;
                }*/
                Plugin.Log.Error($"FetchBeatMap: Error during getting map, key : {p_Key}");
            }
            else
            {
                Plugin.Log.Error("FetchBeatMap: Unhandled exception)" + l_AggregateException.InnerException);
            }


        }

        return l_BeatSaverFormat;
    }

    public static BeatSaverFormat FetchBeatMapByHash(string p_Hash)
    {
        BeatSaverFormat l_BeatSaverFormat = null;
        using HttpClient l_HttpClient = new HttpClient();
        try
        {
            Task<string> l_Response = l_HttpClient.GetStringAsync($"https://api.beatsaver.com/maps/hash/{p_Hash}");
            l_Response.Wait();
            l_BeatSaverFormat = JsonConvert.DeserializeObject<BeatSaverFormat>(l_Response.Result);
        }
        catch (AggregateException l_AggregateException)
        {
            if (l_AggregateException.InnerException is HttpRequestException l_HttpRequestException)
            {
                /*switch (l_HttpRequestException.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        Logs.Info.Log("The Map do not exist");
                        return null;
                    case HttpStatusCode.TooManyRequests:
                        Logs.Info.Log("The bot got rate-limited on BeatSaver, Try later");
                        return null;
                    case HttpStatusCode.BadGateway:
                        Logs.Info.Log("Server BadGateway");
                        return null;
                    case HttpStatusCode.InternalServerError:
                        Logs.Info.Log("InternalServerError");
                        return null;
                }*/
            }
            else
            {
                Plugin.Log.Error($"FetchBeatMap: Unhandled exception) {l_AggregateException.InnerException}");
            }
        }

        return l_BeatSaverFormat;
    }

    public static uint StringToDifficulty(string p_Input)
    {
        return p_Input switch
        {
            "Easy" => 1,
            "Normal" => 3,
            "Hard" => 5,
            "Expert" => 7,
            "ExpertPlus" => 9,
            _ => 0
        };
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once ClassNeverInstantiated.Global
public class BeatSaverFormat
{
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public Uploader uploader { get; set; }
    public Metadata metadata { get; set; }
    public Stats stats { get; set; }
    public string uploaded { get; set; }
    public bool automapper { get; set; }
    public bool ranked { get; set; }
    public bool qualified { get; set; }
    public List<Version> versions { get; set; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Metadata
{
    public float bpm { get; set; }
    public int duration { get; set; }
    public string songName { get; set; }
    public string songSubName { get; set; }
    public string levelAuthorName { get; set; }
    public string songAuthorName { get; set; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Stats
{
    public int plays { get; set; }
    public int downloads { get; set; }
    public int upvotes { get; set; }
    public int downvotes { get; set; }
    public float score { get; set; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Uploader
{
    public int id { get; set; }
    public string name { get; set; }
    public string hash { get; set; }
    public string avatar { get; set; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Version
{
    public string hash { get; set; }
    public string key { get; set; }

    public string state { get; set; }

    public string createdAt { get; set; }
    public int sageScore { get; set; }
    public List<Diff> diffs { get; set; }
    public string downloadURL { get; set; }
    public string coverURL { get; set; }
    public string previewURL { get; set; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Diff
{
    public float njs { get; set; }
    public float offset { get; set; }
    public int notes { get; set; }
    public int bombs { get; set; }
    public int obstacles { get; set; }
    public float nps { get; set; }
    public float length { get; set; }
    public string characteristic { get; set; }
    public string difficulty { get; set; }
    public int events { get; set; }
    public bool chroma { get; set; }
    public bool me { get; set; }
    public bool ne { get; set; }
    public bool cinema { get; set; }
    public float seconds { get; set; }
    public ParitySummary paritySummary { get; set; }
    public int maxScore { get; set; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once ClassNeverInstantiated.Global
public class ParitySummary
{
    public int errors { get; set; }
    public int warns { get; set; }
    public int resets { get; set; }
}
