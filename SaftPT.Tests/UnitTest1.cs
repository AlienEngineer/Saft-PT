using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Framework;
using SaftPT.Tests.Helpers;

namespace SaftPT.Tests
{
    [TestFixture]
    class SaftParserTests : UnderTest<SaftParser>
    {
        protected override void SetDependencies(IUnderTest<SaftParser> subject) { }

        private static Stream GetSaftFileStream()
        {
            var writer = new StreamWriter(new MemoryStream());

            writer.Write(@"<?xml version=""1.0"" encoding=""Windows - 1252"" standalone=""yes""?>
<AuditFile xmlns=""urn:OECD:StandardAuditFile-Tax:PT_1.03_01""> 
    <MasterFiles>
        <Customer>
          <CustomerID>1/123456798</CustomerID>
          <AccountID>Desconhecido</AccountID>
          <CustomerTaxID>123456798</CustomerTaxID>
          <CompanyName>Test customer</CompanyName>
          <BillingAddress>
            <AddressDetail>Test street</AddressDetail>
            <City>Test city</City>
            <PostalCode>test postal code</PostalCode>
            <Country>PT</Country>
          </BillingAddress>
          <Email>someEmail@test.pt</Email>
          <SelfBillingIndicator>0</SelfBillingIndicator>
        </Customer>
        <Customer>
          <CustomerID>1/1234567</CustomerID>
          <AccountID>Desconhecido</AccountID>
          <CustomerTaxID>1234567</CustomerTaxID>
          <CompanyName>Test customer 1</CompanyName>
          <BillingAddress>
            <AddressDetail>Test street 1</AddressDetail>
            <City>Test city</City>
            <PostalCode>test postal code</PostalCode>
            <Country>PT</Country>
          </BillingAddress>
          <Email>someEmail@test.pt</Email>
          <SelfBillingIndicator>0</SelfBillingIndicator>
        </Customer>
    </MasterFiles>
    <SourceDocuments>
        <SalesInvoices>
          <NumberOfEntries>384</NumberOfEntries>
          <TotalDebit>8913.937755</TotalDebit>
          <TotalCredit>124402.260222</TotalCredit>
          <Invoice>
            <InvoiceNo>FT 2014FT1/1</InvoiceNo>
            <DocumentStatus>
              <InvoiceStatus>N</InvoiceStatus>
              <InvoiceStatusDate>2014-08-10T11:54:02</InvoiceStatusDate>
              <SourceID>sérgio</SourceID>
              <SourceBilling>P</SourceBilling>
            </DocumentStatus>
            <Hash>YCf3flAHeNUmB4jyd1TZihgu+hbdLEZwWirHtSAQoFcBKyXP/skH20CLPto3u2A9YrjhhmNGDCEojMnKldRnJ72MeNmiWNLsNI8cYilmyZkauZdrsmsb9MvBbGD7speLGwazINPxbzIdyD7IQ8/DhR+FTPh0V02oPMJ/cZeilUE=</Hash>
            <HashControl>1</HashControl>
            <Period>8</Period>
            <InvoiceDate>2014-08-10</InvoiceDate>
            <InvoiceType>FT</InvoiceType>
            <SpecialRegimes>
              <SelfBillingIndicator>0</SelfBillingIndicator>
              <CashVATSchemeIndicator>0</CashVATSchemeIndicator>
              <ThirdPartiesBillingIndicator>0</ThirdPartiesBillingIndicator>
            </SpecialRegimes>
            <SourceID>admin</SourceID>
            <EACCode>86906</EACCode>
            <SystemEntryDate>2014-08-10T11:54:02</SystemEntryDate>
            <CustomerID>1/262742268</CustomerID>
            <Line>
              <LineNumber>1</LineNumber>
              <ProductCode>Fisioterapia</ProductCode>
              <ProductDescription>Sessão de Fisioterapia</ProductDescription>
              <Quantity>19.0000</Quantity>
              <UnitOfMeasure>Unidade</UnitOfMeasure>
              <UnitPrice>38.80</UnitPrice>
              <TaxPointDate>2014-08-10</TaxPointDate>
              <Description>Sessão de Fisioterapia</Description>
              <CreditAmount>737.200000</CreditAmount>
              <Tax>
                <TaxType>IVA</TaxType>
                <TaxCountryRegion>PT</TaxCountryRegion>
                <TaxCode>ISE</TaxCode>
                <TaxPercentage>0.00</TaxPercentage>
              </Tax>
              <TaxExemptionReason>Isento Artigo 9.º do CIVA</TaxExemptionReason>
              <SettlementAmount>22.800000</SettlementAmount>
            </Line>
            <DocumentTotals>
              <TaxPayable>0.000000</TaxPayable>
              <NetTotal>737.200000</NetTotal>
              <GrossTotal>737.20</GrossTotal>
              <Settlement>
                <SettlementDate>2014-08-10</SettlementDate>
              </Settlement>
            </DocumentTotals>
          </Invoice>
        </SalesInvoices>
    </SourceDocuments>
</AuditFile>
");
            writer.Flush();
            writer.BaseStream.Position = 0;

            return writer.BaseStream;
        }

        [Test]
        public void When_parting_a_saft_xml_it_should_return_the_list_of_clients()
        {
            // Arrange
            var stream = GetSaftFileStream();

            // Act
            var result = Subject.Parse(stream);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.MasterFiles, Is.Not.Null);
            Assert.That(result.MasterFiles.Customers, Is.Not.Null);
            Assert.That(result.MasterFiles.Customers.Count(), Is.EqualTo(2));

            var customers = result.MasterFiles.Customers.ToList();

            AssertComplex.That(customers[0],
                has => has.Field(e => e.CustomerId, Is.EqualTo("1/123456798")),
                has => has.Field(e => e.CustomerTaxId, Is.EqualTo("123456798")),
                has => has.Field(e => e.CompanyName, Is.EqualTo("Test customer"))
            );

            AssertComplex.That(customers[1],
                has => has.Field(e => e.CustomerId, Is.EqualTo("1/1234567")),
                has => has.Field(e => e.CustomerTaxId, Is.EqualTo("1234567")),
                has => has.Field(e => e.CompanyName, Is.EqualTo("Test customer 1"))
            );
        }

