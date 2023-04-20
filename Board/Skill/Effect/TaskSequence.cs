using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class TaskSequence
{
    protected List<Func<Task>> callableTasks;
    protected List<Task> runningTasks = new List<Task>();
    protected int currentSubtask = 0;

    public TaskSequence(params Func<Task>[] tasks)
    {
        callableTasks = new List<Func<Task>>(tasks);
    }
    public TaskSequence(IEnumerable<Func<Task>> tasks)
    {
        callableTasks = new List<Func<Task>>(tasks);
    }
    public static TaskSequence Create<TContext>(TContext context, IEnumerable<Func<TContext, Task>> effects)
    {
        return new TaskSequence(effects.Select((effect) => (Func<Task>)(() => effect(context))));
    }
    public static TaskSequence Create<TContext>(TContext context, IEnumerable<EffectRule<TContext>> effects)
    {
        return new TaskSequence(effects.Select((effect) => (Func<Task>)(effect is null ? null : () => effect.ExecuteAsync(context))));
    }

    public int GetRemainingCount()
    {
        return callableTasks.Count - currentSubtask;
    }
    public bool HasRemaining()
    {
        return currentSubtask < callableTasks.Count;
    }

    public void StartNext()
    {
        if (!HasRemaining())
        {
            throw new IndexOutOfRangeException($"Already all subtasks (count: {callableTasks.Count}) had been consumed, but requesting a {currentSubtask}th one");
        }
        else
        {
            Func<Task> call = callableTasks[currentSubtask];
            if (call is not null)
            {
                Task subTask = call();
                if (subTask is not null)
                {
                    runningTasks.Add(subTask);
                }
            }
        }
        currentSubtask++;
    }

    public void TryStartNext()
    {
        if (HasRemaining())
        {
            StartNext();
        }
    }

    public void StartRemaining()
    {
        while (HasRemaining())
        {
            StartNext();
        }
    }

    public async Task Join()
    {
        StartRemaining();
        foreach (Task running in runningTasks)
        {
            await running;
        }
    }
}