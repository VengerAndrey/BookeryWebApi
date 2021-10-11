﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services.Database
{
    public class UserNodeService : IUserNodeService
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;

        public UserNodeService(IDbContextFactory<ApiDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<UserNode>> GetAll()
        {
            await using var context = _contextFactory.CreateDbContext();

            var items = await context.UserNodes.ToListAsync();

            return items;
        }

        public async Task<UserNode> Create(UserNode entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            var createdResult = await context.UserNodes.AddAsync(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return null;
            }
            
            return createdResult.Entity;
        }

        public async Task<UserNode> Update(UserNode entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            context.UserNodes.Update(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return null;
            }

            return entity;
        }

        public async Task<bool> Delete(UserNode entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            context.UserNodes.Remove(entity);
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