        [Test]
        public void When_parting_a_saft_xml_it_should_return_the_list_of_documents()
        {
            // Arrange
            var stream = GetSaftFileStream();

            // Act
            var result = Subject.Parse(stream);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SourceDocuments, Is.Not.Null);
            Assert.That(result.SourceDocuments.Invoices, Is.Not.Null);
            Assert.That(result.SourceDocuments.Invoices.Count(), Is.EqualTo(1));

            var invoices = result.SourceDocuments.Invoices.ToList();
            
            AssertComplex.That(invoices[0],
                has => has.Field(e => e.GrossTotal, Is.EqualTo(737.20)),
                has => has.Field(e => e.InvoiceDate, Is.EqualTo(new DateTime(2014, 08, 10))),
                has => has.Field(e => e.InvoiceNo, Is.EqualTo("FT 2014FT1/1")),
                has => has.Field(e => e.InvoiceType, Is.EqualTo("FT")),
                has => has.Field(e => e.NetTotal, Is.EqualTo(737.20)),
                has => has.Field(e => e.TaxPayable, Is.EqualTo(0.0))
            );

        }
    }

    public class SaftParser
    {
        public AuditFile Parse(Stream stream)
        {
            var customers = new List<Customer>();
            var invoices = new List<Invoice>();
            using (var xmlReader = new XmlDeserializer(stream, Encoding.GetEncoding("Windows-1252")))
            {
                xmlReader.GotoBeginingOf("MasterFiles");

                while (xmlReader.Read() && !xmlReader.IsEndingOf("MasterFiles"))
                {
                    if (xmlReader.IsBeginingOf("Customer"))
                    {
                        customers.Add(xmlReader.Deserialize<Customer>());
                    }
                }

                xmlReader.GotoBeginingOf("SourceDocuments");

                while (xmlReader.Read() && !xmlReader.IsEndingOf("SourceDocuments"))
                {
                    if (xmlReader.IsBeginingOf("Invoice"))
                    {
                        invoices.Add(xmlReader.Deserialize<Invoice>());
                    }
                }
            }

            return new AuditFile
            {
                MasterFiles = new MasterFile
                {
                    Customers = customers
                },
                SourceDocuments = new SourceDocument
                {
                    NumberOfEntries = 10,
                    TotalCredit = 10.2,
                    TotalDebit = 23.123,
                    Invoices = invoices
                }
            };
        }
    }

    class XmlDeserializer : IDisposable
    {
        private readonly XmlReader _reader;

        public XmlDeserializer(Stream stream, Encoding encoding)
            : this(new StreamReader(stream, encoding)) { }

        public XmlDeserializer(TextReader reader)
            : this(XmlReader.Create(reader)) { }

        public XmlDeserializer(XmlReader reader)
        {
            _reader = reader;
        }

        public void GotoBeginingOf(string elementName)
        {
            while (!IsBeginingOf(elementName))
            {
                _reader.Read();
            }
        }

        public bool IsEndingOf(string elementName)
        {
            return _reader.NodeType == XmlNodeType.EndElement && _reader.Name == elementName;
        }

        public bool IsBeginingOf(string elementName)
        {
            return _reader.NodeType == XmlNodeType.Element && _reader.Name == elementName;
        }

        public T Deserialize<T>() where T : new()
        {
            var customer = new T();

            var properties = typeof(T)
                .GetProperties()
                .ToDictionary(e => e.Name.ToLowerInvariant());

            var intSetterCount = 0;

            while (intSetterCount < properties.Count && _reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element)
                {
                    var property = GetProperty(_reader, properties);
                    if (TrySetPropertyValue(_reader, property, customer))
                    {
                        intSetterCount++;
                    }
                }
            }

            return customer;
        }

        private static PropertyInfo GetProperty(
            XmlReader xmlReader,
            IReadOnlyDictionary<string, PropertyInfo> properties)
        {
            PropertyInfo property;
            properties.TryGetValue(xmlReader.Name.ToLowerInvariant(), out property);
            return property;
        }

        private static bool TrySetPropertyValue(XmlReader xmlReader, PropertyInfo property, object customer)
        {
            if (property == null)
            {
                return false;
            }

            var value = GetValue(xmlReader, property.PropertyType);

            property.SetValue(customer, value);

            return true;
        }

        private static object GetValue(XmlReader xmlReader, Type propertyType)
        {
            switch (propertyType.Name)
            {
                case "DateTime":
                    return DateTime.Parse(xmlReader.ReadString());
                case "Double":
                    return double.Parse(xmlReader.ReadString(), CultureInfo.InvariantCulture);
                default:
                    return xmlReader.ReadString();
            }
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public bool Read()
        {
            return _reader.Read();
        }
    }

    public class AuditFile
    {
        public MasterFile MasterFiles { get; set; }
        public SourceDocument SourceDocuments { get; internal set; }
    }

    public class SourceDocument
    {
        public int NumberOfEntries { get; set; }
        public double TotalCredit { get; set; }
        public double TotalDebit { get; set; }

        public IEnumerable<Invoice> Invoices { get; set; }
    }

    public class Invoice
    {
        public DateTime InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceNo { get; set; }
        public double TaxPayable { get; set; }
        public double GrossTotal { get; set; }
        public double NetTotal { get; set; }
    }

    public class MasterFile
    {
        public IEnumerable<Customer> Customers { get; set; }
    }

    public class Customer
    {
        public string CustomerId { get; set; }
        public string CustomerTaxId { get; set; }
        public string CompanyName { get; set; }
    }
}
