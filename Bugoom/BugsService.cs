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

        public async Task<Bug> GetStatus(int bugId, int createdByUserId)
        {
            var bugs = await GetAll(bugId, createdByUserId, null, null);
            var bugList = bugs.ToList();
            if (bugList.Count == 1)
            {
                return bugList[0];
            }
            else
            {
                throw new KeyNotFoundException("Could not find bug with that ID created by that user.");
            }
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
                throw new InvalidDataException("Invalid data for opening new bug ticket. Bug must be opened by a User or Boss.");
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
                if (newlyAssignedBug == null)
                {
                    throw new Exception("Error occurred when assigning bug.");
                }

                return newlyAssignedBug;
            }
            else
            {
                throw new InvalidDataException("Invalid data for assigning bug ticket to Staff. Bug must be assigned by Boss or Staff to one Staff.");
            }
        }

        public async Task<Bug> Comment(CommentView commentView)
        {
            var commentedBug = await context.Bugs.Where(u => u.Id == commentView.BugId).Include(c => c.Changes.OrderBy(i => i.CreatedAt)).FirstOrDefaultAsync();
            var commentingStaff = await context.Users.Where(u => u.Id == commentView.StaffId).FirstOrDefaultAsync();

            if (commentedBug != null
                && commentingStaff != null
                && !string.IsNullOrWhiteSpace(commentView.Notes)                
                && (commentingStaff.Role == UserRole.Staff || commentingStaff.Role == UserRole.Boss))
            {
                var commentingChange = new BugChange
                {
                    BugId = commentedBug.Id,
                    CreatedByUserId = commentingStaff.Id,
                    Description = $"Comment added by {commentingStaff.UsernameAndId()}",
                    Notes = commentView.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                commentedBug.Changes.Add(commentingChange);
                await context.SaveChangesAsync();

                var newlyCommentedBug = await context.Bugs.Where(b => b.Id == commentedBug.Id).Include(c => c.Changes.OrderBy(i => i.CreatedAt)).FirstOrDefaultAsync();
                if (newlyCommentedBug == null)
                {
                    throw new Exception("Error occurred when commenting on bug.");
                }

                return newlyCommentedBug;
            }
            else
            {
                throw new InvalidDataException("Invalid data for commenting on bug. Comment must be posted by a Staff or Boss.");
            }
        }

        public async Task<Bug> Fix(FixBugView fixBugView)
        {
            var fixingBug = await context.Bugs.Where(u => u.Id == fixBugView.BugId).Include(c => c.Changes.OrderBy(i => i.CreatedAt)).FirstOrDefaultAsync();            
            var fixingStaff = await context.Users.Where(u => u.Id == fixBugView.FixedByStaffId).FirstOrDefaultAsync();

            if (fixingBug != null
                && fixingStaff != null
                && !string.IsNullOrWhiteSpace(fixBugView.Notes)
                && fixingBug.Status == BugStatus.Assigned
                && fixingStaff.Role == UserRole.Staff
                && fixingBug.AssignedToUserId == fixingStaff.Id)
            {
                fixingBug.Status = BugStatus.Fixed;

                var fixingChange = new BugChange
                {
                    BugId = fixingBug.Id,
                    CreatedByUserId = fixingStaff.Id,
                    Description = $"Fixed by {fixingStaff.UsernameAndId()}",
                    Notes = fixBugView.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                fixingBug.Changes.Add(fixingChange);
                await context.SaveChangesAsync();

                var newlyFixedBug = await context.Bugs.Where(b => b.Id == fixingBug.Id).Include(c => c.Changes.OrderBy(i => i.CreatedAt)).FirstOrDefaultAsync();
                if (newlyFixedBug == null)
                {
                    throw new Exception("Error occurred when fixing bug.");
                }

                return newlyFixedBug;
            }
            else
            {
                throw new InvalidDataException("Invalid data for fixing bug. Bug must be fixed by the Staff to whom it is assigned.");
            }
        }

        public async Task<Bug> Close(CloseBugView closeBugView)
        {
            var closingBug = await context.Bugs.Where(u => u.Id == closeBugView.BugId).Include(c => c.Changes.OrderBy(i => i.CreatedAt)).FirstOrDefaultAsync();
            var closingUser = await context.Users.Where(u => u.Id == closeBugView.OpenedByUserId).FirstOrDefaultAsync();

            if (closingBug != null
                && closingUser != null
                && !string.IsNullOrWhiteSpace(closeBugView.Notes)
                && closingBug.Status == BugStatus.Fixed
                && closingBug.CreatedByUserId == closingUser.Id
                && (closingUser.Role == UserRole.User || closingUser.Role == UserRole.Boss))
            {
                closingBug.Status = BugStatus.Closed;

                var closingChange = new BugChange
                {
                    BugId = closingBug.Id,
                    CreatedByUserId = closingUser.Id,
                    Description = $"Closed by {closingUser.UsernameAndId()}",
                    Notes = closeBugView.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                closingBug.Changes.Add(closingChange);
                await context.SaveChangesAsync();

                var newlyFixedBug = await context.Bugs.Where(b => b.Id == closingBug.Id).Include(c => c.Changes.OrderBy(i => i.CreatedAt)).FirstOrDefaultAsync();
                if (newlyFixedBug == null)
                {
                    throw new Exception("Error occurred when closing bug.");
                }

                return newlyFixedBug;
            }
            else
            {
                throw new InvalidDataException("Invalid data for closing bug. Bug must be closed by the User or Boss who opened it.");
            }
        }


    }
}
