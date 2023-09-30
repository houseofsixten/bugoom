using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bugoom
{
    public class BugSearchView
    {
        [DefaultValue(null)]
        public int? BugId { get; set; }

        [DefaultValue(null)]
        public int? CreatedByUserId { get; set; }

        [DefaultValue(null)]
        public BugStatus Status { get; set; }
    }

    public class OpenBugView
    {
        public int OpenedByUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class AssignBugView
    {
        public int BugId { get; set; }
        public int AssignedByStaffId { get; set; }
        public int AssignedToStaffId { get; set; }        
        public string Notes { get; set; }
    }

    public static class UserExtensions
    {
        public static string UsernameAndId(this User user)        
        {
            return $"{user.Username} (user id: {user.Id})";
        }
    }
}
