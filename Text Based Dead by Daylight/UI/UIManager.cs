using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Text_Based_Dead_by_Daylight.UI
{
    public class UIManager
    {
        static List<UIComponent> Components { get; set; } = new List<UIComponent>();

        public static void Add(UIComponent component)
        {
            Components.Add(component);
        }

        public static void Update()
        {
            foreach (var component in Components)
            {
                component.Update();
                if (!component.Visible || (component.Parent != null && !component.IsDirty)) continue;
                component.Draw();
            }

            Thread.Sleep(500);
        }
    }
}
