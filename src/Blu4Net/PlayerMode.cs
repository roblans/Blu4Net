using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public struct PlayerMode : IEquatable<PlayerMode>
    {
        public ShuffleMode Shuffle { get; }
        public RepeatMode Repeat { get; }

        public PlayerMode(ShuffleMode shuffle, RepeatMode repeat)
        {
            Shuffle = shuffle;
            Repeat = repeat;
        }

        public override string ToString()
        {
            return $"{Shuffle} {Repeat}";
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerMode mode && Equals(mode);
        }

        public bool Equals(PlayerMode other)
        {
            return Shuffle == other.Shuffle &&
                   Repeat == other.Repeat;
        }

        public override int GetHashCode()
        {
            int hashCode = -1591799448;
            hashCode = hashCode * -1521134295 + Shuffle.GetHashCode();
            hashCode = hashCode * -1521134295 + Repeat.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(PlayerMode left, PlayerMode right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlayerMode left, PlayerMode right)
        {
            return !(left == right);
        }
    }

    public enum ShuffleMode
    {
        ShuffleOff,
        ShuffleOn,
    }

    public enum RepeatMode
    {
        RepeatOff,
        RepeatAll,
        RepeatOne,
    }
}
