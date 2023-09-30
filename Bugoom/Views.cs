using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public int OpenedByUserId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }

    public class AssignBugView
    {
        [Required]
        public int BugId { get; set; }

        [Required]
        public int AssignedByStaffId { get; set; }

        [Required]
        public int AssignedToStaffId { get; set; }

        [Required]
        public string Notes { get; set; }
    }

    public class FixBugView
    {
        [Required]
        public int BugId { get; set; }

        [Required]
        public int FixedByStaffId { get; set; }

        [Required]
        public string Notes { get; set; }
    }

    public class CommentView
    {
        [Required]
        public int BugId { get; set; }

        [Required]
        public int StaffId { get; set; }

        [Required]
        public string Notes { get; set; }
    }

    public class CloseBugView
    {
        [Required]
        public int BugId { get; set; }

        [Required]
        public int OpenedByUserId { get; set; }

        [Required]
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
