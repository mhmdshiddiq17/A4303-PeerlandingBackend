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
        public async Task<ResListLoanDto> GetLoanById(string loanId)
        {
            var loan = await _postgresContext.MstLoans
                .Include(l => l.User)
                .SingleOrDefaultAsync(l => l.Id == loanId);

            if (loan == null)
            {
                throw new Exception("Loan not found");
            }

            var result = new ResListLoanDto
            {
                LoanId = loan.Id,
                UserLoan = new UserLoan
                {
                    Id = loan.User.Id,
                    Name = loan.User.Name
                },
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
                Status = loan.Status,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt,
            };

            return result;
        }
        public async Task<List<ResListLoanDto>> GetAllLoansByUserId(string status, string userId)
        {
            var loansQuery = _postgresContext.MstLoans
                .Include(l => l.User)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    UserLoan = new UserLoan
                    {
                        Id = loan.User.Id,
                        Name = loan.User.Name
                    },
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt,
                })
                .Where(loan => loan.UserLoan.Id == userId);

            if (!string.IsNullOrEmpty(status))
            {
                loansQuery = loansQuery.Where(loan => loan.Status == status);
            }

            loansQuery = loansQuery.OrderBy(loan => loan.CreatedAt);

            return await loansQuery.ToListAsync();
        }

    }
}
