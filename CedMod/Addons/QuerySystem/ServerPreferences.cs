﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CedMod.ApiModals;
using Newtonsoft.Json;
using PluginAPI.Core;

namespace CedMod.Addons.QuerySystem
{
    public class ServerPreferences
    {
        public static ServerPreferenceModel Prefs = null;

        public static async Task ResolvePreferences(bool loop = true)
        {
            if (string.IsNullOrEmpty(QuerySystem.QuerySystemKey) || CedModMain.CancellationToken.IsCancellationRequested)
                return;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-ServerIp", Server.ServerIpAddress);
                    await VerificationChallenge.AwaitVerification();
                    if (CedModMain.Singleton.Config.CedMod.ShowDebug)
                        Log.Debug($"Getting Prefs.");
                    var response = await client.GetAsync($"http{(QuerySystem.UseSSL ? "s" : "")}://" + QuerySystem.CurrentMaster + $"/ServerPreference/GetServerPreference/{QuerySystem.QuerySystemKey}");
                    if (response.IsSuccessStatusCode)
                    {
                        var data = JsonConvert.DeserializeObject<ServerPreferenceModel>(
                            await response.Content.ReadAsStringAsync());
                        Prefs = data;
                        File.WriteAllText(Path.Combine(CedModMain.PluginConfigFolder, "CedMod", $"ServerPrefs.json"), JsonConvert.SerializeObject(Prefs));
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.PreconditionRequired)
                        {
                            VerificationChallenge.CompletedChallenge = false;
                            VerificationChallenge.ChallengeStarted = false;
                        }
                        Log.Error($"Failed to resolve server preferences, using file: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
                        if (File.Exists(Path.Combine(CedModMain.PluginConfigFolder, "CedMod", $"ServerPrefs.json"))) ;
                        Prefs = JsonConvert.DeserializeObject<ServerPreferenceModel>(File.ReadAllText(Path.Combine(CedModMain.PluginConfigFolder, "CedMod", $"ServerPrefs.json")));
                        if (loop)
                        {
                            await Task.Delay(1000, CedModMain.CancellationToken);
                            await ResolvePreferences();
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to resolve server preferences, using file: {e}");
                if (loop)
                {
                    await Task.Delay(1000, CedModMain.CancellationToken);
                    await ResolvePreferences();
                }
                return;
            }

            if (loop)
            {
                await WaitForSecond(60, CedModMain.CancellationToken, (o) => !Shutdown._quitting && CedModMain.Singleton.CacheHandler != null);
                await ResolvePreferences();
            }
        }
        
        public static async Task WaitForSecond(int i, CancellationToken token, Predicate<object> predicate)
        {
            int wait = i;
            while (wait >= 0 && predicate.Invoke(i))
            {
                await Task.Delay(1000, token);
                wait--;
            }
        }
    }
}