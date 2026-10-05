using System;
using UnityEngine;

namespace Tecaverso.Hub
{
    [CreateAssetMenu(menuName="Tecaverso/Hub Catalog")]
    public sealed class HubCatalog : ScriptableObject
    {
        public Discipline[] disciplines;
        [Serializable] public sealed class Discipline
        {
            public string title;
            [TextArea] public string description;
            public Color accent;
            public Sprite icon;
            public Content[] contents;
        }
        [Serializable] public sealed class Content
        {
            public string title;
            [TextArea] public string description;
            [TextArea] public string objective;
            public Sprite icon;
            [Tooltip("Empty = mockup. Use the exact enabled scene path for available content.")]
            public string scenePath;
        }
    }
}
