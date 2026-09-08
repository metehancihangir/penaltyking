using UnityEngine;
namespace PenaltyKing
{
    public sealed class CrowdCelebration : MonoBehaviour
    {
        [SerializeField] private RectTransform[] sections;
        public RectTransform[] Sections => sections;
        public void Configure(RectTransform[] value) => sections = value;
        public void Sample(float time, bool celebrating)
        {
            if (sections == null) return;
            for (var i = 0; i < sections.Length; i++)
                sections[i].anchoredPosition = new Vector2(0, celebrating ? Mathf.Abs(Mathf.Sin(time * 15 + i * .32f)) * 5 : 0);
        }
    }
}
