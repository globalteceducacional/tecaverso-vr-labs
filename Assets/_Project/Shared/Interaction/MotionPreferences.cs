using System;
using UnityEngine;
namespace Tecaverso.Interaction
{
    public static class MotionPreferences
    {
        static bool? reduced;
        public static bool Reduced => reduced ?? (reduced = PlayerPrefs.GetInt("Tecaverso.ReducedMotion",0)!=0).Value;
        public static event Action Changed;
        public static void SetReduced(bool value){reduced=value;PlayerPrefs.SetInt("Tecaverso.ReducedMotion",value?1:0);PlayerPrefs.Save();Changed?.Invoke();}
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] static void Reset(){Changed=null;reduced=null;}
    }
}
