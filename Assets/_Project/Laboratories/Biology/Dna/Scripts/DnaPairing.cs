using System;

namespace Tecaverso.Labs.Biology.Dna
{
    /// <summary>Two independent, fixed four-position school exercises.</summary>
    public sealed class DnaPairing
    {
        public const string Template = "ATGC";
        readonly char[,] filled = new char[2, Template.Length];
        public bool Rna { get; private set; }
        int Row => Rna ? 1 : 0;
        public void Choose(bool rna) => Rna = rna;
        public char At(int index) => filled[Row, index];
        public char Expected(int index) => (Rna ? "UACG" : "TACG")[index];
        public bool Available(char letter) => (Rna ? "AUGC" : "ATGC").IndexOf(letter) >= 0;

        public int Find(char letter)
        {
            for (int i = 0; i < Template.Length; i++)
                if (At(i) == letter) return i;
            return -1;
        }

        public int Count
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Template.Length; i++)
                    if (At(i) != '\0') count++;
                return count;
            }
        }
        public bool HasWork
        {
            get
            {
                foreach (char letter in filled)
                    if (letter != '\0') return true;
                return false;
            }
        }
        public bool Place(int index, char letter)
        {
            if ((uint)index >= Template.Length || At(index) != '\0' || Expected(index) != letter) return false;
            filled[Row, index] = letter;
            return true;
        }
        public void Lift(char letter)
        {
            int index = Find(letter);
            if (index >= 0) filled[Row, index] = '\0';
        }
        public void Reset()
        {
            Array.Clear(filled, 0, filled.Length);
            Rna = false;
        }
    }
}
