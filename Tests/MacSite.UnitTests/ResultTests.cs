using MacSite.Core.Common;
using FluentAssertions;

namespace MacSite.UnitTests;

public class ResultTests
{
    [Fact]
    public void Success_carries_value()
    {
        Result<int> result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_carries_error_and_throws_on_value()
    {
        var error = Error.NotFound("Production");
        Result<int> result = Result.Failure<int>(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);

        var act = () => result.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Implicit_conversions_work_for_value_and_error()
    {
        Result<string> ok = "hello";
        Result<string> bad = Error.Validation("nope");

        ok.IsSuccess.Should().BeTrue();
        ok.Value.Should().Be("hello");
        bad.IsFailure.Should().BeTrue();
        bad.Error.Code.Should().Be("validation");
    }

    [Fact]
    public void Match_selects_the_right_branch()
    {
        Result<int> ok = 10;
        Result<int> bad = Error.Unexpected("boom");

        ok.Match(v => $"ok:{v}", e => $"err:{e.Code}").Should().Be("ok:10");
        bad.Match(v => $"ok:{v}", e => $"err:{e.Code}").Should().Be("err:unexpected");
    }
}
