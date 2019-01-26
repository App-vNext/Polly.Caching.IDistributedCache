using System;
using Xunit;

namespace Polly.Caching.Distributed.Specs.Integration
{
    public abstract class CacheRoundTripSpecsBase
    {
        protected const string OperationKey = "SomeOperationKey";

        public abstract void Should_roundtrip_this_variant_of<TResult>(TResult testValue);

        [Theory]
        [MemberData(nameof(SampleClassData))]
        public void Should_roundtrip_all_variants_of_reference_type(SampleClass testValue)
        {
            Should_roundtrip_this_variant_of<SampleClass>(testValue);
        }

        [Theory]
        [MemberData(nameof(SampleStringData))]
        public void Should_roundtrip_all_variants_of_string(String testValue)
        {
            Should_roundtrip_this_variant_of<String>(testValue);
        }

        [Theory]
        [MemberData(nameof(SampleNumericData))]
        public void Should_roundtrip_all_variants_of_numeric(int testValue)
        {
            Should_roundtrip_this_variant_of<int>(testValue);
        }

        [Theory]
        [MemberData(nameof(SampleEnumData))]
        public void Should_roundtrip_all_variants_of_enum(SampleEnum testValue)
        {
            Should_roundtrip_this_variant_of<SampleEnum>(testValue);
        }

        [Theory]
        [MemberData(nameof(SampleBoolData))]
        public void Should_roundtrip_all_variants_of_bool(bool testValue)
        {
            Should_roundtrip_this_variant_of<bool>(testValue);
        }

        [Theory]
        [MemberData(nameof(SampleNullableBoolData))]
        public void Should_roundtrip_all_variants_of_nullable_bool(bool? testValue)
        {
            Should_roundtrip_this_variant_of<bool?>(testValue);
        }

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
