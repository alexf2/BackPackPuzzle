using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using CsvHelper;
using DataProviders.Contract;

namespace DataProviders.CsvProvider
{
    /// <summary>
    /// Represent read only merchendise provider against CSV files.
    /// </summary>
    public sealed class CsvMerchendiseProvider: IMerchendiseProvider
    {
        readonly string _filePath;

        /// <summary>
        /// Initializes an instance of the provider.
        /// </summary>
        /// <param name="filePath">Path to a CSV file of predefined format, containing merchendise.</param>
        public CsvMerchendiseProvider(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Returns a collection of merchendise, loaded from underlying storage.
        /// </summary>
        public IEnumerable<Merchendise> Merchendise
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
                            AvgPrice = reader.GetField<double>("Average price of gallon"),
                            MinSize = reader.GetField<int>("Min size"),
                            IncrementStep = reader.GetField<int>("Step size")
                        };

                        var context = new ValidationContext(mc, null, null);
                        Validator.ValidateObject(mc, context); //enforcing fields constraints

                        yield return mc;
                    }
                }
            }
        }
    }
}
