using DAL.DTO.Res;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
	public class LenderServices : ILenderServices
	{
        private readonly PostgresContext _postgresContext;

        public LenderServices(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }

        public async Task<ResLenderSaldo> GetBalance(string userId)
        {
            // Cari user berdasarkan userId di database
            var user = await _postgresContext.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null; // Jika user tidak ditemukan, kembalikan null
            }

            // Isi nilai ResLenderSaldo berdasarkan data user yang ditemukan
            var resLenderSaldo = new ResLenderSaldo
            {
                Id = user.Id,
                Balance = user.Balance // Balance bertipe decimal
            };

            return resLenderSaldo; // Kembalikan data balance
        }
        public async Task<string> UpdateSaldo(string Id, decimal amount)
        {
            var lender = await _postgresContext.Users.FindAsync(Id);
            if(lender == null || lender.Role != "lender")
            {
                throw new Exception("Lender not found or invalid role");
            }

            lender.Balance += amount;
            await _postgresContext.SaveChangesAsync();

            return "Saldo update Successfully";
        }
        public async Task<List<ResListLoanDto>> GetListPeminjam()
        {
            return await _postgresContext.MstLoans
                .Where(loan => loan.Status == "requested" || loan.Status == "funded")
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    BorrowName = loan.User.Name,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status
                }).ToListAsync();
        }

    }

}
