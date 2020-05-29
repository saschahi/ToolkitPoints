using System;
using UnityEngine;
using Verse;

namespace ToolkitPoints
{
    public static class SettingsHelper
    {
        public static bool DrawClearButton(Rect canvas)
        {
            Rect region = new Rect(canvas.x + canvas.width - 16f, canvas.y, 16f, canvas.height);
            Widgets.ButtonText(region, "×", false);

            bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Event.current.clickCount > 0;

            if (!clicked)
            {
                return false;
            }

            GUI.FocusControl(null);
            return true;
        }
        
        public static bool DrawDoneButton(Rect canvas)
        {
            Rect region = new Rect(canvas.x + canvas.width - 16f, canvas.y, 16f, canvas.height);
            Widgets.ButtonText(region, "✔", false);

            bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Event.current.clickCount > 0;

            if (!clicked)
            {
                return false;
            }

            GUI.FocusControl(null);
            return true;
        }

        public static bool WasRightClicked(this Rect region)
        {
            if (!Mouse.IsOver(region))
            {
                return false;
            }

            Event current = Event.current;
            bool was = current.button == 1;

            switch (current.type)
            {
                case EventType.Used when was:
                case EventType.MouseDown when was:
                    current.Use();
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsRegionVisible(this Rect region, Rect scrollView, Vector2 scrollPos)
        {
            return (region.y >= scrollPos.y || region.y + region.height - 1f >= scrollPos.y) && region.y <= scrollPos.y + scrollView.height;
        }

        public static void DrawColored(this Texture2D t, Rect region, Color color)
        {
            Color old = GUI.color;

            GUI.color = color;
            GUI.DrawTexture(region, t);
            GUI.color = old;
        }

        public static Tuple<Rect, Rect> ToForm(this Rect region, float factor = 0.8f)
        {
            Rect left = new Rect(region.x, region.y, region.width * factor - 2f, region.height);

            return new Tuple<Rect, Rect>(
                left,
                new Rect(left.x + left.width + 2f, left.y, region.width - left.width - 2f, left.height)
            );
        }
    }
}
