using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PricingFeed.Models;

[ApiController]
[Route("api/pricing")]
public class PricingController : ControllerBase
{
    private readonly PricingDbContext _context;

    public PricingController(PricingDbContext context)
    {
        _context = context;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string storeId, [FromQuery] string sku)
    {
        var query = _context.PricingRecords.AsQueryable();

        if (!string.IsNullOrEmpty(storeId))
            query = query.Where(p => p.StoreId == storeId);

        if (!string.IsNullOrEmpty(sku))
            query = query.Where(p => p.SKU == sku);

        var result = await query.ToListAsync();
        return Ok(result);
    }

    [HttpPut("edit/{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] PricingRecord updatedRecord)
    {
        var existingRecord = await _context.PricingRecords.FindAsync(id);
        if (existingRecord == null) return NotFound("Record not found");

        existingRecord.Price = updatedRecord.Price;
        existingRecord.Date = updatedRecord.Date;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("Another user has updated this record. Please refresh and try again.");
        }

        return Ok("Record updated successfully");
    }

}
