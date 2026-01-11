using Microsoft.VisualStudio.TestTools.UnitTesting;
using booseapp;
using BOOSE;

namespace booseapp.tests
{
    [TestClass]
    public class IfWhileForTests
    {
        private static ExecutionResult Run(string program)
        { 
            var canvas = new CommandCanvas();
            var interpreter = new BooseInterpreter(canvas);
            return interpreter.Execute(program);
        }

        [TestMethod]
        public void IfElse_Chooses_True_Branch()
        {
            string program = @"
int a = 10
if a > 5
    write ""TRUE""
else
    write ""FALSE""
end if
";
            var result = Run(program);
            StringAssert.Contains(result.Log, "TRUE");
        }

        [TestMethod]
        public void While_Loops_Correct_Number_Of_Times()
        {
            string program = @"
int i = 0
int sum = 0
while i < 5
    sum = sum + 2
    i = i + 1
end while
write sum
";
            var result = Run(program);
            StringAssert.Contains(result.Log, "10");
        }

        [TestMethod]
        public void For_Loop_Increments_And_Stops()
        { 
            string program = @"
int i = 0
int sum = 0
for i = 1 to 5 step 1
    sum = sum + i
end for
write sum
";
            var result = Run(program);
            StringAssert.Contains(result.Log, "15");
        }
    }
}
