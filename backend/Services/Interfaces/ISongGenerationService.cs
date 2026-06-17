using backend.Dtos;

namespace backend.Services;

public interface ISongGenerationService
{
    Task<IReadOnlyList<SongDto>> GenerateAsync(
        SongGenerationRequest request);

    SongDto GenerateFromSeed(
        string locale,
        int songSeed,
        int index

    );

}