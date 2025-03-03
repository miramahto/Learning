using EventManagementSystem.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
   
    public class Countrymodel
    {
        public int CountryId { get; set; }
        public string ?CountryName { get; set; }
    }
    public class ListCountrymodel
    {
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
    }
    public class Statemodel
    {
        
        public int StateId { get; set; }
        public string ? CountryName { get; set; }
        public string ?StateName { get; set; }
        public int CountryId { get; set; }
    }
    public class ListStatemodel
    {

        public int StateId { get; set; }
        public string? StateName { get; set; }
       
    }
    public class Districtmodel
    {

        public int DistrictId { get; set; }
        public string? StateName { get; set; }
        public string ?DistrictName { get; set; }
        public int StateId { get; set; }
    }
    public class ListDistrictmodel
    {
        public int DistrictId { get; set; }
        public string? DistrictName { get; set; }
        
    }
    public class CityTownVillagemodel
    {
        public int CityTownVillageID { get; set; }
        public string? DistrictName { get; set; }
        public string ?CityTownVillageName { get; set; }
        public int DistrictId { get; set; }
        public int StateId { get; set; }
        public string ?StateName { get; set; }
    }
    public class ListCityTownVillagemodel
    {
        public int CityTownVillageID { get; set; }
       public string? CityTownVillageName { get; set; }
      
    }
    public class EventsMastermodel
    {
        public int EventId { get; set; }
        public int AppID { get; set; }
        public string ?EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string ?Location { get; set; }

    }
    public class Guestmodel
    {
        
        public int GuestId { get; set; }
         public int AppID { get; set; }
         public string GuestName { get; set; }
        public string GuestGender { get; set; }
        public int CityTownVillageID { get; set; }
        public List<ListCityTownVillagemodel> ?ListCityTownVillage { get; set; }
        public List<ListDistrictmodel> ?ListDistrict { get; set; }
        public int DistrictId { get; set; }
        public List<ListStatemodel> ?ListState { get; set; }
        public int StateId { get; set; }
        public List<ListCountrymodel> ?ListCountry { get; set; }
        public int CountryId { get; set; }
        public List<EventGuestMappingmodel>? ListEventGuestMappingmodel { get; set; }
    }
    public class EventGuestMappingmodel
    {
        public int EventGuestMappingId { get; set; }
        public int AppID { get; set; }
        public int EventId { get; set; }
        public string ?EventName { get; set; }
        public DateTime ?EventDate { get; set; }
        public int GuestId { get; set; }
        public string? GuestName { get; set; }
        public string? EventGiftDetails { get; set; }
        public bool IsAttended { get; set; } = false;
    }
   


    }
