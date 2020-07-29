using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class PlayerPreset
    {
        public int Number { get; }
        public string Name { get; }
        public Uri ImageUri { get; }

        public PlayerPreset(int number, string name, Uri imageUri)
        {
            Number = number;
            Name = name;
            ImageUri = imageUri;
        }

        public override string ToString()
        {
            return $"{Number} - {Name}";
        }
    }
}
