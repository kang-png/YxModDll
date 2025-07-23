
using UnityEngine;


namespace YxModDll.Mod
{
    internal class DiTuSuoLueTu : MonoBehaviour
    {
        public static bool Show;

        public static Rect windowRect = new Rect();
        public static Rect hoveredRect = new Rect();

        private void Awake()
        {
            //windowRect.width = 200;
        }

        private void Update()
        {
            if (!UI_Main.ShowHuanTuUI)
            {
                Show = false;
                return;
            }
            if (UI_HuanTu.texture2D != null)
            {
                Show = UI_HuanTu.windowRect.Contains(Event.current.mousePosition);
            }
        }

        private void OnGUI()
        {
            if (Show)
            {
                Rect beijingRect = new Rect(UI_HuanTu.windowRect.xMin, UI_HuanTu.windowRect.yMin - UI_HuanTu.windowRect.width-5, UI_HuanTu.windowRect.width-10, UI_HuanTu.windowRect.width - 10);
                GUI.DrawTexture(beijingRect, UI_HuanTu.texture2D, ScaleMode.StretchToFill);
            }
        }
    }
}
