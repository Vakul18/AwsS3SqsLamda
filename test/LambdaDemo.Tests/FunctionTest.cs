using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

namespace LambdaDemo.Tests;

public class FunctionTest
{
    [Fact]
    public void TestToUpperFunction()
    {

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new Function();
        var context = new TestLambdaContext();
        function.FunctionHandler("hello world", context).Wait();

        //Assert.Equal("HELLO WORLD", upperCase);
    }
}
