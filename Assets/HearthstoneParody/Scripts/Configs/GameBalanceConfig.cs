using System;

namespace HearthstoneParody.Configs
{
    [Serializable]
    public class GameBalanceConfig
    {
        public int playerStartHealthPoint = 15;
        public int playerStartMana = 5;
        public int manaPerRoundIncrease = 2;

        public int minCardAtStart = 4;
        public int maxCardAtStart = 6;
        public int newCardPerRound = 2;
    }
}