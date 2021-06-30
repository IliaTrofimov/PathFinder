namespace PathFinder.GraphControls
{
    /// <summary>
    /// Represents selection options for VertexControls.
    /// </summary>
    public enum NodeSelection { A, B, None }

    public class SelectionResetedArgs
    {
        public int Id;
        public NodeSelection newSelection;

        public SelectionResetedArgs(int id, NodeSelection newSelection)
        {
            Id = id;
            this.newSelection = newSelection;
        }
    }
    public class SelectionChangedArgs : SelectionResetedArgs
    {
        public NodeSelection lastSelection;

        public SelectionChangedArgs(int id, NodeSelection lastSelection, NodeSelection newSelection) : base(id, newSelection)
        {
            this.lastSelection = lastSelection;
        }
    }
}
