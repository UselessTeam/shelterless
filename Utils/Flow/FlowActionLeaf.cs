using System;

namespace Flow
{
    public sealed class FlowActionLeaf : Flow
    {
        public Action Apply { get; private set; }
        public FlowActionLeaf(Action apply, string name = "") : base(name)
        {
            Apply = apply;
        }

        protected override void StartInternal()
        {
            Apply();
        }
    }
}