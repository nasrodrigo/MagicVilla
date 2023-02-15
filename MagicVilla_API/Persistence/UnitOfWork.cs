using MagicVilla_API.DataMappers;
using MagicVilla_API.Repositories;

namespace MagicVilla_API.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _db;
        private bool disposedValue;

        public UnitOfWork(
            ApplicationDBContext db,
            IVillaRepository villaRepository,
            IVillaNumberRepository villaNumberRepository,
            IAuthDataMapper authDataMapper,
            IVillaDataMapper villaDataMapper,
            IVillaNumberDataMapper villaNumberDataMapper)
        {
            _db = db;
            VillaRepository = villaRepository;
            VillaNumberRepository = villaNumberRepository;
            AuthDataMapper = authDataMapper;
            VillaDataMapper = villaDataMapper;
            VillaNumberDataMapper = villaNumberDataMapper;
        }

        public IVillaRepository VillaRepository { get; private set; }

        public IVillaNumberRepository VillaNumberRepository { get; private set; }

        public IAuthDataMapper AuthDataMapper { get; private set; }

        public IVillaDataMapper VillaDataMapper { get; private set; }

        public IVillaNumberDataMapper VillaNumberDataMapper { get; private set; }

        public int Complete()
        {
            return _db.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UnitOfWork()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
