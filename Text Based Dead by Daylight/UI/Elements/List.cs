using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Text_Based_Dead_by_Daylight.UI.Elements
{
    public class List : UIComponent
    {
        private List<UIComponent> Items { get; set; }

        public List()
        {
            Items = new List<UIComponent>();
        }

        public void AddItem(UIComponent child)
        {
            Items.Add(child);
            MarkDirty();
        }

        public void AddItem(string text)
        {
            Items.Add(new Label(text));
            MarkDirty();
        }

        public List<UIComponent> GetItems()
        {
            return Items;
        }

        public override void Draw()
        {
            if(IsDirty)
            {
                base.Draw();
                foreach (var c in Items) c.IsDirty = true;
            }

            var itemsToDraw = Items.Skip(Math.Max(0, Items.Count - Height)).ToArray();
            
            var mostWidth = 0;
            for(int i=0;i<itemsToDraw.Length;i++)
            {
                if(itemsToDraw[i].Width > mostWidth)
                {
                    mostWidth = itemsToDraw[i].Width;
                }
            }

            for(int i=0; i < itemsToDraw.Length; i++)
            {
                var item = itemsToDraw[i];
                if (!item.IsDirty) continue;
                item.X = Parent.X + X;
                item.Y = Parent.Y + Y + i;
                item.Draw();
            }
        }

        private void MarkDirty()
        {
            IsDirty = true;
            foreach (var c in Items)
            {
                c.IsDirty = true;
            }
        }
    }
}
