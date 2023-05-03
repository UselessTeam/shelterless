using System;
using System.Collections.Generic;
using Godot;

namespace Flow
{
    public sealed class FlowNode : Flow
    {
        public int OpenChildCount { get; private set; } = 0;
        internal Queue<Flow> queue = null;
        public event Action Started;
        public FlowNode(string name = "") : base(name) { }
        private FlowNode(FlowNode parent, string name) : base(null, name) { }
        protected override void StartInternal()
        {
            Started();
        }
        public FlowNode Fork(string Name = "")
        {
            AssertRunning();
            return new FlowNode(this, Name);
        }
        public static FlowNode Spawn(string name = "")
        {
            FlowNode flow = new FlowNode(null, name);
            flow.Start();
            return flow;
        }
        public FlowNode With(Flow subFlow)
        {
            if (queue is not null)
            {
                throw new FlowException("Flow's `With` called after `Then`, call it explicitly on child or before on parent");
            }
            subFlow.SetParent(this, withParent: true);
            return this;
        }
        public FlowNode Then(Flow subFlow)
        {
            subFlow.SetParent(this, withParent: false);
            if (queue is null)
            {
                queue = new Queue<Flow>();
            }
            queue.Enqueue(subFlow);
            return this;
        }
        public void Join()
        {
            AssertRunning();
            State = FlowState.CLOSING;
            TryHook();
        }
        public void Run()
        {
            Start();
            Join();
        }
        internal void ChildStart()
        {
            AssertRunning();
            OpenChildCount++;
        }
        internal void ChildFinish()
        {
            OpenChildCount--;
            TryHook();
        }
        private void TryHook()
        {
            if (OpenChildCount == 0 && State.IsClosing())
            {
                if (queue is null || queue.Count == 0)
                {
                    Finish();
                }
                else
                {
                    Flow next = queue.Dequeue();
                    next.Start();
                }
            }
        }
    }
}