using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dcidr.Model
{
    public class DecisionItemCollection
    {
        private HashSet<string> _items;

        public DecisionItemCollection()
        {
            _items = new HashSet<string>();
        }

        public event EventHandler OnChange;

        public void Add(string item)
        {
            if (string.IsNullOrWhiteSpace(item)) return;
            _items.Add(item);
            OnChanged(new EventArgs());
        }

        public void Remove(string item)
        {
            _items.Remove(item);
            OnChanged(new EventArgs());
        }

        public IEnumerable<string> Items => _items.OrderBy(b=>b).AsEnumerable();

        public int Count => _items.Count();

        private void OnChanged(EventArgs e)
        {
            EventHandler handler = OnChange;
            OnChange?.Invoke(this, e);
        }
    }
}
