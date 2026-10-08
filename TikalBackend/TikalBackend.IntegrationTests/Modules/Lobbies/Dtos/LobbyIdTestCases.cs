using Xunit;

internal class LobbyIdTestCases : TheoryData<long>
{
    public LobbyIdTestCases()
    {
        Add(0);
        Add(1);
        Add(123);
        Add(45345);
        Add(3490934853);
    }
}