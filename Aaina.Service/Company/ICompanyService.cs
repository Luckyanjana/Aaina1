using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Aaina.Dto;

namespace Aaina.Service
{
   public interface ICompanyService
    {
        Task<CompanyDto> GetById(int id);

        Task<Company> FirstOrDefault();

        Task<bool> IsExist(string name, int? id);

        Task<int> Add(CompanyDto requestDto);

        Task<int> Update(CompanyDto requestDto);

        List<CompanyDto> GetAll();

        void DeleteBy(int id);

        void UpdateDriveId(int companyId, string driveId);
    }
}
