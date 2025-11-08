using DataAccessLayer.Contracts;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SwaiqatAgendaAppContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(SwaiqatAgendaAppContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual T GetById(int id)
        {
            return _dbSet.Find(id) 
                ?? throw new NotFoundException("Not Found");
        }

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        //public virtual void Update(T entity)
        //{
        //    _dbSet.Update(entity);
        //    _context.SaveChanges();
        //}
        //public virtual void Update(T entity)
        //{
        //    var entry = _context.Entry(entity);

        //    // if the model doesn't follow EFCore
        //    if (entry.State == EntityState.Detached)
        //    {
        //        _dbSet.Attach(entity);
        //        entry.State = EntityState.Modified;
        //    }

        //    _context.SaveChanges();
        //}
        public virtual void Update(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                throw new Exception($"Database Update Error: {inner}");
            }
        }



        public virtual void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}
