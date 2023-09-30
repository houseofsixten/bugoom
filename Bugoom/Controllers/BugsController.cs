using System.ComponentModel.DataAnnotations;
using Bugoom.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bugoom.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BugsController : ControllerBase
    {
        private readonly ILogger<BugsController> _logger;
        private readonly IBugsService bugsService;

        public BugsController(ILogger<BugsController> logger, IBugsService bugsService)
        {
            _logger = logger;
            this.bugsService = bugsService;
        }

        [HttpGet(Name = "GetAllBugs")]
        public async Task<IEnumerable<Bug>> GetAll([FromQuery] int? bugId = null, [FromQuery] int? createdByUserId = null, [FromQuery] int? assignedToUserId = null, [FromQuery] BugStatus? status = null)
        {
            var allBugs = await bugsService.GetAll(bugId, createdByUserId, assignedToUserId, status);
            return allBugs;
        }

        [HttpGet(Name = "GetStatus")]
        public async Task<Bug> GetStatus([FromQuery][Required] int bugId, [FromQuery][Required] int createdByUserId)
        {
            var myBug = await bugsService.GetStatus(bugId, createdByUserId);
            return myBug;
        }

        [HttpPost(Name = "Open")]
        public async Task<Bug> Open([FromBody] OpenBugView openBugView)
        {
            var newlyOpenedBug = await bugsService.Open(openBugView);
            return newlyOpenedBug;
        }

        [HttpPost(Name = "Assign")]
        public async Task<Bug> Assign([FromBody] AssignBugView assignBugView)
        {
            var newlyAssignedBug = await bugsService.Assign(assignBugView);
            return newlyAssignedBug;
        }

        [HttpPost(Name = "Fix")]
        public async Task<Bug> Fix([FromBody] FixBugView fixBugView)
        {
            var newlyFixedBug = await bugsService.Fix(fixBugView);
            return newlyFixedBug;
        }

        [HttpPost(Name = "Comment")]
        public async Task<Bug> Comment([FromBody] CommentView commentView)
        {
            var newlyCommentedBug = await bugsService.Comment(commentView);
            return newlyCommentedBug;
        }

        [HttpPost(Name = "Close")]
        public async Task<Bug> Close([FromBody] CloseBugView closeBugView)
        {
            var newlyClosedBug = await bugsService.Close(closeBugView);
            return newlyClosedBug;
        }
    }
}