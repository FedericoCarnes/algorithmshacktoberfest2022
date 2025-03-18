using System;
using System.Collections.Generic;
using System.Linq;

public class Node
{
    public int X, Y;
    public int G, H;
    public Node Parent;
    
    public int F => G + H;
    
    public Node(int x, int y, Node parent = null)
    {
        X = x;
        Y = y;
        Parent = parent;
    }
    
    public override bool Equals(object obj)
    {
        return obj is Node node && node.X == X && node.Y == Y;
    }
    
    public override int GetHashCode()
    {
        return (X, Y).GetHashCode();
    }
}

public class AStar
{
    private static readonly (int, int)[] Directions =
    {
        (0, 1), (1, 0), (0, -1), (-1, 0) 
    };
    
    public static List<Node> FindPath(int[,] grid, (int, int) start, (int, int) end)
    {
        var openSet = new List<Node>();
        var closedSet = new HashSet<Node>();
        var startNode = new Node(start.Item1, start.Item2);
        var endNode = new Node(end.Item1, end.Item2);
        
        openSet.Add(startNode);
        
        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(n => n.F).First();
            
            if (current.Equals(endNode))
                return ReconstructPath(current);
            
            openSet.Remove(current);
            closedSet.Add(current);
            
            foreach (var (dx, dy) in Directions)
            {
                int newX = current.X + dx, newY = current.Y + dy;
                if (!IsValid(grid, newX, newY) || closedSet.Contains(new Node(newX, newY)))
                    continue;
                
                var neighbor = new Node(newX, newY, current)
                {
                    G = current.G + 1,
                    H = Math.Abs(end.Item1 - newX) + Math.Abs(end.Item2 - newY)
                };
                
                var existingNode = openSet.FirstOrDefault(n => n.Equals(neighbor));
                if (existingNode == null || neighbor.G < existingNode.G)
                {
                    openSet.Add(neighbor);
                }
            }
        }
        return null;
    }
    
    private static bool IsValid(int[,] grid, int x, int y)
    {
        return x >= 0 && y >= 0 && x < grid.GetLength(0) && y < grid.GetLength(1) && grid[x, y] == 0;
    }
    
    private static List<Node> ReconstructPath(Node node)
    {
        var path = new List<Node>();
        while (node != null)
        {
            path.Add(node);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }
}

// Ejemplo de uso
class Program
{
    static void Main()
    {
        int[,] grid =
        {
            { 0, 0, 0, 0, 1 },
            { 1, 1, 0, 1, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0 }
        };
        
        var path = AStar.FindPath(grid, (0, 0), (4, 4));
        
        if (path != null)
        {
            foreach (var node in path)
                Console.WriteLine($"({node.X}, {node.Y})");
        }
        else
        {
            Console.WriteLine("No se encontró un camino.");
        }
    }
}
