namespace Common.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        void Begin();

        void Commit();

        void Rollback();

    }
}
