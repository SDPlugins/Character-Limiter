using System;
using System.Linq;
using Rocket.Core;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using System.Xml.Serialization;
using Steamworks;
using Rocket.API.Collections;
using Rocket.Unturned;
using System.Collections;
using System.Collections.Generic;

namespace SDPlugins
{
    public class CharLimiterConfig : IRocketPluginConfiguration
    {
        public int CharAmount;
        public SerializableDictionary<CSteamID, List<bool>> totalCharacters;

        public void LoadDefaults()
        {
            CharAmount = 2;
            totalCharacters = new SerializableDictionary<CSteamID, List<bool>>();
        }
    }
    public class Init : RocketPlugin<CharLimiterConfig>
    {
        public static Init instance;
        protected override void Load()
        {
            instance = this;
            U.Events.OnPlayerConnected += Handler.PlayerJoined;
        }
        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Handler.PlayerJoined;
        }

        public override TranslationList DefaultTranslations 
        {
            get
            {
                return new TranslationList()
                {
                    {"char_limit_exceeded_kick", "You are only allowed to have {0} characters in the server." }
                };
            }
        }
        public void Kick(UnturnedPlayer player)
        {
            StartCoroutine(KickPlayer(player));
        }
        public IEnumerator KickPlayer(UnturnedPlayer player)
        {
            yield return new WaitForSeconds(10);
            Provider.kick(player.CSteamID, Translate("char_limit_exceeded_kick", Configuration.Instance.CharAmount));
        }

    }
    [XmlRoot("Dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
