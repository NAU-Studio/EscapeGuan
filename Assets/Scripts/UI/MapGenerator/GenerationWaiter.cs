using DG.Tweening;

using TMPro;

using UnityEngine;

namespace EscapeGuan.UI.MapGenerator
{
    public class GenerationWaiter : HidableUI
    {
        public SlicedFilledImage PartBar;
        public TMP_Text PartText, Title;

        private void Start()
        {
            Camera.main.orthographicSize = 128;
            ShowNoTransition();

            if (!Hidden)
                GameManager.Pause();
        }

        public override void Show()
        {
            base.Show();
            GameManager.Pause();
        }

        public override void Hide()
        {
            Camera.main.DOOrthoSize(8, 2).SetEase(Ease.InOutCubic);
            base.Hide();
            GameManager.Continue();
        }

        public void SetValue(float pp, float pc, float pm)
        {
            PartBar.fillAmount = pp / 100;
            PartText.text = $"{pp}% ({pc} / {pm}";
        }

        public void SetElapsed(float scale, float elapsed)
        {
            Title.text = $"完成！\n<size=16>地图面积 {scale} m²，花费了 {elapsed} 秒。</size>";
        }
    }
}