using MagicVilla_API.Models.DTO;

namespace MagicVilla_API.Data
{
    public class VillaStore
    {
        public static List<VillaDTO> VillaList = new List<VillaDTO> {
                new VillaDTO { Id = 1, Name = "Pool view" },
                new VillaDTO { Id = 2, Name = "Beach view" }
            };
    }
}
