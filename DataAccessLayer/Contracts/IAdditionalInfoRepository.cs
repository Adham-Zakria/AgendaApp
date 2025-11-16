using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Contracts
{
    public interface IAdditionalInfoRepository: IGenericRepository<AdditionalInfo>
    {
        //void Add(AdditionalInfo info);
        IEnumerable<AdditionalInfo> GetByDateAndBranch(DateOnly date, int branchId);
    }

}
