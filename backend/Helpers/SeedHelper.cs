public static class SeedHelper
{
    public static int BuildSeed(
        long userSeed,
        int page,
        int index)
    {
        return HashCode.Combine(
            userSeed,
            page,
            index);
    }
}