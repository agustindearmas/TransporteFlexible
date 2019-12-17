using Common.Repositories.Interfaces;
using System;
using System.Transactions;

namespace DataAccess.Concrete
{
    [Serializable]
    public class ClosedUnitOfWorkException : Exception
    {
        public ClosedUnitOfWorkException() { }
        public ClosedUnitOfWorkException(string message) : base(message) { }
        public ClosedUnitOfWorkException(string message, Exception inner) : base(message, inner) { }
        protected ClosedUnitOfWorkException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }


    public class UnitOfWorkContext : IUnitOfWork
    {
        private TransactionScope Active;
        private Boolean Transactioning { get { return TranCount > 0; } }
        private Int16 TranCount;

        public UnitOfWorkContext()
        {
            TranCount = 0;
        }

        public void Begin()
        {
            try
            {
                if (!Transactioning)
                {
                    Active = new TransactionScope(TransactionScopeOption.RequiresNew);
                }

                TranCount += 1;

            }
            catch
            {
                if (Active != null)
                {
                    Active.Dispose();
                    Active = null;
                }
                throw;
            }
        }

        public void Commit()
        {
            if (Transactioning)
            {
                if (TranCount == 1)
                {
                    try
                    {
                        Active.Complete();
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        Active.Dispose();
                        Active = null;
                    }
                }
                TranCount -= 1;
            }
            else
            {
                throw new ClosedUnitOfWorkException("Transaction already closed");
            }
        }

        public void Rollback()
        {
            if (Transactioning)
            {
                if (Active != null)
                {
                    Active.Dispose();
                    Active = null;
                    TranCount = 0;
                }
            }
            else
            {
                throw new ClosedUnitOfWorkException("Transaction already closed");
            }
        }
    }
}