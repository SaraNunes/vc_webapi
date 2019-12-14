﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using vc_webapi.Data;

namespace vc_webapi.Tests
{
    public class TestFixture<TContext> where TContext : DbContext
    {
        private readonly Func<DbContextOptions<TContext>, TContext> ContextFactory;
        public TestFixture(Func<DbContextOptions<TContext>, TContext> contextFactory)
        {
            ContextFactory = contextFactory;
        }
        public SqliteConnection ConnectionFactory() => new SqliteConnection("DataSource=:memory:");

        public DbContextOptions<TContext> ModelDbOptionsFactory(SqliteConnection connection)  =>
            new DbContextOptionsBuilder<TContext>()
            .UseSqlite(connection)
            .Options;

        public async Task RunWithDatabaseAsync<TResult, TArrange>(
            Func<TContext, Task<TArrange>> arrange,
            Func<TContext, TArrange, Task<TResult>> act,
            Action<TResult, TArrange, TContext> assert)
        {
            var modelDb = ConnectionFactory();
            await modelDb.OpenAsync();

            try
            {
                var modelOpts = ModelDbOptionsFactory(modelDb);
                using var modelContext = ContextFactory(modelOpts);
                await modelContext.Database.EnsureCreatedAsync();

                // Arrange
                TArrange arrangeVar = default;
                if(arrange != null) 
                    arrangeVar = await arrange.Invoke(modelContext);
                // Act
                var result = await act.Invoke(modelContext, arrangeVar);
                // Assert
                assert.Invoke(result, arrangeVar, modelContext);
            }
            finally
            {
                await modelDb.CloseAsync();
            }
        }
        public async Task RunWithDatabaseAsync<TResult, TArrange>(
            Func<TContext, Task<TArrange>> arrange,
            Func<TContext, TArrange, Task<TResult>> act,
            Action<TResult, TArrange> assert)
        {
            await RunWithDatabaseAsync(arrange, act, (t1, t2, t3) => assert(t1, t2));
        }

        public async Task RunWithDatabaseAsync<TResult, TArrange>(
            Func<TContext, TArrange> arrange,
            Func<TContext, TArrange, Task<TResult>> act,
            Action<TResult, TArrange> assert)
        {
            await RunWithDatabaseAsync(db => Task.FromResult(arrange(db)), act, (t1, t2, t3) => assert(t1, t2));
        }
    }
}
