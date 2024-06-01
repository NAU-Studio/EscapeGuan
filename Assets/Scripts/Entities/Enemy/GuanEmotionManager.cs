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

        private readonly Color transp = new(1, 1, 1, 0);
        public void ChangeEmotion(GuanEmotion e)
        {
            SpriteRenderer rd = GetComponent<SpriteRenderer>();
            rd.sprite = e switch
            {
                GuanEmotion.LoseTarget => LoseTarget,
                GuanEmotion.FindTarget => FindTarget,
                GuanEmotion.Rest => Rest,
                GuanEmotion.Angry => Angry,
                _ => throw new ArgumentOutOfRangeException(nameof(e))
            };
            rd.color = Color.white;
            IEnumerator emotionAnimation()
            {
                rd.transform.DOLocalMoveY(1, .2f).SetEase(Ease.OutCubic);
                yield return new WaitForSecondsRealtime(.2f);
                rd.transform.DOLocalMoveY(.75f, .2f).SetEase(Ease.InCubic);
                yield return new WaitForSecondsRealtime(2.2f);
                rd.DOColor(transp, 1);
            }
            StartCoroutine(emotionAnimation());
        }
    }
}
