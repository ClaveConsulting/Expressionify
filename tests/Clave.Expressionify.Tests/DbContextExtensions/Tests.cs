using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace Clave.Expressionify.Tests.DbContextExtensions
{
    public class Tests
    {
        [Test]
        public void UseExpressionifyInConfig_ExpandsExpression_CanTranslate()
        {
            using var dbContext = new TestDbContext(GetOptions(useExpressionify: true));
            var query = dbContext.TestEntities.Select(e => e.GetName("oh hi"));

            var sql = query.ToQueryString();
            sql.ShouldStartWith("SELECT 'oh hi ' || \"t\".\"Name\"");
        }

        [Test]
        public void UseExpressionifyInQuery_ExpandsExpression_CanTranslate()
        {
            using var dbContext = new TestDbContext(GetOptions(useExpressionify: false));
            var query = dbContext.TestEntities.Expressionify().Select(e => e.GetName("oh hi"));

            var sql = query.ToQueryString();
            sql.ShouldStartWith("SELECT 'oh hi ' || \"t\".\"Name\"");
        }

        [Test]
        public void UseExpressionifyInQueryAndConfig_ExpandsExpression_CanTranslate()
        {
            using var dbContext = new TestDbContext(GetOptions(useExpressionify: true));
            var query = dbContext.TestEntities.Expressionify().Select(e => e.GetName("oh hi"));

            var sql = query.ToQueryString();
            sql.ShouldStartWith("SELECT 'oh hi ' || \"t\".\"Name\"");
        }

        [Test]
        public void DontUseExpressionify_EfSelectsWholeEntity()
        {
            // This is basically a self-test of the test setup. EF should select the whole entity here, instead of the "optimized" version where
            // the concatenation is done in the statement and only the required name is selected
            using var dbContext = new TestDbContext(GetOptions(useExpressionify: false));
            var query = dbContext.TestEntities.Select(e => e.GetName("oh hi"));

            var sql = query.ToQueryString();
            sql.ShouldStartWith("SELECT \"t\".\"Id\", \"t\".\"Name\"");
        }

        [Test]
        public void Expressionify_ShouldHandleWhereWithParameters()
        {
            using var dbContext = new TestDbContext(GetOptions(useExpressionify: true));
            var query = dbContext.TestEntities.Expressionify().Where(e => e.IsSomething());

            query.ToQueryString().ShouldBe($".param set @__Name_0 'Something'{Environment.NewLine}{Environment.NewLine}SELECT \"t\".\"Id\", \"t\".\"Name\"{Environment.NewLine}FROM \"TestEntities\" AS \"t\"{Environment.NewLine}WHERE \"t\".\"Name\" = @__Name_0");
        }

        [Test]
        public void UseExpressionify_ShouldHandleWhereWithParameters()
        {
            using var dbContext = new TestDbContext(GetOptions(useExpressionify: true));
            var query = dbContext.TestEntities.Where(e => e.IsSomething());

            query.ToQueryString().ShouldBe($".param set @__Name_0 'Something'{Environment.NewLine}{Environment.NewLine}SELECT \"t\".\"Id\", \"t\".\"Name\"{Environment.NewLine}FROM \"TestEntities\" AS \"t\"{Environment.NewLine}WHERE \"t\".\"Name\" = @__Name_0");
        }

        [Test]
        public void UseExpressionify_ShouldProduceSameOutputAsExpressionify()
        {
            using var dbContext = new TestDbContext(GetOptions(useExpressionify: true));
            var queryA = dbContext.TestEntities.Where(e => e.IsSomething());
            var queryB = dbContext.TestEntities.Expressionify().Where(e => e.IsSomething());

            queryA.ToQueryString().ShouldBe(queryB.ToQueryString());
        }

        private DbContextOptions GetOptions(bool useExpressionify)
        {
            var builder = new DbContextOptionsBuilder<TestDbContext>().UseSqlite("DataSource=:memory:");

            if (useExpressionify)
                builder.UseExpressionify();

            return builder.Options;
        }
    }
}
