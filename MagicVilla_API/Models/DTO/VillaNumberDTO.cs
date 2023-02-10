using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Models.DTO
{
    public class VillaNumberDTO
    {
        public int Id { get; set; }
        public int VillaId { get; set; }
        public string? SpecialDetails { get; set; }
       
    }
}
