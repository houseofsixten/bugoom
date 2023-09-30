using Bugoom.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bugoom
{
    public class BugsService : IBugsService
    {
        BuggingContext context;
        public BugsService(BuggingContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Bug>> GetAll(int? bugId, int? createdByUserId, int? assignedToUserId, BugStatus? status)
        {
            var bugs = await context.Bugs
                        .Where(b => (bugId == null || bugId == b.Id)
                                    && (createdByUserId == null || createdByUserId == b.CreatedByUserId)
                                    && (assignedToUserId == null || assignedToUserId == b.AssignedToUserId)
                                    && (status == null || status == b.Status))
                        .Include(c => c.Changes.OrderBy(i => i.CreatedAt)).ToListAsync();
            return bugs;
        }

        public async Task<Bug> Open(OpenBugView openBugView)
        {
            var openingUser = await context.Users.Where(u => u.Id == openBugView.OpenedByUserId).FirstOrDefaultAsync();

            if (openingUser != null
                && !string.IsNullOrWhiteSpace(openBugView.Title)
                && !string.IsNullOrWhiteSpace(openBugView.Title)
                && (openingUser.Role == UserRole.User || openingUser.Role == UserRole.Boss))
            {
                var newBug = new Bug
                {
                    CreatedByUserId = openingUser.Id,
                    Title = openBugView.Title,
                    Description = openBugView.Description,
                    Status = BugStatus.Open,
                    CreatedAt = DateTime.UtcNow
                };

                context.Bugs.Add(newBug);
                await context.SaveChangesAsync();

                var firstChange = new BugChange
                {
                    BugId = newBug.Id,
                    CreatedByUserId = openingUser.Id,
                    Description = $"Opened by {openingUser.UsernameAndId()}",
                    CreatedAt = DateTime.UtcNow
                };

                context.BugChanges.Add(firstChange);
                await context.SaveChangesAsync();

                return newBug;
            }
            else
            {
                throw new InvalidDataException("Invalid data for opening new bug ticket");
            }
        }

        public async Task<Bug> Assign(AssignBugView assignBugView)
        {
            var assignedBug = await context.Bugs.Where(u => u.Id == assignBugView.BugId).Include(c => c.Changes.OrderBy(i => i.CreatedAt)).FirstOrDefaultAsync();
            var assigningStaff = await context.Users.Where(u => u.Id == assignBugView.AssignedByStaffId).FirstOrDefaultAsync();
            var assignedStaff = await context.Users.Where(u => u.Id == assignBugView.AssignedToStaffId).FirstOrDefaultAsync();

            if (assignedBug != null
                && assigningStaff != null
                && assignedStaff != null
                && !string.IsNullOrWhiteSpace(assignBugView.Notes)
                && (assignedBug.Status == BugStatus.Open || assignedBug.Status == BugStatus.Assigned)
                && (assigningStaff.Role == UserRole.Staff || assigningStaff.Role == UserRole.Boss)
                && assignedStaff.Role == UserRole.Staff)
            {
                assignedBug.Status = BugStatus.Assigned;
                assignedBug.AssignedToUserId = assignedStaff.Id;

                var assignmentChange = new BugChange
                {
                    BugId = assignedBug.Id,
                    CreatedByUserId = assigningStaff.Id,
                    Description = $"Assigned by {assigningStaff.UsernameAndId()} to {assignedStaff.UsernameAndId()}",
                    Notes = assignBugView.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                assignedBug.Changes.Add(assignmentChange);
                await context.SaveChangesAsync();

                var newlyAssignedBug = await context.Bugs.Where(b => b.Id == assignedBug.Id).Include(c => c.Changes.OrderBy(i => i.CreatedAt)).FirstOrDefaultAsync();
                return newlyAssignedBug;
            }
            else
            {
                throw new InvalidDataException("Invalid data for assigning bug ticket to staff");
            }
        }


    }
}
