public class Node
{
    public int Index { get; set; }
    public double G { get; set; }
    public double H { get; set; }
    public double F { get { return G + H; } }
    public Node Parent { get; set; }
}