﻿using System.Threading.Tasks;
using MovieCollection.Domain;

namespace MovieCollection.Application.Contracts.Persistence
{
    public interface IUnitOfWork
    {
        IGenericRepository<Movie> MovieRepository { get; }
        IGenericRepository<Genre> GenreRepository { get; }

        Task CommitAsync();
    }
}
