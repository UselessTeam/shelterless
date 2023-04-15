using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class Board : TileMap
{
    public static readonly Vector2I INVALID_COORDS = new Vector2I(int.MaxValue, int.MaxValue);
    private Dictionary<Vector2I, HashSet<Pawn>> PawnByCoords = new Dictionary<Vector2I, HashSet<Pawn>>();
    private HashSet<Pawn> Pawns = new HashSet<Pawn>();

    internal void Register(Pawn pawn)
    {
        Pawns.Add(pawn);
        if (pawn.Coords != INVALID_COORDS)
        {
            GD.PrintErr($"Coords for pawn {pawn} weren't null {pawn.Coords}");
            pawn.SetCoords(INVALID_COORDS);
        }
        MovePawn(pawn, LocalToMap(pawn.Position));
    }

    internal void Unregister(Pawn pawn)
    {
        MovePawn(pawn, INVALID_COORDS);
        Pawns.Remove(pawn);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton
            && mouseButton.ButtonIndex == MouseButton.Left)
        {
            // TODO: Remove, test only
            MovePawn(Pawns.First(), LocalToMap(GetGlobalMousePosition()));
        }
    }

    public void MovePawn(Pawn pawn, Vector2I coords)
    {
        if (pawn.Coords == coords)
        {
            return;
        }
        if (pawn.Coords != INVALID_COORDS)
        {
            if (PawnByCoords[pawn.Coords].Count == 1)
            {
                PawnByCoords.Remove(pawn.Coords);
            }
            else
            {
                PawnByCoords[pawn.Coords].Remove(pawn);
            }
        }
        if (coords != INVALID_COORDS)
        {
            if (!PawnByCoords.ContainsKey(coords))
            {
                PawnByCoords[coords] = new HashSet<Pawn>();
            }
            PawnByCoords[coords].Add(pawn);
            pawn.SetCoords(coords);
        }
    }

    public IEnumerable<Pawn> GetPawnsAt(Vector2I coords)
    {
        if (PawnByCoords.ContainsKey(coords))
        {
            return PawnByCoords[coords];
        }
        else
        {
            return Enumerable.Empty<Pawn>();
        }
    }

    public Pawn GetFirstPawnAt(Vector2I coords)
    {
        if (PawnByCoords.ContainsKey(coords))
        {
            return PawnByCoords[coords].First();
        }
        else
        {
            return null;
        }
    }
}
