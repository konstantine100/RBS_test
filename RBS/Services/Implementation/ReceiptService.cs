using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.Data;
using RBS.DTOs;
using RBS.Models;
using RBS.Services.Interfaces;

namespace RBS.Services.Implenetation
{
    public class ReceiptService : IReceiptService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReceiptService(DataContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReceiptDTO>> GetAllReceiptsAsync()
        {
            var receipts = await _context.Receipts
                .Include(x => x.CustomerDetails)
                .Include(x => x.TableItems)
                .Include(x => x.ChairItems)
                .Include(x => x.SpaceItems)
                .ToListAsync();

            var receiptDtos = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);

            return receiptDtos;
        }

        public async Task<ReceiptDTO> GetReceiptByIdAsync(int id)
        {
            var receipt = await _context.Receipts
                .Include(x => x.CustomerDetails)
                .Include(x => x.TableItems)
                .Include(x => x.ChairItems)
                .Include(x => x.SpaceItems)
                .FirstOrDefaultAsync(r => r.Id == id);

            var receiptDto = _mapper.Map<ReceiptDTO>(receipt);

            return receiptDto;
        }

        public async Task<IEnumerable<ReceiptDTO>> GetReceiptsByAmountRangeAsync(decimal minAmount, decimal maxAmount)
        {
            var receipts = await _context.Receipts
                .Where(r => r.TotalAmount >= minAmount && r.TotalAmount <= maxAmount)
                .Include(x => x.CustomerDetails)
                .Include(x => x.TableItems)
                .Include(x => x.ChairItems)
                .Include(x => x.SpaceItems)
                .ToListAsync();

            var receiptDtos = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);

            return receiptDtos;
        }

        public async Task<IEnumerable<ReceiptDTO>> GetReceiptsByBookingIdAsync(int bookingId)
        {
            var receipts = await _context.Receipts
                .Where(r => r.BookingId == bookingId)
                .Include(x => x.CustomerDetails)
                .Include(x => x.TableItems)
                .Include(x => x.ChairItems)
                .Include(x => x.SpaceItems)
                .ToListAsync();

            var receiptDtos = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);

            return receiptDtos;
        }

        public async Task<IEnumerable<ReceiptDTO>> GetReceiptsByChairIdAsync(int chairId)
        {
            var receipts = await _context.Receipts
                .Include(x => x.CustomerDetails)
                .Include(x => x.TableItems)
                .Include(x => x.ChairItems)
                .Include(x => x.SpaceItems)
                .Where(r => r.ChairItems.Any(c => c.Id == chairId))
                .ToListAsync();

            var receiptDtos = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);

            return receiptDtos;
        }

        public async Task<IEnumerable<ReceiptDTO>> GetReceiptsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var receipts = await _context.Receipts
                .Include(x => x.CustomerDetails)
                .Include(x => x.TableItems)
                .Include(x => x.ChairItems)
                .Include(x => x.SpaceItems)
                .Where(r => r.Date >= startDate && r.Date <= endDate)
                .ToListAsync();

            var receiptDtos = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);

            return receiptDtos;
        }

        public async Task<IEnumerable<ReceiptDTO>> GetReceiptsBySpaceIdAsync(int spaceId)
        {
            var receipts = await _context.Receipts
                .Include(x => x.CustomerDetails)
                .Include(x => x.TableItems)
                .Include(x => x.ChairItems)
                .Include(x => x.SpaceItems)
                .Where(r => r.SpaceItems.Any(s => s.Id == spaceId))
                .ToListAsync();

            var receiptDtos = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);

            return receiptDtos;
        }

        public async Task<IEnumerable<ReceiptDTO>> GetReceiptsByTableIdAsync(int tableId)
        {
            var receipts = await _context.Receipts
                .Include(x => x.CustomerDetails)
                .Include(x => x.TableItems)
                .Include(x => x.ChairItems)
                .Include(x => x.SpaceItems)
                .Where(r => r.TableItems.Any(t => t.Id == tableId))
                .ToListAsync();

            var receiptDtos = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);

            return receiptDtos;
        }

        public async Task<IEnumerable<ReceiptDTO>> GetReceiptsByUserIdAsync(int userId)
        {
            var receipts = await _context.Receipts
                .Include(x => x.CustomerDetails)
                .Include(x => x.TableItems)
                .Include(x => x.ChairItems)
                .Include(x => x.SpaceItems)
                .Where(r => r.CustomerDetails!.Id == userId)
                .ToListAsync();

            var receiptDtos = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);

            return receiptDtos;
        }

        public async Task<ReceiptDTO> CreateReceiptAsync(ReceiptDTO receiptDto)
        {
            var receipt = _mapper.Map<Receipt>(receiptDto);
            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            return _mapper.Map<ReceiptDTO>(receipt);
        }

        public async Task<ReceiptDTO> UpdateReceiptAsync(int id, ReceiptDTO receiptDto)
        {
            var receipt = await _context.Receipts.FindAsync(id) ?? throw new KeyNotFoundException($"Receipt with ID {id} not found.");
            _mapper.Map(receiptDto, receipt);
            _context.Receipts.Update(receipt);
            await _context.SaveChangesAsync();

            return _mapper.Map<ReceiptDTO>(receipt);
        }

        public async Task<bool> DeleteReceiptAsync(int id)
        {
            var receipt = await _context.Receipts.FindAsync(id) ?? throw new KeyNotFoundException($"Receipt with ID {id} not found.");
            _context.Receipts.Remove(receipt);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}