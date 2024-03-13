public class NodePuzzle
{
    public int[,] State { get; set; }
    public int G { get; set; }
    public int H { get; set; }
    public int F { get { return G + H; } }
    public NodePuzzle Parent { get; set; }

    public NodePuzzle(int[,] state, int g, int h, NodePuzzle parent)
    {
        State = state;
        G = g;
        H = h;
        Parent = parent;
    }
}
