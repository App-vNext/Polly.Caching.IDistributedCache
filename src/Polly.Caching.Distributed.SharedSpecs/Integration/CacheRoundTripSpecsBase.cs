using System;
using Xunit;

namespace Polly.Caching.Distributed.Specs.Integration
{
    public abstract class CacheRoundTripSpecsBase
    {
        protected const string OperationKey = "SomeOperationKey";

        public static TheoryData<SampleClass> SampleClassData =>
            new TheoryData<SampleClass>
            {
                new SampleClass(),
                new SampleClass()
                {
                    StringProperty = "<html></html>",
                    IntProperty = 1
                },
                (SampleClass)null,
                default(SampleClass)
            };

        public static TheoryData<String> SampleStringData =>
            new TheoryData<String>
            {
                "some string",
                "",
                null,
                default(string),
                "null"
            };

        public static TheoryData<int> SampleNumericData =>
            new TheoryData<int>
            {
                -1,
                0,
                1,
                default(int)
            };

        public static TheoryData<SampleEnum> SampleEnumData =>
            new TheoryData<SampleEnum>
            {
                SampleEnum.FirstValue,
                SampleEnum.SecondValue,
                default(SampleEnum),
            };

        public static TheoryData<bool> SampleBoolData =>
            new TheoryData<bool>
            {
                true,
                false,
                default(bool),
            };

        public static TheoryData<bool?> SampleNullableBoolData =>
            new TheoryData<bool?>
            {
                true,
                false,
                null,
                default(bool?),
            };

        public class SampleClass
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
        }

        public enum SampleEnum
        {
            FirstValue,
            SecondValue,
        }
    }
}
