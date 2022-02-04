using DG.Tweening;
using HearthstoneParody.Data;
using HearthstoneParody.Presenters;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HearthstoneParody.Animations
{
    [RequireComponent(typeof(ICardPresenter))]
    public class WiggleTheCardOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float hoverHeightMultiplayer = 0.1f;
        private ICardPresenter _cardPresenter;
        private Tween _goingUpTween;

        private void Start()
        {
            _cardPresenter = GetComponent<ICardPresenter>();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_goingUpTween != null 
               || DOTween.IsTweening(_cardPresenter.RectTransform) 
               || _cardPresenter.IsSelectedByUser.Value)
                return;
            
            var targetPosition = _cardPresenter.RectTransform.localPosition 
                                 + _cardPresenter.RectTransform.up * _cardPresenter.RectTransform.rect.height * hoverHeightMultiplayer;
            _goingUpTween = _cardPresenter.RectTransform
                .DOLocalMove(targetPosition, 0.3f)
                .SetAutoKill(false)
                .OnKill(() => _goingUpTween = null)
                .OnRewind(() => _goingUpTween.Kill());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_goingUpTween != null && !(_goingUpTween.IsPlaying() && _goingUpTween.isBackwards))
                _goingUpTween.PlayBackwards();
        }
    }
}