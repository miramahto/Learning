using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Data
{
  
    public class BaseModel
    {
        [MaxLength(80)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
   

    public class Country: BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }
        [MaxLength(100)]
        public string CountryName { get; set; }
         public ICollection<State> States { get; set; }// Navigation property for related States
    }
    public class State: BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateId { get; set; }
        [MaxLength(100)]
        public string StateName { get; set; }
        [ForeignKey("CountryId")]
        public int CountryId { get; set; }
        public Country Country { get; set; } // Add this navigation property
        public ICollection<District> Districts { get; set; }// Navigation property for related Districts
    }
    public class District: BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DistrictId { get; set; }
        [MaxLength(100)]
        public string DistrictName { get; set; }
        [ForeignKey("StateId")]
        public int StateId { get; set; }
        public State State { get; set; } // Add this navigation property
        public ICollection<CityTownVillage> CityTownVillages { get; set; }// Navigation property for related CityTownVillages
    }
    public class CityTownVillage: BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CityTownVillageID { get; set; }
        [MaxLength(100)]
        public string CityTownVillageName { get; set; }
        [ForeignKey("DistrictId")]
        public int DistrictId { get; set; }
        public District District { get; set; } // Add this navigation property
        public ICollection<GuestList> GuestLists { get; set; }// Navigation property for related GuestLists
    }
    public class AppMaster : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppID { get; set; }
        public Guid AppsGUID { get; set; }
    }
    public class EventsMaster: BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }
        [ForeignKey("AppID")]
        public int AppID { get; set; }
        [MaxLength(200)]
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        [MaxLength(200)]
        public string Location { get; set; }
       
    }
    public class GuestList: BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GuestId { get; set; }
        [ForeignKey("AppID")]
        public int AppID { get; set; }
        [MaxLength(200)]
        public string GuestName { get; set; }
        [MaxLength(10)]
        public string GuestGender { get; set; }
        [ForeignKey("CityTownVillageID")]
        public int CityTownVillageID { get; set; }
        public CityTownVillage CityTownVillage { get; set; } // Add this navigation property
    }
    public class EventGuestMapping:BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("EventId")]
        public int EventId { get; set; }
        [ForeignKey("GuestId")]
        public int GuestId { get; set; }
        [MaxLength(500)]
        public string EventGiftDetails { get; set; }
        public bool IsAttended { get; set; } = false;
    }
}
