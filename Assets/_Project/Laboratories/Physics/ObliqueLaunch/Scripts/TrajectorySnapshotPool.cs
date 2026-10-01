using System.Collections.Generic;
using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class TrajectorySnapshotPool : MonoBehaviour
    {
        [SerializeField] ProjectileView[] snapshots;
        [SerializeField, Min(.1f)] float interval = .5f;
        readonly List<ProjectileView> active = new();
        float nextTime;
        public void Bind(ProjectileView[] pooledViews) => snapshots = pooledViews;

        public void Record(in FlightSample sample)
        {
            if (sample.Time + .001f < nextTime || active.Count >= snapshots.Length) return;
            var view = snapshots[active.Count];
            view.gameObject.SetActive(true);
            view.Show(sample);
            active.Add(view);
            nextTime += interval;
        }

        public void Clear()
        {
            foreach (var view in snapshots) if (view != null) view.gameObject.SetActive(false);
            active.Clear();
            nextTime = 0f;
        }
    }
}
