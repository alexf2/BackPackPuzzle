using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using CsvHelper;
using DataProviders.Contract;

namespace DataProviders.CsvProvider
{
    public sealed class CsvMerchendiseProvider: IMerchendiseProvider
    {
        readonly string _filePath;

        public CsvMerchendiseProvider(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerable<Merchendise> Merchendises
        {
            get
            {
                using (var reader = new CsvReader(new StreamReader(File.OpenRead(_filePath))))
                {
                    reader.Configuration.Delimiter = ";";
                    reader.Configuration.CultureInfo = CultureInfo.CreateSpecificCulture("en-US");

                    while (reader.Read())
                    {
                        var mc = new Merchendise()
                        {
                            Name = reader.GetField("Source Name"),
                            Size = reader.GetField<int>("Size"),
                            AvgPrice = reader.GetField<decimal>("Average price of gallon"),
                            MinSize = reader.GetField<int>("Min size"),
                            IncrementStep = reader.GetField<int>("Step size")
                        };

                        var context = new ValidationContext(mc, null, null);
                        Validator.ValidateObject(mc, context);

                        yield return mc;
                    }
                }
            }
        }
    }
}
