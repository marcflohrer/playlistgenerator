namespace MusicOrganizer.Tests.TestDataGenerator;

public static class RemoveDuplicatesTests
{
    public static class RemoveDuplicatesWithDifferentAmazonIdTestDataGenerator
    {
        public static TheoryData<ICollection<string>, int, int> DuplicateSongTestInput()
        => new()
        {
            { new List<string>{ "B000FPX0FC", "B000HEWFRK" }, 0, 0  },
            { new List<string>{ "B000FPX0FC", "B000FPX0FC" }, 1, 2 }
        };
    }
}

