namespace Examination_System.Data.Repositories
{
    public interface IService<T>
    {
        public List<T> GetAll();


        public T GetById(int id);


        public T Create(T entity);

        public T Update(int id,T entity);

        public void Delete(int id);
    }
}
