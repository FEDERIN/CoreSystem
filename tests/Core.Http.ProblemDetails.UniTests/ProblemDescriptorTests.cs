namespace Core.Http.ProblemDetails.UniTests;

public sealed class ProblemDescriptorTests
{
    [Theory]
    [InlineData(99)]
    [InlineData(600)]
    public void Constructor_RejectsAnInvalidHttpStatusCode(int status)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProblemDescriptor(status, "ERROR", "Title", "Detail"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_RejectsEmptyRequiredFields(string value)
    {
        Assert.Throws<ArgumentException>(() => new ProblemDescriptor(500, value, "Title", "Detail"));
        Assert.Throws<ArgumentException>(() => new ProblemDescriptor(500, "ERROR", value, "Detail"));
        Assert.Throws<ArgumentException>(() => new ProblemDescriptor(500, "ERROR", "Title", value));
    }

    [Fact]
    public void Constructor_RejectsNullRequiredFields()
    {
        Assert.Throws<ArgumentNullException>(() => new ProblemDescriptor(500, null!, "Title", "Detail"));
        Assert.Throws<ArgumentNullException>(() => new ProblemDescriptor(500, "ERROR", null!, "Detail"));
        Assert.Throws<ArgumentNullException>(() => new ProblemDescriptor(500, "ERROR", "Title", null!));
    }

    [Fact]
    public void WithExpression_PreservesValidation()
    {
        var descriptor = new ProblemDescriptor(500, "ERROR", "Title", "Detail");

        Assert.Throws<ArgumentException>(() => descriptor with { ErrorCode = " " });
        Assert.Throws<ArgumentOutOfRangeException>(() => descriptor with { Status = 600 });
    }
}
