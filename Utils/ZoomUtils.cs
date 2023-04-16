using Godot;

public static class ZoomUtils
{
    public static void FixZoom(this Node2D node, float neutralScale = 1f)
    {
        Vector2 zoom = node.GetViewport().GetCamera2D().Zoom;
        node.Scale = new Vector2(neutralScale / zoom.X, neutralScale / zoom.Y);
    }
}