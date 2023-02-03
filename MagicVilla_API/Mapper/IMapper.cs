namespace MagicVilla_API.Mapper
{
    public interface IMapper<I, O>
    {
        O CreateMap(I input);
    }
}
