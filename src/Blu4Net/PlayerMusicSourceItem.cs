using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class PlayerMusicSourceItem
    {
        public string Text { get; }
        public string Key { get; }

        public PlayerMusicSourceItem(string text, string key)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Key = key;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
