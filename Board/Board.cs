using Godot;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Board : TileMap
{
    private Dictionary<Vector2I, HashSet<Pawn>> PawnByCoords = new Dictionary<Vector2I, HashSet<Pawn>>();
    private HashSet<Pawn> Pawns = new HashSet<Pawn>();

    internal void Register(Pawn pawn)
    {
        Pawns.Add(pawn);
        if (pawn.Coords.IsValid())
        {
            GD.PrintErr($"Coords for pawn {pawn} weren't null {pawn.Coords}");
            pawn.OverrideCoords(VectorUtils.INVALID_VECTOR2I);
        }
        MovePawn(pawn, LocalToMap(pawn.Position));
    }

    internal void Unregister(Pawn pawn)
    {
        MovePawn(pawn, VectorUtils.INVALID_VECTOR2I);
        Pawns.Remove(pawn);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Action"))
        {
            Vector2I coords = LocalToMap(GetGlobalMousePosition());
            GUI.Main?.MouseOnTile(this, coords, GetGlobalMousePosition(), true);
            GetViewport().SetInputAsHandled();
        }
        if (@event is InputEventMouseMotion)
        {
            Vector2I coords = LocalToMap(GetGlobalMousePosition());
            GUI.Main?.MouseOnTile(this, coords, GetGlobalMousePosition(), false);
            GetViewport().SetInputAsHandled();
        }
    }

    public override void _Ready()
    {
        base._Ready();
        RunningGame = RunGame();
    }

    public bool Busy;
    private Task RunningGame;
    private async Task RunGame()
    {
        GD.Print("Running Game");
        while (true)
        {
            GD.Print("Player turn");
            await RunPlayerTurn();
            GD.Print("Monster turn");
            await RunMonsterTurn();
        }
    }
    public async Task RunPlayerTurn()
    {
        Busy = true;
        foreach (Pawn pawn in GetPawnsWith<PlayerComponent>())
        {
            await GUI.Main.ControlPawn(Player);
        }
        Busy = false;
    }

    public async Task RunMonsterTurn()
    {
        Busy = true;
        foreach (Pawn pawn in GetPawnsWith<MonsterAIComponent>())
        {
            await pawn.Get<MonsterAIComponent>().RunTurn();
        }
        Busy = false;
    }

    private static List<Vector2I> BuildPath(Dictionary<Vector2I, Vector2I> steps, Vector2I begin, Vector2I end)
    {
        List<Vector2I> path = new List<Vector2I>();
        Vector2I step = end;
        while (step != begin)
        {
            path.Insert(0, step);
            step = steps[end];
        }
        return path;
    }
    public List<Vector2I> Pathfind(Vector2I origin, Vector2I target, bool nextTo = true)
    {
        // TODO: Use A* once we have time to optimize
        Dictionary<Vector2I, Vector2I> reached = new Dictionary<Vector2I, Vector2I> {
            {origin, origin}
        };
        Queue<Vector2I> exploring = new Queue<Vector2I>();
        exploring.Enqueue(origin);
        while (exploring.Count > 0)
        {
            Vector2I tile = exploring.Dequeue();
            foreach (Vector2I neighbor in tile.Neighbors())
            {
                if (Walkable(neighbor) && !reached.ContainsKey(neighbor))
                {
                    reached[neighbor] = tile;
                    if (nextTo)
                    {
                        if (neighbor.Distance(target) == 1)
                        {
                            return BuildPath(reached, origin, neighbor);
                        }
                    }
                    else if (neighbor == target)
                    {
                        return BuildPath(reached, origin, target);
                    }
                    exploring.Enqueue(neighbor);
                }
            }
        }
        return null;
    }

    public bool Exists(Vector2I coords)
    {
        return GetCellSourceId(0, coords) >= 0;
    }

    public bool Walkable(Vector2I coords)
    {
        return Exists(coords) && GetFirstPawnAt(coords) == null;
    }

    public void MovePawn(Pawn pawn, Vector2I coords)
    {
        if (pawn.Coords == coords)
        {
            return;
        }
        if (pawn.Coords.IsValid())
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
        if (coords.IsValid())
        {
            if (!PawnByCoords.ContainsKey(coords))
            {
                PawnByCoords[coords] = new HashSet<Pawn>();
            }
            PawnByCoords[coords].Add(pawn);
            pawn.OverrideCoords(coords);
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

    public IEnumerable<Pawn> GetPawnsWith<T>() where T : Component
    {
        foreach (Pawn pawn in Pawns)
        {
            if (pawn.Has<T>())
            {
                yield return pawn;
            }
        }
    }

    public Pawn GetFirstPawnWith<T>() where T : Component
    {
        foreach (Pawn pawn in Pawns)
        {
            if (pawn.Has<T>())
            {
                return pawn;
            }
        }
        return null;
    }

    public Pawn Player => PlayerComponent.Main.Pawn;
}

public static class VectorUtils
{
    public static readonly Vector2I INVALID_VECTOR2I = new Vector2I(int.MaxValue, int.MaxValue);
    public enum Direction : byte
    {
        NONE,
        SE,
        E,
        NE,
        NW,
        W,
        SW,
    }

    public static readonly Direction[] Directions = {
        Direction.SE,
        Direction.E,
        Direction.NE,
        Direction.NW,
        Direction.W,
        Direction.SW,
    };

    public static Vector2I ToVector2I(this Direction direction)
    {
        return direction switch
        {
            Direction.NONE => Vector2I.Zero,
            Direction.SE => new Vector2I(0, 1),
            Direction.E => new Vector2I(1, 0),
            Direction.NE => new Vector2I(1, -1),
            Direction.NW => new Vector2I(0, -1),
            Direction.W => new Vector2I(-1, 0),
            Direction.SW => new Vector2I(-1, 1),
            _ => INVALID_VECTOR2I,
        };
    }
    public static IEnumerable<Vector2I> Neighbors(this Vector2I center)
    {
        foreach (Direction direction in Directions)
        {
            yield return center + direction.ToVector2I();
        }
    }
    public static bool IsValid(this Vector2I vector)
    {
        return vector != INVALID_VECTOR2I;
    }
    public static bool IsInvalid(this Vector2I vector)
    {
        return vector == INVALID_VECTOR2I;
    }

    public static int Magnitude(this Vector2I vector)
    {
        return Mathf.Max(Mathf.Max(Mathf.Abs(vector.X), Mathf.Abs(vector.Y)), Mathf.Abs(vector.X + vector.Y));
    }

    public static int Distance(this Vector2I vector, Vector2I other)
    {
        return (vector - other).Magnitude();
    }

    public static Sign SideTowards(this Vector2I origin, Vector2I target)
    {
        Vector2I delta = (target - origin);
        return (2 * delta.X + delta.Y).Sign();
    }
    public static Sign Sign(this int value)
    {
        if (value > 0)
            return global::Sign.POSITIVE;
        else if (value < 0)
            return global::Sign.NEGATIVE;
        return global::Sign.NEUTRAL;
    }
    public static Sign Sign(this float value)
    {
        if (value > 0)
            return global::Sign.POSITIVE;
        else if (value < 0)
            return global::Sign.NEGATIVE;
        return global::Sign.NEUTRAL;
    }
}

public enum Sign : sbyte
{
    NEGATIVE = -1,
    NEUTRAL = 0,
    POSITIVE = 1,
    LEFT = NEGATIVE,
    RIGHT = POSITIVE,
}