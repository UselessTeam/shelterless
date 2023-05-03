using System;
using System.Collections.Generic;
using Godot;

public abstract class AsyncTask{
    event Action Finished;

    public abstract void Start();

    public void Then(AsyncTask task){
        Finished+=()=> task.Start();
    }
    public abstract bool IsFinished();
}

public class AnimationAsyncTask : AsyncTask{
    public AnimationAsyncTask(AnimationPlayer animation){}

    public override bool IsFinished()
    {
        throw new NotImplementedException();
    }

    public override void Start()
    {
        throw new NotImplementedException();
    }
}

public class CompositeTask : AsyncTask{
    List<AsyncTask> tasks;

    public override bool IsFinished()
    {
        throw new NotImplementedException();
    }

    public override void Start()
    {
        throw new NotImplementedException();
    }
}