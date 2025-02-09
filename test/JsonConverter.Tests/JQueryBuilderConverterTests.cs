using DynamicWhere.Core.Models;
using Shared.Tests;
using System.Text.Json;
using Shouldly;
using DynamicWhere.JsonConverter;
using DynamicWhere.JsonConverter.ValueParsers;

namespace JsonConverter.Tests
{
    public class JQueryBuilderConverterTests
    {
        private readonly JsonSerializerOptions options;
        public JQueryBuilderConverterTests()
        {
            var customParsers = new Dictionary<string, IValueParser>
            {
                { "int", new PrimitiveValueParser() }
            };

            options = new() { Converters = { new JQueryBuilderConverter(customParsers) } };
        }

        [Fact]
        public void GivenPrimitiveDataTypes_ThenDeserializeAsNormal()
        {
            // Arrange
            var json = EmbeddedResourceHelper.GetEmbeddedResourceContent("JsonConverter.Tests.TestData.primitive-types.json");

            // Act
            DynamicGroup group = JsonSerializer.Deserialize<DynamicGroup>(json, options)!;

            // Assert

            group.Condition.ShouldBe("AND");
            group.Rules.ShouldSatisfyAllConditions(x =>
            {
                x.Count.ShouldBe(4);
                x[0].ShouldSatisfyAllConditions(x =>
                {
                    x.FieldName.ShouldBe("name");
                    x.Operator.ShouldBe("equal");
                    x.Value.ShouldBe("value1");
                });
                x[1].ShouldSatisfyAllConditions(x =>
                {
                    x.FieldName.ShouldBe("age");
                    x.Operator.ShouldBe("less");
                    x.Value.ShouldBe(20);
                });
                x[2].ShouldSatisfyAllConditions(x =>
                {
                    x.FieldName.ShouldBe("dateOnly");
                    x.Operator.ShouldBe("less");
                    x.Value.ShouldBe(new DateTime(2024, 08, 15));
                });
                x[3].ShouldSatisfyAllConditions(x =>
                {
                    x.FieldName.ShouldBe("datetime");
                    x.Operator.ShouldBe("greater");
                    x.Value.ShouldBe(new DateTime(2024, 08, 15, 15, 20, 00));
                });
            });
            group.Groups.ShouldSatisfyAllConditions(x =>
            {
                x.Count.ShouldBe(1);
                x.First().Condition.ShouldBe("OR");
                x.First().Rules.ShouldSatisfyAllConditions(x =>
                {
                    x.Count.ShouldBe(2);
                    x[0].ShouldSatisfyAllConditions(x =>
                    {
                        x.FieldName.ShouldBe("gender");
                        x.Operator.ShouldBe("equal");
                        x.Value.ShouldBe(false);
                    });
                    x[1].ShouldSatisfyAllConditions(x =>
                    {
                        x.FieldName.ShouldBe("money");
                        x.Operator.ShouldBe("greater");
                        x.Value.ShouldBe(3000.00);
                    });
                });
            });
        }

        [Fact]
        public void Should_Parse_StringValue()
        {
            // Arrange
            string json = "{\"condition\": \"AND\", \"rules\": [{\"field\": \"name\", \"operator\": \"equal\", \"value\": \"John\", \"type\": \"string\"}]}";

            // Act
            var group = JsonSerializer.Deserialize<DynamicGroup>(json, options);

            // Assert
            group.ShouldNotBeNull();
            group.Rules.Count.ShouldBe(1);
            group.Rules[0].Value.ShouldBe("John");
            group.Rules[0].Data.ShouldBeNull();
        }

        [Fact]
        public void Should_Parse_IntegerValue()
        {
            // Arrange
            string json = "{\"condition\": \"AND\", \"rules\": [{\"data\": {\"extra\": \"test\"}, \"field\": \"age\", \"operator\": \"greater\", \"value\": 30, \"type\": \"int\"}]}";

            // Act
            var group = JsonSerializer.Deserialize<DynamicGroup>(json, options);

            // Assert
            group.ShouldNotBeNull();
            group.Rules.Count.ShouldBe(1);
            group.Rules[0].Value.ShouldBe(30);

            var extraInfo = group.Rules[0].Data as Dictionary<string, object?>; ;
            extraInfo.ShouldNotBeNull();
            extraInfo["extra"].ShouldBe("test");
        }

