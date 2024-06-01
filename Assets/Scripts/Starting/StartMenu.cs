using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEditor;

using UnityEngine;

namespace EscapeGuan.Starting
{
    public class StartMenu : MonoBehaviour
    {
        public List<FadeAnimationNode> Animation = new();
        public List<CanvasGroup> StartHide = new();
        public AudioSource Music;

        public CanvasGroup Main;

        private void Start()
        {
            foreach (CanvasGroup c in StartHide)
                c.alpha = 0;
        }

        public void Awake()
        {
            StartCoroutine(AnimationCoroutine());
        }

        public IEnumerator AnimationCoroutine()
        {
            yield return new WaitForSeconds(.5f);
            Music.Play();
            foreach (FadeAnimationNode node in Animation)
            {
                yield return new WaitForSeconds(node.Delay);
                node.Target.DOFade(node.EndValue, node.Duration);
                yield return new WaitForSeconds(node.Duration);
                if (node.EndValue == 0)
                    node.Target.blocksRaycasts = false;
            }
        }

        public void Exit()
        {
            StartCoroutine(ExitCoroutine());
        }

        public IEnumerator ExitCoroutine()
        {
            Main.DOFade(0, .5f);
            Music.DOFade(0, .9f);
            yield return new WaitForSeconds(1);
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
