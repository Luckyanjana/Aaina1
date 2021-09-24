using Aaina.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Service
{
    public interface IFormBuilderService
    {
        Task<FormBuilderDto> GetById(int id);
        Task<int> Add(FormBuilderDto requestDto);
        Task<int> Update(FormBuilderDto requestDto);
        List<FormBuilderDto> GetAll(int companyId);
        void DeleteBy(int id);

        void DeleteAtterbute(int id);

        void DeleteAtterbuteLook(int id);
    }
}
