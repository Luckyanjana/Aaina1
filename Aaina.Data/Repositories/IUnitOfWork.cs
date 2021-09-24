using System;
using System.Collections.Generic;
using System.Text;
using Aaina.Data.Models;

namespace Aaina.Data
{
    public interface IUnitOfWork
    {
        ApplicationDbContext Context { get; }
        void CreateTransaction();
        void Commit();
        void Rollback();
        void Save();
    }
}
