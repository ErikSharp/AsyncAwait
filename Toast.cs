class Toast
{
    public bool Buttered { get; set; }
    public bool HasJam { get; set; }

    public override string ToString()
    {
        return $"Buttered: {Buttered}, HasJam: {HasJam}";
    }
}