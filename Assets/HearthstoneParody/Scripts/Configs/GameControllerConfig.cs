using System;
using HearthstoneParody.Presenters;
using TMPro;
using UnityEngine.UI;

namespace HearthstoneParody.Configs
{
    [Serializable]
    public class GameControllerConfig
    {
        public TMP_Text statusText;
        public PlayerPresenterBase firstPlayer;
        public PlayerPresenterBase secondPlayer;
        public Button restartButton;
    }
}