namespace Flow
{
    public abstract class Flow
    {
        public string Name { get; private set; }
        public FlowNode Parent { get; private set; }
        internal FlowState State;
        protected Flow(string name = "") : this(null, name) { }
        internal Flow(FlowNode parent, string name = "", bool withParent = false)
        {
            Name = name;
            State = FlowState.READY;
            SetParent(parent, withParent);
        }
        public string FullName()
        {
            if (Parent is null)
            {
                return Name;
            }
            return $"{Parent.FullName()}/{Name}";
        }
        public void Start()
        {
            if (State != FlowState.READY)
            {
                throw new FlowException("Flow had already started");
            }
            State = FlowState.RUNNING;
            Parent?.ChildStart();
            StartInternal();
        }
        protected abstract void StartInternal();
        protected void Finish()
        {
            State = FlowState.CLOSED;
            Parent?.ChildFinish();
        }

        internal void SetParent(FlowNode parent, bool withParent = true)
        {
            if (Parent == parent && !withParent)
            {
                return;
            }
            if (Parent is not null)
            {
                throw new FlowException("Flow already had a parent");
            }
            Parent = parent;
            if (parent is null)
            {
                return;
            }
            if (State == FlowState.READY)
            {
                if (State != FlowState.READY)
                {
                    throw new FlowException("Child flow started before parent");
                }
                if (withParent)
                {
                    parent.Started += Start;
                }
            }
            else
            {
                if (withParent)
                {
                    AssertRunning();
                }
                switch (State)
                {
                    case FlowState.READY:
                        if (withParent)
                        {
                            Start();
                        }
                        break;
                    case FlowState.RUNNING:
                    case FlowState.CLOSING:
                        parent.ChildStart();
                        break;
                    case FlowState.CLOSED:
                    default:
                        break;
                }
            }
        }

        protected void AssertRunning()
        {
            if (!State.IsRunning())
            {
                throw new FlowException("Flow has already been closed");
            }
        }
    }
}