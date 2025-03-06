using PricingFeed.Services;
using System.Formats.Asn1;
using System.Globalization;

namespace PricingFeed.Interfaces
{

        // Services/PriceFeedService.cs
        public class PriceFeedService : IPriceFeedService
        {
            private readonly DbContext _dbContext;
            private readonly IBlobStorageService _blobStorage;

            public async Task ProcessCsvFile(IFormFile file)
            {
                // Parse CSV using CsvHelper
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<PriceRecord>().ToList();

                // Save to DB
                await _dbContext.PriceRecords.AddRangeAsync(records);
                await _dbContext.SaveChangesAsync();

                // Save file to Blob Storage
                await _blobStorage.UploadFileAsync(file.FileName, stream);
            }

            public async Task<List<PriceRecord>> SearchPriceRecords(PriceSearchCriteria criteria)
            {
                var query = _dbContext.PriceRecords.AsQueryable();

                if (!string.IsNullOrEmpty(criteria.StoreId))
                    query = query.Where(r => r.StoreId == criteria.StoreId);
                // Similar filters for SKU and Date

                return await query.ToListAsync();
            }
        }
    }
