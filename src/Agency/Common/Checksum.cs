using System;

namespace Agency.Common
{
    public class Checksum
    {
        private int value = 0;

        public int Value => value;

        public void Add(int a)
        {
            value ^= a;
        }

        public void Add(float a)
        {
            value ^= BitConverter.ToInt32(BitConverter.GetBytes(a), 0);
        }

        public void Add(Checksum checksum)
        {
            value ^= checksum.value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}