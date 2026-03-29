using System;
using System.Collections.Generic;

namespace Bejeweled;

    public class Time
    {
        public int Minutes { get; private set; }
        public int Seconds { get; private set; }

        public Time(int minutes = 0, int seconds = 0)
        {
            Minutes = minutes;
            Seconds = seconds;
        }

        public void Increment()
        {
            Seconds++;
            if (Seconds >= 60)
            {
                Seconds = 0;
                Minutes++;
            }
        }

        public override string ToString() => $"{Minutes:D2}:{Seconds:D2}";

        public void Reset()
        {
            Minutes = 0;
            Seconds = 0;
        }

        public bool IsZero() => Minutes == 0 && Seconds == 0;

        public bool Equals(Time other)
        {
            if (other == null) return false;
            return this.Minutes == other.Minutes && this.Seconds == other.Seconds;
        }

        public override bool Equals(object obj) => Equals(obj as Time);

        public override int GetHashCode() => HashCode.Combine(Minutes, Seconds);
    }