using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServicesAbstraction
{
    public interface IAdditionalInfoService
    {
        void AddInfo(AdditionalInfo info);
        IEnumerable<AdditionalInfo> GetInfoForDay(int branchId, DateOnly date);
        IEnumerable<AdditionalInfo> GetAllInfo();
        AdditionalInfo GetById(int id);
        void UpdateInfo(AdditionalInfo info, string userName);
    }

}
