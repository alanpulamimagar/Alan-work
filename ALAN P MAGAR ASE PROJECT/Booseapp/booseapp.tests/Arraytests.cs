using Microsoft.VisualStudio.TestTools.UnitTesting;
using booseapp;   // CommandCanvas
using BOOSE;      // BooseInterpreter

namespace booseapp.tests
{
    [TestClass] 
    public class ArrayTests
    {
        private static ExecutionResult Run(string program)
        {
            var canvas = new CommandCanvas();
            var interpreter = new BooseInterpreter(canvas);
            return interpreter.Execute(program);
        }

        [TestMethod]
        public void Array_Declare_Poke_Peek_Writes_Value() 
        {
            // array int, poke, peek, write
            string program = @"
array int nums 10
poke nums 5 = 99
int x = 0
peek x = nums 5
write x
";
            var result = Run(program);
            StringAssert.Contains(result.Log, "99");
        }

        [TestMethod]
        public void Array_Poke_Multiple_Then_Sum()
        {
            string program = @"
array int nums 3
poke nums 0 = 10
poke nums 1 = 20
poke nums 2 = 30

int i = 0
int temp = 0
int sum = 0

for i = 0 to 2 step 1
    peek temp = nums i
    sum = sum + temp
end for

write sum
";
            var result = Run(program);
            StringAssert.Contains(result.Log, "60");
        }

        [TestMethod]
        [ExpectedException(typeof(System.IndexOutOfRangeException))]
        public void ArrayOutOfRangeThrows()
        {
            string program = @"
array int nums 3
poke nums 5 = 10
";
            Run(program);
        }

        [TestMethod]
        [ExpectedException(typeof(System.IndexOutOfRangeException))]
        public void Array_NegativeIndex_Throws()
        {
            string program = @"
array int nums 3
poke nums -1 = 10
";
            Run(program);
        }
    }
}
