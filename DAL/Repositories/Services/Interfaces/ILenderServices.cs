using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
	public interface ILenderServices
	{
        Task<ResLenderSaldo> GetBalance(string userId);
        Task<string> UpdateSaldo(string lenderId, decimal amount);

        Task<List<ResListLoanDto>> GetListPeminjam();
        
    }
}
