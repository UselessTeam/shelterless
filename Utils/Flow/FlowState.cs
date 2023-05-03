namespace Flow
{
    public enum FlowState : short
    {
        READY,
        RUNNING,
        CLOSING,
        CLOSED,
    }

    public static class FlowStateExtension
    {
        public static bool IsRunning(this FlowState state)
        {
            return state == FlowState.RUNNING;
        }
        public static bool IsClosing(this FlowState state)
        {
            return state == FlowState.CLOSING;
        }
        public static bool IsOpened(this FlowState state)
        {
            return state == FlowState.RUNNING || state == FlowState.CLOSING;
        }
    }
}