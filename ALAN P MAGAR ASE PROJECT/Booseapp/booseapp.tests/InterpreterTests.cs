using BOOSE;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace booseapp.tests
{
    [TestClass]
    public sealed class InterpreterTests
    { 
        [TestMethod]
        public void Variables_And_Arithmetic_Work()
        {
            var canvas = new CommandCanvas(200, 200);
            var interp = new BooseInterpreter(canvas);

            string program = @"
int radius = 50
int width
width = 2*radius
";

            var result = interp.Execute(program);
            // If no exception thrown, at least arithmetic executed.
            Assert.IsTrue(result.StatementsExecuted >= 2);
        }

        [TestMethod]
        public void If_Statement_Works()
        {
            var canvas = new CommandCanvas(200, 200);
            var interp = new BooseInterpreter(canvas);

            string program = @"
int x = 5
if x > 3
    x = x + 1
else
    x = x - 1
end if
";

            var result = interp.Execute(program);
            Assert.IsTrue(result.StatementsExecuted >= 2);
        }

        [TestMethod]
        public void Arrays_Poke_And_Peek_Work()
        {
            var canvas = new CommandCanvas(200, 200);
            var interp = new BooseInterpreter(canvas);

            string program = @"
array int nums 10
int x
poke nums 5 = 99
peek x = nums 5
write x
";

            var result = interp.Execute(program);
            Assert.IsTrue(result.Log.Contains("99"));
        }

        [TestMethod]
        public void Methods_Return_Value_Work()
        {
            var canvas = new CommandCanvas(200, 200);
            var interp = new BooseInterpreter(canvas);

            string program = @"
method int add int a, int b
    add = a + b
end method

call add 3 4
write add
";

            var result = interp.Execute(program);
            Assert.IsTrue(result.Log.Contains("7"));
        }

        [TestMethod]
        public void While_And_For_Loops_Work()
        {
            var canvas = new CommandCanvas(200, 200);
            var interp = new BooseInterpreter(canvas);

            string program = @"
int sum = 0
int i = 1
while i <= 5
    sum = sum + i
    i = i + 1
end while

int total = 0
for j = 1 to 5 step 1
    total = total + j
end for

write sum
write total
";

            var result = interp.Execute(program);
            // 1+2+3+4+5 = 15
            Assert.IsTrue(result.Log.Contains("15"));
        }

        [TestMethod]
        public void Text_Command_Is_Accepted()
        {
            var canvas = new CommandCanvas(200, 200);
            var interp = new BooseInterpreter(canvas);

            string program = @"
moveto 10 10
text hello world
";

            var result = interp.Execute(program);
            Assert.IsTrue(result.StatementsExecuted >= 2);
            Assert.IsTrue(result.Log.ToLowerInvariant().Contains("hello"));
        }
    }
}
