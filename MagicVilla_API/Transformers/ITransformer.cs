namespace MagicVilla_API.Transformers
{
    public interface ITransformer<I, O>
    {
        O CreateMap(I input);
    }
}
