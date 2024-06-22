using System;
using System.Collections;

using DG.Tweening;

using UnityEngine;

namespace EscapeGuan.Entities.Enemy
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GuanEmotionManager : MonoBehaviour
    {
        public Sprite LoseTarget, FindTarget, Rest, Angry;
    
        private readonly Color Transparent = new(1, 1, 1, 0);
        private Coroutine EmotionAnimationCoroutine;
        
        private SpriteRenderer Sprite => GetComponent<SpriteRenderer>();

        public void ChangeEmotion(GuanEmotion e)
        {
            Sprite.sprite = e switch
            {
                GuanEmotion.LoseTarget => LoseTarget,
                GuanEmotion.FindTarget => FindTarget,
                GuanEmotion.Rest => Rest,
                GuanEmotion.Angry => Angry,
                _ => throw new ArgumentOutOfRangeException(nameof(e))
            };

            if (EmotionAnimationCoroutine != null)
                StopCoroutine(EmotionAnimationCoroutine);
            
            EmotionAnimationCoroutine = StartCoroutine(EmotionAnimation());
        }

        private IEnumerator EmotionAnimation()
        {
            Sprite.color = Color.white;
            Sprite.transform.DOLocalMoveY(1, .2f).SetEase(Ease.OutCubic);
            yield return new WaitForSecondsRealtime(.2f);
            Sprite.transform.DOLocalMoveY(.75f, .2f).SetEase(Ease.InCubic);
            yield return new WaitForSecondsRealtime(2.2f);
            Sprite.DOColor(Transparent, 1);
        }
    }
}
