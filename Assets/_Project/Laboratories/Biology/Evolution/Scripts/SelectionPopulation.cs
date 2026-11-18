namespace Tecaverso.Labs.Biology.Evolution
{
    // Relative reproductive contribution per phenotype; counts represent a whole cohort.
    public sealed class SelectionPopulation
    {
        public const int Size = 24;
        public const int Limit = 6;
        public int Light { get; private set; } = Size / 2;
        public int Dark => Size - Light;
        public int Generation { get; private set; }
        public bool DarkGround { get; private set; }
        public Snapshot? Reference { get; private set; }
        public bool HasChanges => Generation > 0 || DarkGround || Reference.HasValue;

        public readonly struct Snapshot
        {
            public readonly int Generation, Light;
            public readonly bool DarkGround;
            public Snapshot(int generation, int light, bool darkGround)
            {
                Generation = generation;
                Light = light;
                DarkGround = darkGround;
            }
        }

        public void SetGround(bool dark) => DarkGround = dark;
        public void SaveReference() => Reference = new Snapshot(Generation, Light, DarkGround);

        public bool Advance()
        {
            if (Generation >= Limit) return false;
            int lightWeight = Light * (DarkGround ? 7 : 10);
            int darkWeight = Dark * (DarkGround ? 10 : 7);
            int sum = lightWeight + darkWeight;
            // Round half up using the exact ratio, matching the review prototype.
            Light = (2 * Size * lightWeight + sum) / (2 * sum);
            Generation++;
            return true;
        }

        public void Reset()
        {
            Generation = 0;
            Light = Size / 2;
            DarkGround = false;
            Reference = null;
        }
    }
}
