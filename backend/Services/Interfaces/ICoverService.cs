public interface ICoverService
{
    Task<byte[]> GenerateAsync(
        string locale,
        int songSeed,
        int index);
}