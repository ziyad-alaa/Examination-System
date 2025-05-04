namespace Examination_System.Data.Interfaces
{
    public interface IService<T>
    {
        public List<T> GetAll();
        public T GetById(int id);
        public void Create(T entity);
        public void Delete(int id);
        public void Update(int id, T entity);
    }
}
