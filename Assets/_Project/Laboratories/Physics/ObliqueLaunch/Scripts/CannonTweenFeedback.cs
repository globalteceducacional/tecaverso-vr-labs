using DG.Tweening;
using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    [DisallowMultipleComponent]
    public sealed class CannonTweenFeedback : MonoBehaviour
    {
        [SerializeField] ObliqueLaunchLab source;
        [SerializeField, Range(0f,.5f)] float expansion = .16f;
        [SerializeField, Min(.01f)] float inflateDuration = .07f;
        [SerializeField, Min(.01f)] float recoverDuration = .18f;
        Vector3 restScale;
        Sequence sequence;
        public void Bind(ObliqueLaunchLab lab) => source=lab;
        void Awake() => restScale=transform.localScale;
        void OnEnable()
        { if(source==null) return; source.Launched+=Play; source.ResetRequested+=Restore; }
        void OnDisable()
        { if(source!=null){source.Launched-=Play; source.ResetRequested-=Restore;} Restore(); }
        public void Play()
        {
            Restore();
            var inflated=Vector3.Scale(restScale,new Vector3(1f+expansion,1f,1f+expansion));
            sequence=DOTween.Sequence()
                .Append(transform.DOScale(inflated,inflateDuration).SetEase(Ease.OutQuad))
                .Append(transform.DOScale(restScale,recoverDuration).SetEase(Ease.OutSine));
        }
        public void Restore() { sequence?.Kill(); sequence=null; transform.localScale=restScale; }
    }
}
