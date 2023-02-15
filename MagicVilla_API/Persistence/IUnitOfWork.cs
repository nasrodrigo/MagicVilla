using MagicVilla_API.DataMappers;
using MagicVilla_API.Repositories;

namespace MagicVilla_API.Persistence
{
    public interface IUnitOfWork: IDisposable
    {
        IVillaRepository VillaRepository { get; }
        IVillaNumberRepository VillaNumberRepository { get; }

        IAuthDataMapper AuthDataMapper { get; }
        IVillaDataMapper VillaDataMapper { get; }
        IVillaNumberDataMapper VillaNumberDataMapper { get; }

        int Complete();
    }
}