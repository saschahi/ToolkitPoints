using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore.Controllers;
using ToolkitCore.Models;
using ToolkitPoints.SimpleJson;
using Verse;

namespace ToolkitPoints
{
    public class ChatterParse
    {
        public static List<Viewer> ParseChatterString()
        {
            List<Viewer> chatters = new List<Viewer>();

            string jsonString = TwitchAPI.lastChatterDownload;

            var parsed = JSON.Parse(jsonString);

            List<JSONArray> groups = new List<JSONArray>();

            groups.Add(parsed["chatters"]["broadcaster"].AsArray);
            groups.Add(parsed["chatters"]["moderators"].AsArray);
            groups.Add(parsed["chatters"]["staff"].AsArray);
            groups.Add(parsed["chatters"]["admins"].AsArray);
            groups.Add(parsed["chatters"]["global_mods"].AsArray);
            groups.Add(parsed["chatters"]["viewers"].AsArray);
            groups.Add(parsed["chatters"]["vips"].AsArray);

            foreach (JSONArray array in groups)
            {
                foreach (JSONNode node in array)
                {
                    string value = node.ToString();
                    string username = value.Substring(1, value.Length - 2);
                    
                    if (ViewerController.ViewerExists(username))
                    {
                        chatters.Add(ViewerController.GetViewer(username));
                    }
                    else
                    {
                        chatters.Add(ViewerController.CreateViewer(username));
                    }
                }
            }

            return chatters;
        }
    }
}
