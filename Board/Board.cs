using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class Board : Node
{
    public static readonly Vector2I INVALID_COORDS = new Vector2I(int.MaxValue, int.MaxValue);
    public TileMap TileMap { get; private set; }
    private Dictionary<Vector2I, HashSet<Pawn>> PawnByCoords = new Dictionary<Vector2I, HashSet<Pawn>>();
    private HashSet<Pawn> Pawns = new HashSet<Pawn>();

    internal void Register(Pawn pawn)
    {
        Pawns.Add(pawn);
    }

    internal void Unregister(Pawn pawn)
    {
        MovePawn(pawn, INVALID_COORDS);
        Pawns.Remove(pawn);
    }

    public void MovePawn(Pawn pawn, Vector2I coords)
    {
        if (pawn.Coords != INVALID_COORDS)
        {
            if (PawnByCoords[coords].Count == 1)
            {
                PawnByCoords.Remove(coords);
            }
            else
            {
                PawnByCoords[coords].Remove(pawn);
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
