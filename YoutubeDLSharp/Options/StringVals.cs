using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YoutubeDLSharp.Options
{
    public class StringVals
    {
        public readonly string[] Values;

        public StringVals(params string[] value)
        {
            Values = value;
        }

        public static implicit operator string[](StringVals p)
        {
            return p.Values;
        }

        public static implicit operator StringVals(string[] p)
        {
            return new StringVals(p);
        }

        public override string ToString()
        {
            return string.Join(" ", Values.Select(v => $"\"{v}\""));
        }

        public static bool operator ==(StringVals a, StringVals b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return a.Values.SequenceEqual(b.Values);
        }

        public static bool operator !=(StringVals a, StringVals b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is StringVals other)
            {
                return this == other;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var value in Values)
            {
                hash = hash * 31 + (value?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
}
