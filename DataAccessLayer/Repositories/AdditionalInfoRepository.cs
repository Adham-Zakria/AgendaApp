using DataAccessLayer.Contracts;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class AdditionalInfoRepository :  GenericRepository<AdditionalInfo>, IAdditionalInfoRepository
    {
        //private readonly SwaiqatAgendaAppContext _context;

        public AdditionalInfoRepository(SwaiqatAgendaAppContext context) : base(context) { }
        //{
        //    _context = context;
        //}

        //public void Add(AdditionalInfo info)
        //{
        //    _context.AdditionalInfos.Add(info);
        //    _context.SaveChanges();
        //}

        public IEnumerable<AdditionalInfo> GetByDateAndBranch(DateOnly date, int branchId)
        {
            return _context.AdditionalInfos
                           .Where(a => a.TransactionDate == date && a.BranchId == branchId)
                           .ToList();
        }
    }

}
