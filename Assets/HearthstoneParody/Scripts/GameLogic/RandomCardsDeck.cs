using HearthstoneParody.Core;
using HearthstoneParody.Data;
using UnityEngine;

namespace HearthstoneParody.GameLogic
{
    public interface ICardsDeck
    {
        Card GetNextCard();
    }

    public class RandomCardsDeck : ICardsDeck
    {
        private readonly Card.Factory _cardFactory;
        private readonly ICardsDatabaseProvider _cardsDatabaseProvider;

        public RandomCardsDeck(Card.Factory cardFactory, ICardsDatabaseProvider cardsDatabaseProvider)
        {
            _cardFactory = cardFactory;
            _cardsDatabaseProvider = cardsDatabaseProvider;
        }

        public Card GetNextCard()
        {
            var cardTemplate = _cardsDatabaseProvider.CardsDatabase.CardTemplates[
                Random.Range(0, _cardsDatabaseProvider.CardsDatabase.CardTemplates.Length)];
            return _cardFactory.Create(cardTemplate);
        }
    }
}