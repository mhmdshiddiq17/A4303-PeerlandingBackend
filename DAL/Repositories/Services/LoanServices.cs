using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class LoanServices : ILoanServices
    {
        private readonly PostgresContext _postgresContext;
        

        public LoanServices(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }

        public async Task<string> CreateLoan(ReqLoanDto loan)
        {
            var newLoan = new MstLoans
            {
                BorrowId = loan.BorrowId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
            };

            await _postgresContext.AddAsync(newLoan);
            await _postgresContext.SaveChangesAsync();

            return newLoan.BorrowId;

        }

        public async Task<List<ResListLoanDto>> LoanList(string status)
        {
            //var statusLoans = await _postgresContext.MstLoans.SingleOrDefaultAsync(loans => loans.User == Status);
            
            
                var loans = await _postgresContext.MstLoans.Include(l =>
                 l.User).Where(loan => status == null || loan.Status == status).OrderByDescending(loan => loan.CreatedAt).Select(loan => new ResListLoanDto
                 {
                     LoanId = loan.Id,
                     BorrowName = loan.User.Name,
                     Amount = loan.Amount,
                     InterestRate = loan.InterestRate,
                     Duration = loan.Duration,
                     Status = loan.Status,
                     CreatedAt = loan.CreatedAt,
                     UpdatedAt = loan.UpdatedAt,
                 }).ToListAsync();
                return loans;
            
            
        }


        public async Task<string> UpdateLoan(string userId, ReqUpdateLoanDto loanUpdate)
        {
            var existingUser = await _postgresContext.MstLoans.SingleOrDefaultAsync(user => user.Id == userId);
            if (existingUser == null)
            {
                throw new Exception("loan not found");
            }
            existingUser.Status = loanUpdate.Status ?? existingUser.Status;
            existingUser.UpdatedAt = DateTime.UtcNow;


             _postgresContext.MstLoans.Update(existingUser);
            await _postgresContext.SaveChangesAsync();

            return loanUpdate.Status;

        }
    }
}
