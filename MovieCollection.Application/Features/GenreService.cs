using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Domain;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Application.Features
{
    public class GenreService(IUnitOfWork unitOfWork) : IGenreService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<AppResponse<List<Genre>>> RegisterAsync(List<Genre> genres)
        {
            var result = new List<Genre>();

            var genresWithId = genres.Where(genre => genre.Id != default!).Select(g => g.Id).ToList();
            var genresWithoutId = genres.Where(genre => genre.Id == default!).ToList();

            result.AddRange(await this.unitOfWork.GenreRepository.Trackable()
                .Where(genre => 
                    genresWithId.Contains(genre.Id) || 
                    genresWithoutId.Select(g => g.Name.ToLower()).Contains(genre.Name.ToLower()))
                .ToListAsync());

            genresWithoutId = genresWithoutId.Where(genre => !result.Select(r => r.Name.ToLower()).Contains(genre.Name.ToLower())).ToList();

            foreach (var genre in genresWithoutId) 
            {
                var resultInsert = this.unitOfWork.GenreRepository.InsertOrUpdate(genre);

                if (!resultInsert.Succeeded)
                    return AppResponse<List<Genre>>.Error(resultInsert.Messages);

                result.Add(genre);
            }

            return AppResponse<List<Genre>>.Success(result);
        }
    }
}
