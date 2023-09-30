using Microsoft.AspNetCore.Mvc;

namespace Bugoom.Interfaces
{
    public interface IBugsService
    {
        Task<IEnumerable<Bug>> GetAll(int? bugId, int? createdByUserId, int? assignedToUserId, BugStatus? status);
        Task<Bug> GetStatus(int bugId, int createdByUserId);
        Task<Bug> Open(OpenBugView openBugView);
        Task<Bug> Assign(AssignBugView assignBugView);
        Task<Bug> Comment(CommentView commentView);
        Task<Bug> Fix(FixBugView fixBugView);
        Task<Bug> Close(CloseBugView closeBugView);

    }
}
