﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore;
using UnityEngine;
using Verse;

namespace ToolkitPoints
{
    public static class TwitchAPI
    {
        if(ToolkitCoreSettings.twitchChannelUsername == "")
        {
            return;
        }
        static string GetChatterURL() => $"http://tmi.twitch.tv/group/user/{ToolkitCoreSettings.twitchChannelUsername}/chatters";

        static readonly Task<string> downloadChattersTask = new WebClient().DownloadStringTaskAsync(new Uri(GetChatterURL()));

        public static string lastChatterDownload = string.Empty;
        public static async Task UpdateChatters()
        {
            lastChatterDownload = await downloadChattersTask;
        }
    }
}
