using Godot;

public static class NodeExtension
{
    public static T GetAncestor<T>(this Node node) where T : Node
    {
        Node ancestor = node.GetParent();
        while (ancestor is not null)
        {
            if (ancestor is T typedAncestor)
            {
                return typedAncestor;
            }
            ancestor = ancestor.GetParent();
        }
        return null;
    }
}