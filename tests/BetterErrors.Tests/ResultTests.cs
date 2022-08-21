namespace BetterErrors.Tests;

public class ResultTests
{
    [Fact]
    public void Result_WhenSuccess()
    {
        var value = 4;
        Result<int> intRes = value;

        intRes.IsSuccess.Should().BeTrue();
        intRes.IsFailure.Should().BeFalse();

        var valueAccessor = () => intRes.Value;
        var errAccessor = () => intRes.Error;

        valueAccessor.Should().NotThrow()
            .Subject
            .Should()
            .Be(value);
        errAccessor.Should().Throw<InvalidOperationException>();

        // Match test
        
        intRes.Match(_ => 0, _ => -1).Should().Be(0);

        // Switch test
        var valueAction = Substitute.For<Action<int>>();
        var errAction = Substitute.For<Action<IError>>();

        intRes.Switch(valueAction, errAction);

        valueAction.Received(1)(value);
        errAction.DidNotReceive()(Arg.Any<IError>());
    }

    [Fact]
    public void Result_WhenFailure()
    {
        Error err = new("test err");
        Result<int> intRes = err;

        intRes.IsSuccess.Should().BeFalse();
        intRes.IsFailure.Should().BeTrue();

        var valueAccessor = () => intRes.Value;
        var errAccessor = () => intRes.Error;

        valueAccessor.Should().Throw<InvalidOperationException>();
        errAccessor.Should().NotThrow()
            .Subject
            .Should()
            .Be(err);

        // Match test
        
        intRes.Match(_ => 0, _ => -1).Should().Be(-1);

        // Switch test
        var valueAction = Substitute.For<Action<int>>();
        var errAction = Substitute.For<Action<IError>>();

        intRes.Switch(valueAction, errAction);

        valueAction.DidNotReceive()(Arg.Any<int>());
        errAction.Received(1)(err);
    }

    [Fact]
    public void MapResult_WhenSuccess()
    {
        Result<int> intRes = 4;
        var strResult = intRes.Map<string>(_ => "ok");

        strResult.IsSuccess.Should().BeTrue();
        strResult.Value.Should().Be("ok");
    }

    [Fact]
    public void MapResult_WhenFailue()
    {
        var err = new Error("err");
        Result<int> intRes = err;
        var strResult = intRes.Map<string>(_ => "ok");

        strResult.IsSuccess.Should().BeFalse();
        strResult.Error.Should().Be(err);
    }
}