using System;

namespace HearthstoneParody.Configs
{
    [Serializable]
    public class CardsDatabaseConfig
    {
        public int totalCards = 50;
        public int maxHealth = 10;
        public int maxAttack = 8;
        public int maxMana = 20;
        public string artUrl = "https://picsum.photos/512";
        public string generateNamesUrl = "http://names.drycodes.com/";
    }
}