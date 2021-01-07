﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Pagination
{
    static class PaginationExtentions
    {
        public static async Task<TResult> PaginateAsync<TDto, TResult>(
            this IQueryable<TDto> source, 
            PagedQuery<TResult> request)
            where TResult: PagedResponse<TDto>, new()
        {
            if (request.PageNumber < 1)
                throw new InvalidQueryException("Page number must be greather than 0");
            if (request.PageSize < 1)
                throw new InvalidQueryException("Page size must me greather than 0");
            if (request.PageNumber > Constants.MaxPageSize)
                throw new InvalidQueryException($"Max page must be less or equal {Constants.MaxPageSize}");

            var result = new TResult
            {
                CurrentPage = request.PageNumber,
                TotalPages = (int) Math.Ceiling(
                    await source.CountAsync() / (double) request.PageSize),
                Items = await source
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync()
            };

            result.PageSize = result.Items.Count;

            return result;
        }
    }
}
