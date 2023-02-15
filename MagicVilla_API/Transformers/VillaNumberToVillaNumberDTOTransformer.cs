using MagicVilla_API.Models.DTOs;
using MagicVilla_API.Models;

namespace MagicVilla_API.Transformers
{
    public class VillaNumberToVillaNumberDTOTransformer : ITransformer<VillaNumber, VillaNumberDTO>
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

