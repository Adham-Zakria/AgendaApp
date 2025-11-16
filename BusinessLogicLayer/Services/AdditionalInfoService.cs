using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Contracts;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class AdditionalInfoService : IAdditionalInfoService
    {
        private readonly IAdditionalInfoRepository _additionalInfoRepository;

        public AdditionalInfoService(IAdditionalInfoRepository repo)
        {
            _additionalInfoRepository = repo;
        }

        public void AddInfo(AdditionalInfo info)
        {
            _additionalInfoRepository.Add(info);
        }

        public IEnumerable<AdditionalInfo> GetInfoForDay(int branchId, DateOnly date)
        {
            return _additionalInfoRepository.GetByDateAndBranch(date, branchId);
        }

        public IEnumerable<AdditionalInfo> GetAllInfo()
        {
            return _additionalInfoRepository.GetAll();
        }
        
        public AdditionalInfo GetById(int id)
        {
            return _additionalInfoRepository.GetById(id);
        }

        public void UpdateInfo(AdditionalInfo info, string userName)
        {
            var existing = _additionalInfoRepository.GetById(info.InfoId);

            if (existing != null)
            {
                existing.Position = info.Position;
                existing.FullName = info.FullName;
                existing.Code = info.Code;
                existing.Value = info.Value;
                existing.Notes = info.Notes;

                existing.LastModifiedBy = userName;   // username who modified
                existing.LastModifiedAt = DateTime.Now; //  modification time


                _additionalInfoRepository.Update(existing);
            }
        }
    }

}
