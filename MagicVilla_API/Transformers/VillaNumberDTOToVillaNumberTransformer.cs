using MagicVilla_API.Models.DTOs;
using MagicVilla_API.Models;

namespace MagicVilla_API.Transformers
{
    public class VillaNumberDTOToVillaNumberTransformer : ITransformer<VillaNumberDTO, VillaNumber>
    {
        public VillaNumber CreateMap(VillaNumberDTO villaNumberDTO)
        {
            return new VillaNumber
            {
                VillaId = villaNumberDTO.VillaId,
                SpecialDetails = villaNumberDTO.SpecialDetails
            };
        }
    }
}
