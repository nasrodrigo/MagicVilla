using MagicVilla_API.Models.DTO;
using MagicVilla_API.Models;

namespace MagicVilla_API.Mapper
{
    public class VillaNumberToVillaNumberDTOMapper : IMapper<VillaNumber, VillaNumberDTO>
    {
        public VillaNumberDTO CreateMap(VillaNumber villaNumber)
        {
            return new VillaNumberDTO
            {
                Id = villaNumber.Id,
                VillaId = villaNumber.VillaId,
                SpecialDetails = villaNumber.SpecialDetails
            };
        }
    }
}

