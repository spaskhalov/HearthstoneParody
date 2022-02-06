using HearthstoneParody.Data;
using UnityEngine.UI;
using Zenject;

namespace HearthstoneParody.GameLogic
{
    public class GameController
    {
        private ICardsDeck _deck;
        
        [Inject]
        public void Init(Button btn, Player player, ICardsDeck deck)
        {
            btn.onClick.AddListener(() => player.CardsOnTable.Add(deck.GetNextCard(player)));
        }
    }
}