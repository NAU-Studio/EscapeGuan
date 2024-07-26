using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

namespace EscapeGuan.Starting
{
    public class StartMenu : MonoBehaviour
    {
        public AudioSource Music;

        public CanvasGroup Main;
        public Image BlackMask;

        public void Awake()
        {
            Main.alpha = 0;
            Main.DOFade(1, .2f);
        }

        public void Exit()
        {
            StartCoroutine(ExitCoroutine());
        }

        public IEnumerator ExitCoroutine()
        {
            Main.DOFade(0, 1f);
            Music.DOFade(0, 1.5f);
            yield return new WaitForSeconds(.5f);
            BlackMask.DOColor(new(0, 0, 0, 1), .5f);
            yield return new WaitForSeconds(.6f);
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        }
    }

    [Serializable]
    public class FadeAnimationNode
    {
        public float Duration;
        public float Delay;
        public float EndValue;
        public CanvasGroup Target;

        public FadeAnimationNode(float duration, float delay, float endValue, CanvasGroup target)
        {
            Duration = duration;
            Delay = delay;
            EndValue = endValue;
            Target = target;
        }
    }
}
