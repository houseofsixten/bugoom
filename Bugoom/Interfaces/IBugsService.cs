using Microsoft.AspNetCore.Mvc;

namespace Bugoom.Interfaces
{
    public interface IBugsService
    {
        Task<IEnumerable<Bug>> GetAll(int? bugId, int? createdByUserId, int? assignedToUserId, BugStatus? status);
        Task<Bug> Open(OpenBugView openBugView);
        Task<Bug> Assign(AssignBugView assignBugView);

    }
}
