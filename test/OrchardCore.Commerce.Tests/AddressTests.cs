using OrchardCore.Commerce.AddressDataType;
using OrchardCore.Commerce.AddressDataType.Abstractions;
using Shouldly;
using Xunit;

namespace OrchardCore.Commerce.Tests;

public class AddressTests
{
    [Fact]
    public void AddressFormatterShouldOmitBlankEntries()
    {
        IAddressFormatter formatter = new DefaultAddressFormatter();

        void VerifyAddress(string expectation, Address address) =>
            formatter
                .Format(address)
                .Replace(System.Environment.NewLine, "\n")
                .ShouldBe(expectation);

        VerifyAddress(
            "EXAMPLE STR. 1",
            new Address { StreetAddress1 = "Example str. 1" });

        VerifyAddress(
            "EXAMPLE STR. 1\nHOMETOWN",
            new Address { StreetAddress1 = "Example str. 1", City = "Hometown" });

        VerifyAddress(
            "EXAMPLE STR. 1\nHOMETOWN\nNORTHERN",
            new Address { StreetAddress1 = "Example str. 1", City = "Hometown", Region = "Northern" });

        VerifyAddress(
            "EXAMPLE STR. 1\nHOMETOWN CAPITAL 00000\nNORTHERN",
            new Address
            {
                StreetAddress1 = "Example str. 1",
                City = "Hometown",
                Province = "Capital",
                PostalCode = "00000",
                Region = "Northern",
            });

        VerifyAddress(
            "ACQUISITIONS\nACME INC.\nEXAMPLE STR. 1\nBUILDING 5, DOOR 401\nHOMETOWN CAPITAL 00000\nNORTHERN",
            new Address
            {
                Department = "Acquisitions",
                Company = "ACME Inc.",
                StreetAddress1 = "Example str. 1",
                StreetAddress2 = "Building 5, Door 401",
                City = "Hometown",
                Province = "Capital",
                PostalCode = "00000",
                Region = "Northern",
                Name = "This is not used here so whatever.",
            });
    }
}
