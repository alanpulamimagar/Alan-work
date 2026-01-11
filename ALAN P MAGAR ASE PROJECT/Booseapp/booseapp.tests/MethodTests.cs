using Microsoft.VisualStudio.TestTools.UnitTesting;
using booseapp;
using BOOSE;

namespace booseapp.tests
{
    [TestClass] 
    public class MethodTests
    {
        private static ExecutionResult Run(string program)
        {
            var canvas = new CommandCanvas();
            var interpreter = new BooseInterpreter(canvas);
            return interpreter.Execute(program);
        }

        [TestMethod]
        public void Method_Call_Sets_Method_Result()
        {
            // Your implementation returns by assigning to method name.
            string program = @"
method int add int a, int b
    add = a + b
end method

call add 40 2
write add
";
            var result = Run(program);
            StringAssert.Contains(result.Log, "42");
        }

        [TestMethod]
        public void Method_Can_Be_Called_Multiple_Times()
        {
            string program = @"
method int mul int a, int b
    mul = a * b
end method

call mul 3 4
write mul
call mul 5 6
write mul
";
            var result = Run(program);
            StringAssert.Contains(result.Log, "12");
            StringAssert.Contains(result.Log, "30");
        }
    }
}