        [Fact]
        public void Should_Parse_ObjectValue()
        {
            // Arrange
            string json = "{\"condition\": \"AND\", \"rules\": [{\"field\": \"customer\", \"operator\": \"equal\", \"value\": {\"id\": 123, \"name\": \"John Doe\"}, \"type\": \"customer\"}]}";

            // Act
            var group = JsonSerializer.Deserialize<DynamicGroup>(json, options);

            // Assert
            group.ShouldNotBeNull();
            group.Rules.Count.ShouldBe(1);
            var customer = group.Rules[0].Value as Dictionary<string, object?>;
            customer.ShouldNotBeNull();
            customer["id"].ShouldBe(123);
            customer["name"].ShouldBe("John Doe");
        }

        [Fact]
        public void Should_Parse_ArrayValue()
        {
            // Arrange
            string json = "{\"condition\": \"AND\", \"rules\": [{\"field\": \"numbers\", \"operator\": \"in\", \"value\": [12, 15], \"type\": \"int\"}]}";

            // Act
            var group = JsonSerializer.Deserialize<DynamicGroup>(json, options);

            // Assert
            group.ShouldNotBeNull();
            group.Rules.Count.ShouldBe(1);
            var numbers = group.Rules[0].Value as List<object>;
            numbers.ShouldNotBeNull();
            numbers.Count.ShouldBe(2);
            numbers[0].ShouldBe(12);
            numbers[1].ShouldBe(15);
        }

        [Fact]
        public void Should_Parse_NestedGroups()
        {
            // Arrange
            string json = @"
            {
                ""condition"": ""AND"",
                ""rules"": [
                    {
                        ""condition"": ""OR"",
                        ""rules"": [
                            {""field"": ""name"", ""operator"": ""equal"", ""value"": ""John"", ""type"": ""string""},
                            {""field"": ""age"", ""operator"": ""greater"", ""value"": 30, ""type"": ""int""}
                        ]
                    }
                ]
            }";

            // Act
            var group = JsonSerializer.Deserialize<DynamicGroup>(json, options);

            // Assert
            group.ShouldNotBeNull();
            group.Groups.Count.ShouldBe(1);
            var nestedGroup = group.Groups[0];
            nestedGroup.Rules.Count.ShouldBe(2);
            nestedGroup.Rules[0].Value.ShouldBe("John");
            nestedGroup.Rules[1].Value.ShouldBe(30);
        }

        [Fact]
        public void Should_Handle_EmptyRules()
        {
            // Arrange
            string json = "{\"condition\": \"AND\", \"rules\": []}";

            // Act
            var group = JsonSerializer.Deserialize<DynamicGroup>(json, options);

            // Assert
            group.ShouldNotBeNull();
            group.Rules.Count.ShouldBe(0);
        }

        [Fact]
        public void Should_Serialize_DynamicGroup()
        {
            // Arrange
            var dynamicGroup = new DynamicGroup
            {
                Condition = "AND",
                Rules = new List<DynamicRule>
                {
                    new DynamicRule { FieldName = "name", Operator = "equal", Value = "value1" },
                    new DynamicRule { FieldName = "age", Operator = "less", Value = 20 }
                },
                Groups = new List<DynamicGroup>
                {
                    new DynamicGroup
                    {
                        Condition = "OR",
                        Rules = new List<DynamicRule>
                        {
                            new DynamicRule { FieldName = "gender", Operator = "equal", Value = false },
                            new DynamicRule { FieldName = "money", Operator = "greater", Value = 3000.00 }
                        }
                    }
                }
            };

            // Act
            string json = JsonSerializer.Serialize(dynamicGroup, options);

            // Assert
            json.ShouldNotBeNullOrEmpty();
        }
    }
}
