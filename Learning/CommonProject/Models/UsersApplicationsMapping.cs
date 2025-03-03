using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApi.Models
{

    public class ApplicationsTypeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppTypeID { get; set; }
        public string AppTypeName { get; set; }
       
    }
    public class ApplicationsRegisterModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppID { get; set; }
        public int AppTypeID { get; set; }
        public Guid GUID { get; set; }
        public string AppName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class UserApplicationsMappingModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("ApplicationsRegisterModel")]
        public int AppID { get; set; }
        public string UserID { get; set; }
        public ApplicationsRegisterModel ApplicationsRegisterModel { get; set; }

    }


}
