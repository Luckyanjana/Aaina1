using System;
using System.Collections.Generic;
using System.Text;
using Aaina.Data.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Aaina.Data
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed;        
        private IDbContextTransaction _objTran;
        private Dictionary<string, object> _repositories;
        //Using the Constructor we are initializing the _context variable is nothing but
        //we are storing the DBContext (EmployeeDBContext) object in _context variable
        public UnitOfWork()
        {
            _context = new ApplicationDbContext();
        }
        //The Dispose() method is used to free unmanaged resources like files, 
        //database connections etc. at any time.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //This Context property will return the DBContext object i.e. (EmployeeDBContext) object
        public ApplicationDbContext Context
        {
            get { return _context; }
        }
        //This CreateTransaction() method will create a database Trnasaction so that we can do database operations by
        //applying do evrything and do nothing principle
        public void CreateTransaction()
        {
            _objTran = _context.Database.BeginTransaction();
        }
        //If all the Transactions are completed successfuly then we need to call this Commit() 
        //method to Save the changes permanently in the database
        public void Commit()
        {
            _objTran.Commit();
        }
        //If atleast one of the Transaction is Failed then we need to call this Rollback() 
        //method to Rollback the database changes to its previous state
        public void Rollback()
        {
            _objTran.Rollback();
            _objTran.Dispose();
        }
        //This Save() Method Implement DbContext Class SaveChanges method so whenever we do a transaction we need to
        //call this Save() method so that it will make the changes in the database
        public void Save()
        {
                _context.SaveChanges();
           
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();
            _disposed = true;
        }   
    }
}