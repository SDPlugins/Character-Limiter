using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDPlugins
{

    public class Handler
    {
        public static void PlayerJoined(UnturnedPlayer player)
        {
            SteamPlayer splayer = PlayerTool.getSteamPlayer(player.CSteamID);
            SerializableDictionary<CSteamID, List<bool>> totalCharacters = Init.instance.Configuration.Instance.totalCharacters;

            if (totalCharacters.ContainsKey(player.CSteamID))
            {
                int count = 0;
                for (int i = 0; i < totalCharacters[player.CSteamID].Count; i++)
                {
                    if (totalCharacters[player.CSteamID][i] && i != splayer.playerID.characterID)
                        count++;
                }

                if (count > Init.instance.Configuration.Instance.CharAmount - 1)
                {
                    Init.instance.Kick(player);
                }
                else
                {
                    totalCharacters[player.CSteamID][splayer.playerID.characterID] = true;
                }
            }
            else
            {
                totalCharacters.Add(player.CSteamID, new List<bool>()
                {
                    false,
                    false,
                    false,
                    false,
                    false
                });

                totalCharacters[player.CSteamID][splayer.playerID.characterID] = true;
            }
            Init.instance.Configuration.Instance.totalCharacters = totalCharacters;
            Init.instance.Configuration.Save();
        }
    }
}
