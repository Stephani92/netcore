using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pro.Repository.Data;
using Pro_Domain.Entities;

namespace Pro.Repository.Repository
{
    public class BaseRepository : IBaseRepository
    {
        private readonly ProDbContext _data;

        public BaseRepository(ProDbContext data)
        {
            _data = data;
            _data.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

            //geral
        public void Add<T>(T entity) where T : class
        {
            _data.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _data.Remove(entity);
        }
        public void DeleteRange<T>(T[] entityArray) where T : class
        {
            _data.RemoveRange(entityArray);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _data.SaveChangesAsync())>0;
        }

        public void Update<T>(T entity) where T : class
        {
            _data.Update<T>(entity);
        }

        //Eventos
        public async Task<Evento[]> GetAllEventoAsync(bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _data.Eventos.Include(c=> c.Lotes)
            .Include(c=> c.RedesSociais);
            
            if (includePalestrantes)
            {   
                query = query.Include(pe=> pe.EventoPalestrantes)
                .ThenInclude(p=>p.Palestrante);
            }
            query = query.OrderByDescending(c=>c.Data);
            return await query.ToArrayAsync();
        }

        public async Task<Evento[]> GetAllEventoAsyncByTema(string tema, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _data.Eventos
            .Include(c=>c.Lotes)
            .Include(c=>c.RedesSociais);
            if (includePalestrantes)
            {   
                query = query.Include(pe=> pe.EventoPalestrantes)
                .ThenInclude(p=>p.Palestrante);
            }
            query = query.OrderBy(c=>c.Id)
                        .Where(c=>c.Tema.ToLower().Contains(tema.ToLower()));
            return await query.ToArrayAsync();
        }
        public async Task<Evento> GetEventoAsyncById(int EventoId, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _data.Eventos
            .Include(c=>c.Lotes)
            .Include(c=>c.RedesSociais);
            if (includePalestrantes)
            {   
                query = query.Include(pe=> pe.EventoPalestrantes)
                .ThenInclude(p=>p.Palestrante);
            }
            query = query.OrderBy(c=>c.Id)
                        .Where(c=> c.Id == EventoId);
            return await query.FirstOrDefaultAsync();
        }
        //Palestrante
        public async Task<Palestrante[]> GetAllPalestrantesAsyncByName(string name, bool includeEventos)
        {
            IQueryable<Palestrante> query = _data.Palestrantes
            .Include(c=>c.RedesSociais);
            if (includeEventos)
            {   
                query = query.Include(pe=> pe.EventoPalestrantes)
                .ThenInclude(p=>p.Evento);
            }
            query = query.Where(c=> c.Nome.ToLower().Contains(name.ToLower()));
            return await query.ToArrayAsync();
        }
        public async Task<Job> GetAllJobAsyncById(int custId, int userId)
        {
            IQueryable<Job> query = _data.Jobs;
            
            query = query.Where(c=> c.CustomerId == custId && c.UserId == userId);
            return await query.FirstOrDefaultAsync();
        }

        

        public async Task<Palestrante> GetAllPalestranteByIdAsync(int PalestranteId, bool includeEventos)
        {
            IQueryable<Palestrante> query = _data.Palestrantes
            .Include(c=>c.RedesSociais);
            if (includeEventos)
            {   
                query = query.Include(pe=> pe.EventoPalestrantes)
                .ThenInclude(p=>p.Evento);
            }
            query = query.OrderBy(c=>c.Nome)
                         .Where(c=> c.Id == PalestranteId);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Customer> GetCustomerAsyncById(int CustId)
        {
            IQueryable<Customer> query = _data.Customers;           
            query = query.Where(c => c.Id == CustId);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Job[]> GetJobsAsyncByUser(int CustId)
        {
            IQueryable<Job> query = _data.Jobs;
            query = query.Where(c => c.UserId.Equals(CustId));
            
            return await query.ToArrayAsync();

        }
    }
}