﻿using Kawkaba.Core;
using Kawkaba.RepositoryLayer.Interfaces;
using Kawkaba.Core.Entity.ApplicationData;
using Kawkaba.Core.Entity.Files;

namespace Kawkaba.RepositoryLayer.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IBaseRepository<ApplicationUser> UserRepository { get; set; }


    public IBaseRepository<Paths> PathsRepository { get; set; }
    public IBaseRepository<Images> ImagesRepository { get; set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        UserRepository = new BaseRepository<ApplicationUser>(context);
        PathsRepository = new BaseRepository<Paths>(context);
        ImagesRepository = new BaseRepository<Images>(context);

    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}