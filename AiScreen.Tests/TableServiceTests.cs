using System;
using Component_TableListing.Services;
using Xunit;

namespace AiScreen.Tests
{
    public class TableServiceTests
    {
        private TableService CreateService()
        {
            return new TableService(null);
        }

        [Theory]
        [InlineData("int", 0)]
        [InlineData("decimal", 0)]
        [InlineData("varchar", "")]
        public void GetDefaultValue_KnownTypes_ReturnsExpected(string type, object expected)
        {
            var service = CreateService();

            var (status, message, result) = service.GetDefaultValue(type);

            Assert.True(status);
            Assert.Equal("OK", message);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetDefaultValue_UnknownType_ReturnsError()
        {
            var service = CreateService();

            var (status, message, result) = service.GetDefaultValue("unknown");

            Assert.False(status);
            Assert.Equal("Unrecognized data type: 'unknown'", message);
            Assert.Null(result);
        }
    }
}
