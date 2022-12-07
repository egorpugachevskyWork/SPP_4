using MainPart.ForScript;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class Tests
    {
        private static List<string> _namesOfFile = new List<string>() { "A.cs", "B.cs", "Akappuza.cs" }; //{ "Aleksei.cs", "Akappuza.cs", "Gigachad.cs", "Oop.cs", "SPP.cs", "A.cs", "B.cs" };
        private static string _writePath = "D:\\Studying\\third_course\\ÑÏÏ\\generated_test";
        private TestScripter _scripter;

        public Tests()
        {
            _scripter = new TestScripter(_namesOfFile, _writePath, 3, 2, 4);
            _scripter.Generate().GetAwaiter().GetResult();
        }


        [Test]
        public void CheckAmountAndNamesOfGeneratedFiles()
        {
            var count = _scripter.GeneratedFiles.Count;
            var listOfNames = new List<string>();
            listOfNames.AddRange(_scripter.GeneratedFiles.Keys);
            Assert.Multiple(() =>
            {
                Assert.That(count == 4, $"Wrong amount of generated files {count}");
                Assert.That(listOfNames.FindAll(n => n.Contains("Test")).Count == 4, "Wrong extension of files");
            });
        }

        [Test]
        public void CheckAmountOfAnotherATestMethods()
        {
            var anotherAFileCs = _scripter.GeneratedFiles["AnotherATest"];
            var count = 0;
            while (anotherAFileCs.Contains("[Test]"))
            {
                var index = anotherAFileCs.IndexOf("[Test]");
                count++;
                anotherAFileCs = anotherAFileCs.Remove(index, 6);
            }

            Assert.Multiple(() =>
            {
                Assert.That(count == 2, $"Wrong amount public methods in AnotherATest.cs file {count}");
            });
        }

        [Test]
        public void CheckAmountOfATestMethods()
        {
            var anotherAFileCs = _scripter.GeneratedFiles["ATest"];
            var count = 0;
            while (anotherAFileCs.Contains("[Test]"))
            {
                var index = anotherAFileCs.IndexOf("[Test]");
                count++;
                anotherAFileCs = anotherAFileCs.Remove(index, 6);
            }

            Assert.Multiple(() =>
            {
                Assert.That(count == 1, $"Wrong amount public methods in ATest.cs file {count}");
            });
        }

        [Test]
        public void CheckAmountOfFakerTestMethods()
        {
            var anotherAFileCs = _scripter.GeneratedFiles["FakerTest"];
            var count = 0;
            while (anotherAFileCs.Contains("[Test]"))
            {
                var index = anotherAFileCs.IndexOf("[Test]");
                count++;
                anotherAFileCs = anotherAFileCs.Remove(index, 6);
            }

            Assert.Multiple(() =>
            {
                Assert.That(count == 12, $"Wrong amount public methods in FakerTest.cs file {count}");
            });
        }

        [Test]
        public void CheckAmountOfFakerConfigTestMethods()
        {
            var anotherAFileCs = _scripter.GeneratedFiles["FakerConfigTest"];
            var count = 0;
            while (anotherAFileCs.Contains("[Test]"))
            {
                var index = anotherAFileCs.IndexOf("[Test]");
                count++;
                anotherAFileCs = anotherAFileCs.Remove(index, 6);
            }

            Assert.Multiple(() =>
            {
                Assert.That(count == 3, $"Wrong amount public methods in FakerConfigTest.cs file {count}");
            });
        }

        [Test]
        public void CheckNamesOfFakerConfigTestMethods()
        {
            var anotherAFileCs = _scripter.GeneratedFiles["FakerConfigTest"];

            Assert.Multiple(() =>
            {
                Assert.That(anotherAFileCs.Contains("AddTest"), "Doesn't contain AddTest method");
                Assert.That(anotherAFileCs.Contains("CheckGeneratorTest"), "Doesn't contain CheckGeneratorTest method");
                Assert.That(anotherAFileCs.Contains("ObtaionGeneratorTest"), "Doesn't contain ObtaionGeneratorTest method");
            });
        }
    }
}