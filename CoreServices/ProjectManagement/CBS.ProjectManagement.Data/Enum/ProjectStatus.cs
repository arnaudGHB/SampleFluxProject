namespace CBS.ProjectManagement.Data.Enum
{
    /// <summary>
    /// Represents the possible statuses of a project.
    /// </summary>
    public enum ProjectStatus
    {
        /// <summary>
        /// Project is currently active and in progress.
        /// </summary>
        Active,
        
        /// <summary>
        /// Project has been completed and closed.
        /// </summary>
        Closed,
        
        /// <summary>
        /// Project has been archived and is no longer active.
        /// </summary>
        Archived
    }
}
