using HafizDemoAPI.ModelCDN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static HafizDemoAPI.Controllers.StatController;

namespace HafizDemoAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StatController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult GetStatByGroup(string statGroup)
        {
            using (CDNContext cdnCtxt = new())
            {
                var stats = cdnCtxt.StatTables.Where(stat => stat.StatGroup == statGroup).ToList();
                var sum = stats.Sum(d => d.StatValue);

                if (stats == null)  return NotFound();
                List<StatTable> statTables = new();
                stats.ForEach(stat =>
                {
                    statTables.Add(new StatTable { Name = stat.StatName, Value = stat.StatValue * 100 / sum });
                });

                return Ok(statTables);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetStatValByGroup(string statGroup)
        {
            using (CDNContext cdnCtxt = new())
            {
                var stats = cdnCtxt.StatTables.Where(stat => stat.StatGroup == statGroup).ToList();
                var sum = stats.Sum(d => d.StatValue);

                if (stats == null) return NotFound();
                List<StatTableUpdate> statTables = new();
                stats.ForEach(stat =>
                {
                    statTables.Add(new StatTableUpdate {Id = stat.Id, Name = stat.StatName, Value = stat.StatValue});
                });

                return Ok(statTables);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateStatValueAsync (string Group , List<StatTableUpdate> statUpdate, IHubContext<ChatHub, iChatClient> context)
        {
            Int32 updatedRow = 0;
            statUpdate.ForEach(stat =>
            {
                using (CDNContext cdnCtxt = new())
                {
                    updatedRow = cdnCtxt.StatTables.Where(u => u.Id == stat.Id).ExecuteUpdate(setters => setters
                    //.SetProperty(s => s.StatGroup,"State")
                    //.SetProperty(s => s.StatName, stat.Name)
                    .SetProperty(s => s.StatValue, stat.Value)
                    );

                }
            });
            if (updatedRow > 0)
            {
                await NotifyClient(Group,context);
            }
            return Ok(new UpdateStatus {  HasError = false , ReturnMsg = "Success"} );
        }

        internal async Task<bool> NotifyClient(string Group,IHubContext<ChatHub, iChatClient> context)
        {
            await context.Clients.All.NotifyStatUpdate(Group);
            return true;
        }

        public class UpdateStatus
        {
            public bool HasError { get; set; }
            public string ReturnMsg { get; set; }
        }
        public class StatTableUpdate : StatTable
        {
            public Int32 Id { get; set; }
        }

        public class StatTable
        {
            public string? Name { get; set; }
            public Int32? Value { get; set; }
        }
    }
}
