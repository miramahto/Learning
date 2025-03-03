using EventManagementSystem.Data;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace EventManagementSystem.Controllers
{
    [Authorize(Roles = "admin")] // Restrict access to admin users
    public class AdminController : Controller
    {
        private readonly EMSDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(EMSDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Countries()
        {
            var countries = _context.Countries.ToList();
            var countryModels = countries.Select(c => new Countrymodel
            {
                CountryId = c.CountryId,
                CountryName = c.CountryName
            }).ToList();
            return View(countryModels);
        }

        [HttpGet]
        public IActionResult CreateCountry()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountry(Countrymodel country)
        {
            if (ModelState.IsValid)
            {
                var existingCountry = _context.Countries.FirstOrDefault(c => c.CountryName == country.CountryName);
                if (existingCountry != null)
                {
                    ModelState.AddModelError("CountryName", "Country name already exists.");
                    return View(country);
                }

                var userEmail = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.UserEmail);
                if (userEmail == null)
                {
                    return BadRequest("User is not available in the session.");
                }

                var CreateCountry = new Country
                {
                    CountryName = country.CountryName,
                    CreatedBy = userEmail,
                    CreatedDate = DateTime.Now
                };
                _context.Countries.Add(CreateCountry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Countries));
            }
            return View(country);
        }

        [HttpGet]
        public IActionResult EditCountry(int id)
        {
            var country = _context.Countries.FirstOrDefault(c => c.CountryId == id);
            if (country == null)
            {
                return NotFound();
            }
            var countryModel = new Countrymodel
            {
                CountryId = country.CountryId,
                CountryName = country.CountryName
            };
            return View(countryModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditCountry(Countrymodel Editcountry)
        {
            if (ModelState.IsValid)
            {
                var country = _context.Countries.FirstOrDefault(c => c.CountryId == Editcountry.CountryId);
                if (country == null)
                {
                    return NotFound();
                }

                var existingCountry = _context.Countries.FirstOrDefault(c => c.CountryName == Editcountry.CountryName && c.CountryId != Editcountry.CountryId);
                if (existingCountry != null)
                {
                    ModelState.AddModelError("CountryName", "Country name already exists.");
                    return View(Editcountry);
                }

                country.CountryName = Editcountry.CountryName;
                _context.Countries.Update(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Countries));
            }
            return View(Editcountry);
        }

        [HttpGet]
        public IActionResult DeleteCountry(int id)
        {
            var country = _context.Countries.FirstOrDefault(c => c.CountryId == id);
            if (country == null)
            {
                return NotFound();
            }
            var countryModel = new Countrymodel
            {
                CountryId = country.CountryId,
                CountryName = country.CountryName
            };
            return View(countryModel);
        }

        [HttpPost, ActionName("DeleteCountry")]
        public async Task<IActionResult> DeleteCountryConfirmed(int id)
        {
            var country = _context.Countries.FirstOrDefault(c => c.CountryId == id);
            if (country == null)
            {
                return NotFound();
            }
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Countries));
        }

        #region Manage States
        public IActionResult States()
        {
            var states = _context.States.Include(s => s.Country).ToList();
            var stateModels = states.Select(s => new Statemodel
            {
                StateId = s.StateId,
                StateName = s.StateName,
                CountryId = s.CountryId,
                CountryName = s.Country.CountryName
            }).ToList();
            return View(stateModels);
        }

        [HttpGet]
        public IActionResult CreateState()
        {
            ViewBag.Countries = new SelectList(_context.Countries, "CountryId", "CountryName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateState(Statemodel state)
        {
            if (ModelState.IsValid)
            {
                var existingState = _context.States.FirstOrDefault(s => s.StateName == state.StateName && s.CountryId == state.CountryId);
                if (existingState != null)
                {
                    ModelState.AddModelError("StateName", "State name already exists in the selected country.");
                    ViewBag.Countries = new SelectList(_context.Countries, "CountryId", "CountryName");
                    return View(state);
                }

                var userEmail = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.UserEmail);
                if (userEmail == null)
                {
                    return BadRequest("User is not available in the session.");
                }

                var CreateState = new State
                {
                    StateName = state.StateName,
                    CountryId = state.CountryId,
                    CreatedBy = userEmail,
                    CreatedDate = DateTime.Now
                };
                _context.States.Add(CreateState);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(States));
            }
            else
            {
                // Log the model state errors for debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                // You can log these errors to a file or a logging service
                // For example: _logger.LogError("ModelState errors: {Errors}", string.Join(", ", errors));
            }
            ViewBag.Countries = new SelectList(_context.Countries, "CountryId", "CountryName");
            return View(state);
        }
        [HttpGet]
        public IActionResult EditState(int id)
        {
            var state = _context.States.FirstOrDefault(s => s.StateId == id);
            if (state == null)
            {
                return NotFound();
            }
            var stateModel = new Statemodel
            {
                StateId = state.StateId,
                StateName = state.StateName,
                CountryId = state.CountryId
            };
            ViewBag.Countries = new SelectList(_context.Countries, "CountryId", "CountryName", state.CountryId);
            return View(stateModel);
        }



        [HttpPost]
        public async Task<IActionResult> EditState(Statemodel Editstate)
        {
            if (ModelState.IsValid)
            {
                var state = _context.States.FirstOrDefault(s => s.StateId == Editstate.StateId);
                if (state == null)
                {
                    return NotFound();
                }
                // Check if the state name already exists in the same country
                var existingState = _context.States.FirstOrDefault(s => s.StateName == Editstate.StateName && s.CountryId == Editstate.CountryId && s.StateId != Editstate.StateId);
                if (existingState != null)
                {
                    ModelState.AddModelError("StateName", "State name already exists in the selected country.");
                    ViewBag.Countries = new SelectList(_context.Countries, "CountryId", "CountryName", Editstate.CountryId);
                    return View(Editstate);
                }

                state.StateName = Editstate.StateName;
                state.CountryId = Editstate.CountryId;
                _context.States.Update(state);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(States));
            }
            ViewBag.Countries = new SelectList(_context.Countries, "CountryId", "CountryName", Editstate.CountryId);
            return View(Editstate);
        }


        [HttpGet]
        public IActionResult DeleteState(int id)
        {
            var state = _context.States.FirstOrDefault(s => s.StateId == id);
            if (state == null)
            {
                return NotFound();
            }
            var stateModel = new Statemodel
            {
                StateId = state.StateId,
                StateName = state.StateName,
                CountryId = state.CountryId
            };
            return View(stateModel);
        }

        [HttpPost, ActionName("DeleteState")]
        public async Task<IActionResult> DeleteStateConfirmed(int id)
        {
            var state = _context.States.FirstOrDefault(s => s.StateId == id);
            if (state == null)
            {
                return NotFound();
            }
            _context.States.Remove(state);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(States));
        }


        #endregion
        #region Manage Districts
        public IActionResult Districts()
        {
            var districts = _context.Districts.Include(d => d.State).ToList();
            var districtModels = districts.Select(d => new Districtmodel
            {
                DistrictId = d.DistrictId,
                DistrictName = d.DistrictName,
                StateId = d.StateId,
                StateName = d.State.StateName
            }).ToList();
            return View(districtModels);
        }

        [HttpGet]
        public IActionResult CreateDistrict()
        {
            ViewBag.States = new SelectList(_context.States, "StateId", "StateName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDistrict(Districtmodel district)
        {
            if (ModelState.IsValid)
            {
                var existingDistrict = _context.Districts.FirstOrDefault(d => d.DistrictName == district.DistrictName && d.StateId == district.StateId);
                if (existingDistrict != null)
                {
                    ModelState.AddModelError("DistrictName", "District name already exists in the selected state.");
                    ViewBag.States = new SelectList(_context.States, "StateId", "StateName");
                    return View(district);
                }

                var userEmail = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.UserEmail);
                if (userEmail == null)
                {
                    return BadRequest("User is not available in the session.");
                }

                var CreateDistrict = new District
                {
                    DistrictName = district.DistrictName,
                    StateId = district.StateId,
                    CreatedBy = userEmail,
                    CreatedDate = DateTime.Now
                };
                _context.Districts.Add(CreateDistrict);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Districts));
            }
            ViewBag.States = new SelectList(_context.States, "StateId", "StateName");
            return View(district);
        }

        [HttpGet]
        public IActionResult EditDistrict(int id)
        {
            var district = _context.Districts.FirstOrDefault(d => d.DistrictId == id);
            if (district == null)
            {
                return NotFound();
            }
            var districtModel = new Districtmodel
            {
                DistrictId = district.DistrictId,
                DistrictName = district.DistrictName,
                StateId = district.StateId
            };
            ViewBag.States = new SelectList(_context.States, "StateId", "StateName", district.StateId);
            return View(districtModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditDistrict(Districtmodel Editdistrict)
        {
            if (ModelState.IsValid)
            {
                var district = _context.Districts.FirstOrDefault(d => d.DistrictId == Editdistrict.DistrictId);
                if (district == null)
                {
                    return NotFound();
                }

                var existingDistrict = _context.Districts.FirstOrDefault(d => d.DistrictName == Editdistrict.DistrictName && d.StateId == Editdistrict.StateId && d.DistrictId != Editdistrict.DistrictId);
                if (existingDistrict != null)
                {
                    ModelState.AddModelError("DistrictName", "District name already exists in the selected state.");
                    ViewBag.States = new SelectList(_context.States, "StateId", "StateName", Editdistrict.StateId);
                    return View(Editdistrict);
                }

                district.DistrictName = Editdistrict.DistrictName;
                district.StateId = Editdistrict.StateId;
                _context.Districts.Update(district);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Districts));
            }
            ViewBag.States = new SelectList(_context.States, "StateId", "StateName", Editdistrict.StateId);
            return View(Editdistrict);
        }

        [HttpGet]
        public IActionResult DeleteDistrict(int id)
        {
            var district = _context.Districts.FirstOrDefault(d => d.DistrictId == id);
            if (district == null)
            {
                return NotFound();
            }
            var districtModel = new Districtmodel
            {
                DistrictId = district.DistrictId,
                DistrictName = district.DistrictName,
                StateId = district.StateId
            };
            return View(districtModel);
        }

        [HttpPost, ActionName("DeleteDistrict")]
        public async Task<IActionResult> DeleteDistrictConfirmed(int id)
        {
            var district = _context.Districts.FirstOrDefault(d => d.DistrictId == id);
            if (district == null)
            {
                return NotFound();
            }
            _context.Districts.Remove(district);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Districts));
        }
        #endregion
        #region Manage CityTownVillages
        [HttpGet]
        //public JsonResult GetDistrictsByState(int stateId)
        //{
        //    var districts = _context.Districts
        //        .Where(d => d.StateId == stateId)
        //        .Select(d => new { d.DistrictId, d.DistrictName })
        //        .ToList();
        //    return Json(districts);
        //}
        public IActionResult CityTownVillages()
        {
            var cityTownVillages = _context.CityTownVillages.Include(c => c.District).ThenInclude(d => d.State).ToList();
            var cityTownVillageModels = cityTownVillages.Select(c => new CityTownVillagemodel
            {
                CityTownVillageID = c.CityTownVillageID,
                CityTownVillageName = c.CityTownVillageName,
                DistrictId = c.DistrictId,
                DistrictName = c.District.DistrictName,
                StateId = c.District.StateId,
                StateName = c.District.State.StateName
            }).ToList();
            return View(cityTownVillageModels);
        }



        [HttpGet]
        public IActionResult CreateCityTownVillage()
        {
            ViewBag.States = new SelectList(_context.States, "StateId", "StateName");
            ViewBag.Districts = new SelectList(_context.Districts, "DistrictId", "DistrictName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCityTownVillage(CityTownVillagemodel cityTownVillage)
        {
            if (ModelState.IsValid)
            {
                var existingCityTownVillage = _context.CityTownVillages.FirstOrDefault(c => c.CityTownVillageName == cityTownVillage.CityTownVillageName && c.DistrictId == cityTownVillage.DistrictId);
                if (existingCityTownVillage != null)
                {
                    ModelState.AddModelError("CityTownVillageName", "City/Town/Village name already exists in the selected district.");
                    ViewBag.States = new SelectList(_context.States, "StateId", "StateName");
                    ViewBag.Districts = new SelectList(_context.Districts, "DistrictId", "DistrictName");
                    return View(cityTownVillage);
                }

                var userEmail = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.UserEmail);
                if (userEmail == null)
                {
                    return BadRequest("User is not available in the session.");
                }

                var CreateCityTownVillage = new CityTownVillage
                {
                    CityTownVillageName = cityTownVillage.CityTownVillageName,
                    DistrictId = cityTownVillage.DistrictId,
                    CreatedBy = userEmail,
                    CreatedDate = DateTime.Now
                };
                _context.CityTownVillages.Add(CreateCityTownVillage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CityTownVillages));
            }
            ViewBag.States = new SelectList(_context.States, "StateId", "StateName");
            ViewBag.Districts = new SelectList(_context.Districts, "DistrictId", "DistrictName");
            return View(cityTownVillage);
        }


        [HttpGet]
        public IActionResult EditCityTownVillage(int id)
        {
            var cityTownVillage = _context.CityTownVillages.FirstOrDefault(c => c.CityTownVillageID == id);
            if (cityTownVillage == null)
            {
                return NotFound();
            }
            var cityTownVillageModel = new CityTownVillagemodel
            {
                CityTownVillageID = cityTownVillage.CityTownVillageID,
                CityTownVillageName = cityTownVillage.CityTownVillageName,
                DistrictId = cityTownVillage.DistrictId
            };
            ViewBag.Districts = new SelectList(_context.Districts, "DistrictId", "DistrictName", cityTownVillage.DistrictId);
            return View(cityTownVillageModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditCityTownVillage(CityTownVillagemodel EditcityTownVillage)
        {
            if (ModelState.IsValid)
            {
                var cityTownVillage = _context.CityTownVillages.FirstOrDefault(c => c.CityTownVillageID == EditcityTownVillage.CityTownVillageID);
                if (cityTownVillage == null)
                {
                    return NotFound();
                }

                var existingCityTownVillage = _context.CityTownVillages.FirstOrDefault(c => c.CityTownVillageName == EditcityTownVillage.CityTownVillageName && c.DistrictId == EditcityTownVillage.DistrictId && c.CityTownVillageID != EditcityTownVillage.CityTownVillageID);
                if (existingCityTownVillage != null)
                {
                    ModelState.AddModelError("CityTownVillageName", "City/Town/Village name already exists in the selected district.");
                    ViewBag.Districts = new SelectList(_context.Districts, "DistrictId", "DistrictName", EditcityTownVillage.DistrictId);
                    return View(EditcityTownVillage);
                }

                cityTownVillage.CityTownVillageName = EditcityTownVillage.CityTownVillageName;
                cityTownVillage.DistrictId = EditcityTownVillage.DistrictId;
                _context.CityTownVillages.Update(cityTownVillage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CityTownVillages));
            }
            ViewBag.Districts = new SelectList(_context.Districts, "DistrictId", "DistrictName", EditcityTownVillage.DistrictId);
            return View(EditcityTownVillage);
        }

        [HttpGet]
        public IActionResult DeleteCityTownVillage(int id)
        {
            var cityTownVillage = _context.CityTownVillages
                .Include(c => c.District)
                .ThenInclude(d => d.State)
                .FirstOrDefault(c => c.CityTownVillageID == id);
            if (cityTownVillage == null)
            {
                return NotFound();
            }
            var cityTownVillageModel = new CityTownVillagemodel
            {
                CityTownVillageID = cityTownVillage.CityTownVillageID,
                CityTownVillageName = cityTownVillage.CityTownVillageName,
                DistrictId = cityTownVillage.DistrictId,
                DistrictName = cityTownVillage.District.DistrictName,
                StateId = cityTownVillage.District.StateId,
                StateName = cityTownVillage.District.State.StateName
            };
            return View(cityTownVillageModel);
        }


        [HttpPost, ActionName("DeleteCityTownVillage")]
        public async Task<IActionResult> DeleteCityTownVillageConfirmed(int id)
        {
            var cityTownVillage = _context.CityTownVillages.FirstOrDefault(c => c.CityTownVillageID == id);
            if (cityTownVillage == null)
            {
                return NotFound();
            }
            _context.CityTownVillages.Remove(cityTownVillage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CityTownVillages));
        }
        #endregion
        #region Manage EventsMaster
        public IActionResult Events()
        {
            var events = _context.EventMasters.ToList();
            var eventModels = events.Select(e => new EventsMastermodel
            {
                EventId = e.EventId,
                EventName = e.EventName,
                EventDate = e.EventDate,
                Location = e.Location
            }).ToList();
            return View(eventModels);
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventsMastermodel eventModel)
        {
            if (ModelState.IsValid)
            {
                var existingEvent = _context.EventMasters.FirstOrDefault(e => e.EventName == eventModel.EventName);
                if (existingEvent != null)
                {
                    ModelState.AddModelError("EventName", "Event name already exists.");
                    return View(eventModel);
                }

                var userEmail = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.UserEmail);
                var guidString = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.UserGUID);
                if (userEmail == null)
                {
                    return BadRequest("User is not available in the session.");
                }

                var guid = Guid.Parse(guidString);
                var appMaster = _context.AppMasters.FirstOrDefault(a => a.AppsGUID == guid);



                var CreateEvent = new EventsMaster
                {
                    EventName = eventModel.EventName,
                    EventDate = eventModel.EventDate,
                    Location = eventModel.Location,
                    CreatedBy = userEmail,
                    CreatedDate = DateTime.Now,
                    AppID = appMaster.AppID


                };
                _context.EventMasters.Add(CreateEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Events));
            }
            return View(eventModel);
        }

        [HttpGet]
        public IActionResult EditEvent(int id)
        {
            var eventMaster = _context.EventMasters.FirstOrDefault(e => e.EventId == id);
            if (eventMaster == null)
            {
                return NotFound();
            }
            var eventModel = new EventsMastermodel
            {
                EventId = eventMaster.EventId,
                EventName = eventMaster.EventName,
                EventDate = eventMaster.EventDate,
                Location = eventMaster.Location
            };
            return View(eventModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(EventsMastermodel EditEvent)
        {
            if (ModelState.IsValid)
            {
                var eventMaster = _context.EventMasters.FirstOrDefault(e => e.EventId == EditEvent.EventId);
                if (eventMaster == null)
                {
                    return NotFound();
                }

                var existingEvent = _context.EventMasters.FirstOrDefault(e => e.EventName == EditEvent.EventName && e.EventId != EditEvent.EventId);
                if (existingEvent != null)
                {
                    ModelState.AddModelError("EventName", "Event name already exists.");
                    return View(EditEvent);
                }

                eventMaster.EventName = EditEvent.EventName;
                eventMaster.EventDate = EditEvent.EventDate;
                eventMaster.Location = EditEvent.Location;
                _context.EventMasters.Update(eventMaster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Events));
            }
            return View(EditEvent);
        }

        [HttpGet]
        public IActionResult DeleteEvent(int id)
        {
            var eventMaster = _context.EventMasters.FirstOrDefault(e => e.EventId == id);
            if (eventMaster == null)
            {
                return NotFound();
            }
            var eventModel = new EventsMastermodel
            {
                EventId = eventMaster.EventId,
                EventName = eventMaster.EventName,
                EventDate = eventMaster.EventDate,
                Location = eventMaster.Location
            };
            return View(eventModel);
        }

        [HttpPost, ActionName("DeleteEvent")]
        public async Task<IActionResult> DeleteEventConfirmed(int id)
        {
            var eventMaster = _context.EventMasters.FirstOrDefault(e => e.EventId == id);
            if (eventMaster == null)
            {
                return NotFound();
            }
            _context.EventMasters.Remove(eventMaster);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Events));
        }
        #endregion
        #region Manage Guests
        public IActionResult Guests()
        {
            var guests = _context.GuestLists.Include(g => g.CityTownVillage).ToList();
            var guestModels = guests.Select(g => new Guestmodel
            {
                GuestId = g.GuestId,
                AppID = g.AppID,
                GuestName = g.GuestName,
                GuestGender = g.GuestGender,
                CityTownVillageID = g.CityTownVillageID,
                ListCityTownVillage = _context.CityTownVillages.Select(ctv => new ListCityTownVillagemodel
                {
                    CityTownVillageID = ctv.CityTownVillageID,
                    CityTownVillageName = ctv.CityTownVillageName
                }).ToList(),
                ListDistrict = _context.Districts.Select(d => new ListDistrictmodel
                {
                    DistrictId = d.DistrictId,
                    DistrictName = d.DistrictName
                }).ToList(),
                ListState = _context.States.Select(s => new ListStatemodel
                {
                    StateId = s.StateId,
                    StateName = s.StateName
                }).ToList(),
                ListCountry = _context.Countries.Select(c => new ListCountrymodel
                {
                    CountryId = c.CountryId,
                    CountryName = c.CountryName
                }).ToList()
            }).ToList();
            return View(guestModels);
        }

        [HttpGet]
        public IActionResult CreateGuest()
        {
            ViewBag.Countries = new SelectList(_context.Countries, "CountryId", "CountryName");
            ViewBag.States = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Districts = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.CityTownVillages = new SelectList(Enumerable.Empty<SelectListItem>());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGuest(Guestmodel guestModel)
        {
            if (ModelState.IsValid)
            {
                var existingGuest = _context.GuestLists.FirstOrDefault(g => g.GuestName == guestModel.GuestName);
                if (existingGuest != null)
                {
                    ModelState.AddModelError("GuestName", "Guest name already exists.");
                    ViewBag.Countries = new SelectList(_context.Countries, "CountryId", "CountryName");
                    ViewBag.States = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewBag.Districts = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewBag.CityTownVillages = new SelectList(Enumerable.Empty<SelectListItem>());
                    return View(guestModel);
                }

                var userEmail = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.UserEmail);
                if (userEmail == null)
                {
                    return BadRequest("User is not available in the session.");
                }
                var guidString = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.UserGUID);
                var guid = Guid.Parse(guidString);
                var appMaster = _context.AppMasters.FirstOrDefault(a => a.AppsGUID == guid);
                var CreateGuest = new GuestList
                {
                    GuestName = guestModel.GuestName,
                    GuestGender = guestModel.GuestGender,
                    CityTownVillageID = guestModel.CityTownVillageID,
                    CreatedBy = userEmail,
                    CreatedDate = DateTime.Now,
                    AppID = appMaster.AppID
                };
                _context.GuestLists.Add(CreateGuest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Guests));
            }
            else
            {
                // Log the model state errors for debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                // You can log these errors to a file or a logging service
                // For example: _logger.LogError("ModelState errors: {Errors}", string.Join(", ", errors));
            }
            ViewBag.Countries = new SelectList(_context.Countries, "CountryId", "CountryName");
            ViewBag.States = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Districts = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.CityTownVillages = new SelectList(Enumerable.Empty<SelectListItem>());
            return View(guestModel);
        }


        [HttpGet]
        public IActionResult EditGuest(int id)
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
                CityTownVillageID = guest.CityTownVillageID
            };
            ViewBag.CityTownVillages = new SelectList(_context.CityTownVillages, "CityTownVillageID", "CityTownVillageName", guest.CityTownVillageID);
            return View(guestModel);
        }
        [HttpGet]
        public IActionResult GuestEventsMap(int id)
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
                            EventId= evm.EventId,
                            AppID= evm.AppID,
                            EventName=evm.EventName,
                            EventDate=evm.EventDate,
                            IsAttended= egm != null ? egm.IsAttended :false,
                            EventGiftDetails= egm != null ? egm.EventGiftDetails : null
                         };

            var result = query.ToList();
            guestModel.ListEventGuestMappingmodel = result;

            return View(guestModel);
        }
        [HttpPost]
        public async Task<IActionResult> SaveGuestEventsMap(Guestmodel model)
        {
            if (ModelState.IsValid)
            {
                var userEmail = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.UserEmail);
                if (userEmail == null)
                {
                    return BadRequest("User is not available in the session.");
                }
                foreach (var mapping in model.ListEventGuestMappingmodel)
                {
                    var existingMapping = await _context.EventGuestMapping
                        .FirstOrDefaultAsync(egm => egm.EventId == mapping.EventId && egm.GuestId == model.GuestId);
                    
                    if (!mapping.IsAttended)
                        mapping.EventGiftDetails = string.Empty;

                    if (existingMapping != null)
                    {  if(existingMapping.IsAttended != mapping.IsAttended)
                        {
                            existingMapping.IsAttended = mapping.IsAttended;
                            _context.EventGuestMapping.Update(existingMapping);
                        }
                       
                      existingMapping.EventGiftDetails = mapping.EventGiftDetails;
                        _context.EventGuestMapping.Update(existingMapping);
                    }
                    else
                    {
                        var newMapping = new EventGuestMapping
                        {
                            EventId = mapping.EventId,
                            GuestId = model.GuestId,
                            EventGiftDetails = mapping.EventGiftDetails,
                            IsAttended=mapping.IsAttended,
                            CreatedBy= userEmail,
                            CreatedDate = DateTime.Now
                        };
                        _context.EventGuestMapping.Add(newMapping);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Guests));
            }
            else
            {
                // Log the model state errors for debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                // You can log these errors to a file or a logging service
                // For example: _logger.LogError("ModelState errors: {Errors}", string.Join(", ", errors));
            }

            // If we got this far, something failed, redisplay form
            return View("GuestEventsMap", model);
            
        }
        [HttpPost]
        public async Task<IActionResult> EditGuest(Guestmodel EditGuest)
        {
            if (ModelState.IsValid)
            {
                var guest = _context.GuestLists.FirstOrDefault(g => g.GuestId == EditGuest.GuestId);
                if (guest == null)
                {
                    return NotFound();
                }

                var existingGuest = _context.GuestLists.FirstOrDefault(g => g.GuestName == EditGuest.GuestName && g.GuestId != EditGuest.GuestId);
                if (existingGuest != null)
                {
                    ModelState.AddModelError("GuestName", "Guest name already exists.");
                    ViewBag.CityTownVillages = new SelectList(_context.CityTownVillages, "CityTownVillageID", "CityTownVillageName", EditGuest.CityTownVillageID);
                    return View(EditGuest);
                }

                guest.GuestName = EditGuest.GuestName;
                guest.GuestGender = EditGuest.GuestGender;
                guest.CityTownVillageID = EditGuest.CityTownVillageID;
                _context.GuestLists.Update(guest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Guests));
            }
            ViewBag.CityTownVillages = new SelectList(_context.CityTownVillages, "CityTownVillageID", "CityTownVillageName", EditGuest.CityTownVillageID);
            return View(EditGuest);
        }

        [HttpGet]
        public IActionResult DeleteGuest(int id)
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
            return View(guestModel);
        }

        [HttpPost, ActionName("DeleteGuest")]
        public async Task<IActionResult> DeleteGuestConfirmed(int id)
        {
            var guest = _context.GuestLists.FirstOrDefault(g => g.GuestId == id);
            if (guest == null)
            {
                return NotFound();
            }
            _context.GuestLists.Remove(guest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Guests));
        }
        [HttpGet]
        public JsonResult GetStatesByCountry(int countryId)
        {
            var states = _context.States
                .Where(s => s.CountryId == countryId)
                .Select(s => new { s.StateId, s.StateName })
                .ToList();
            return Json(states);
        }

        [HttpGet]
        public JsonResult GetDistrictsByState(int stateId)
        {
            var districts = _context.Districts
                .Where(d => d.StateId == stateId)
                .Select(d => new { d.DistrictId, d.DistrictName })
                .ToList();
            return Json(districts);
        }

        [HttpGet]
        public JsonResult GetCityTownVillagesByDistrict(int districtId)
        {
            var cityTownVillages = _context.CityTownVillages
                .Where(ctv => ctv.DistrictId == districtId)
                .Select(ctv => new { ctv.CityTownVillageID, ctv.CityTownVillageName })
                .ToList();
            return Json(cityTownVillages);

            #endregion

        }
    }
}
