using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using EventManagementSystem.Data;
using EventManagementSystem.Models;
using System.Text;
using System.IO;
using Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace EventManagementSystem.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly EMSDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(EMSDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var guests = _context.GuestLists.Include(g => g.CityTownVillage)
                                            .Select(g => new Guestmodel
                                            {
                                                GuestId = g.GuestId,
                                                AppID = g.AppID,
                                                GuestName = g.GuestName,
                                                GuestGender = g.GuestGender,
                                                CityTownVillageID = g.CityTownVillageID,
                                                ListCityTownVillage = g.CityTownVillage != null ? new List<ListCityTownVillagemodel> { new ListCityTownVillagemodel { CityTownVillageName = g.CityTownVillage.CityTownVillageName } } : null
                                            }).AsQueryable();
            var paginatedList = await PaginatedList<Guestmodel>.CreateAsync(guests, pageNumber, pageSize);
            return View(paginatedList);
        }
        [HttpGet]
        public IActionResult GuestEventsDetails(int id)
        {
            var guest = _context.GuestLists.FirstOrDefault(g => g.GuestId == id);
            if (guest == null)
            {
                return NotFound();
            }
            var guestModel = new Guestmodel
            {
                GuestId = guest.GuestId,
                GuestName = guest.GuestName,
                GuestGender = guest.GuestGender,
                CityTownVillageID = guest.CityTownVillageID,
                ListCityTownVillage = _context.CityTownVillages.Select(ctv => new ListCityTownVillagemodel
                {
                    CityTownVillageID = ctv.CityTownVillageID,
                    CityTownVillageName = ctv.CityTownVillageName
                }).ToList(),

            };
            var guidString = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.UserGUID);
            var guid = Guid.Parse(guidString);
            var appMaster = _context.AppMasters.FirstOrDefault(a => a.AppsGUID == guid);
            var query = from evm in _context.EventMasters
                        join gst in _context.GuestLists on evm.AppID equals gst.AppID
                        join egm in _context.EventGuestMapping on new { gst.GuestId, evm.EventId } equals new { egm.GuestId, egm.EventId } into egmGroup
                        from egm in egmGroup.DefaultIfEmpty()
                        where evm.AppID == appMaster.AppID  // Add filter condition for evm.AppID
                         && gst.GuestId == guestModel.GuestId // Add filter condition for egm.GuestId
                        select new EventGuestMappingmodel
                        {
                            EventId = evm.EventId,
                            AppID = evm.AppID,
                            EventName = evm.EventName,
                            EventDate = evm.EventDate,
                            IsAttended = egm != null ? egm.IsAttended : false,
                            EventGiftDetails = egm != null ? egm.EventGiftDetails : null
                        };

            var result = query.ToList();
            guestModel.ListEventGuestMappingmodel = result;

            return View(guestModel);
        }

        public async Task<IActionResult> ExportGuests()
        {
            var guests = await _context.GuestLists.Include(g => g.CityTownVillage).ToListAsync();
            var csv = new StringBuilder();
            csv.AppendLine("Guest Name,Guest Gender,City/Town/Village");

            foreach (var guest in guests)
            {
                var cityTownVillageName = guest.CityTownVillage?.CityTownVillageName ?? string.Empty;
                csv.AppendLine($"{guest.GuestName},{guest.GuestGender},{cityTownVillageName}");
            }

            var fileName = "guests.csv";
            var mimeType = "text/csv";
            var fileBytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(fileBytes, mimeType, fileName);
        }
    }
}