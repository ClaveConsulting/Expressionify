using System;
using System.Collections.Generic;
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
            using var dbContext = new TestDbContext(GetOptions());
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
            using var dbContext = new TestDbContext(GetOptions());
            var prefix = "oh hi";
            var query = dbContext.TestEntities.Expressionify().Select(e => e.GetName(prefix));

            var sql = query.ToQueryString();
            sql.ShouldBe($".param set @__p_0 'oh hi '{Environment.NewLine}{Environment.NewLine}SELECT @__p_0 || \"t\".\"Name\"{Environment.NewLine}FROM \"TestEntities\" AS \"t\"");
        }

        [Test]
        public void DontUseExpressionify_EfSelectsWholeEntity()
        {
            // This is basically a self-test of the test setup. EF should select the whole entity here, instead of the "optimized" version where
            // the concatenation is done in the statement and only the required name is selected
            using var dbContext = new TestDbContext(GetOptions(useExpressionify: false));
            var prefix = "oh hi";
            var query = dbContext.TestEntities.Select(e => e.GetName(prefix));

            var sql = query.ToQueryString();
            sql.ShouldStartWith("SELECT \"t\".\"Id\", \"t\".\"Created\", \"t\".\"Name\"");
        }

        [Test]
        public void Expressionify_ShouldHandleWhereWithParameters_AfterExpansion()
        {
            using var dbContext = new TestDbContext(GetOptions());
            var query = dbContext.TestEntities.Expressionify().Where(e => e.IsSomething());

            query.ToQueryString().ShouldBe($".param set @__Name_0 'Something'{Environment.NewLine}{Environment.NewLine}SELECT \"t\".\"Id\", \"t\".\"Created\", \"t\".\"Name\"{Environment.NewLine}FROM \"TestEntities\" AS \"t\"{Environment.NewLine}WHERE \"t\".\"Name\" = @__Name_0");
        }

        [TestCase(ExpressionEvaluationMode.FullCompatibilityButSlow)]
        [TestCase(ExpressionEvaluationMode.LimitedCompatibilityButCached)]
        public void UseExpressionify_ShouldHandleConstants(ExpressionEvaluationMode mode)
        {
            var name = "oh hi";
            using var dbContext = new TestDbContext(GetOptions(o => o.WithEvaluationMode(mode)));
            var query = dbContext.TestEntities.Where(e => e.IsJohnDoe());

            query.ToQueryString().ShouldBe($"SELECT \"t\".\"Id\", \"t\".\"Created\", \"t\".\"Name\"{Environment.NewLine}FROM \"TestEntities\" AS \"t\"{Environment.NewLine}WHERE \"t\".\"Name\" = 'John Doe'");
        }

        [TestCase(ExpressionEvaluationMode.FullCompatibilityButSlow)]
        [TestCase(ExpressionEvaluationMode.LimitedCompatibilityButCached)]
        public void UseExpressionify_ShouldHandleWhereWithParameters(ExpressionEvaluationMode mode)
        {
            var name = "oh hi";
            using var dbContext = new TestDbContext(GetOptions(o => o.WithEvaluationMode(mode)));
            var query = dbContext.TestEntities.Where(e => e.NameEquals(name));

            query.ToQueryString().ShouldBe($".param set @__name_0 'oh hi'{Environment.NewLine}{Environment.NewLine}SELECT \"t\".\"Id\", \"t\".\"Created\", \"t\".\"Name\"{Environment.NewLine}FROM \"TestEntities\" AS \"t\"{Environment.NewLine}WHERE \"t\".\"Name\" = @__name_0");
        }

        [Test]
        public void UseExpressionify_EvaluationModeAlways_ShouldHandleWhereWithNewParameters()
        {
            using var dbContext = new TestDbContext(GetOptions(optionsAction: o => o.WithEvaluationMode(ExpressionEvaluationMode.FullCompatibilityButSlow)));
            var query = dbContext.TestEntities.Where(e => e.IsSomething());

            query.ToQueryString().ShouldBe($".param set @__Name_0 'Something'{Environment.NewLine}{Environment.NewLine}SELECT \"t\".\"Id\", \"t\".\"Created\", \"t\".\"Name\"{Environment.NewLine}FROM \"TestEntities\" AS \"t\"{Environment.NewLine}WHERE \"t\".\"Name\" = @__Name_0");
        }

        [Test]
        public void UseExpressionify_EvaluationModeAlways_ShouldHandleWhereWithExternalServices()
        {
            using var dbContext = new TestDbContext(GetOptions(optionsAction: o => o.WithEvaluationMode(ExpressionEvaluationMode.FullCompatibilityButSlow)));
            var query = dbContext.TestEntities.Where(e => e.IsRecent());

            query.ToQueryString().ShouldBe($".param set @__AddDays_0 '2022-03-03 05:06:07'{Environment.NewLine}{Environment.NewLine}SELECT \"t\".\"Id\", \"t\".\"Created\", \"t\".\"Name\"{Environment.NewLine}FROM \"TestEntities\" AS \"t\"{Environment.NewLine}WHERE \"t\".\"Created\" > @__AddDays_0");
        }

        [Test]
        public void UseExpressionify_EvaluationModeCached_CannotHandleNewParameters()
        {
            using var dbContext = new TestDbContext(GetOptions(optionsAction: o => o.WithEvaluationMode(ExpressionEvaluationMode.LimitedCompatibilityButCached)));
            var query = dbContext.TestEntities.Where(e => e.IsSomething());

            var exception = Should.Throw<InvalidOperationException>(() => query.ToQueryString());
            exception.Message.ShouldBe("Accessing parameters in a cached query context is not allowed. Explicitly call .Expressionify() on the query or use ExpressionEvaluationMode.Always.");
        }

        [Test]
        public void UseExpressionify_EvaluationModeCached_CannotHandleParametersFromExternalServices()
        {
            using var dbContext = new TestDbContext(GetOptions(optionsAction: o => o.WithEvaluationMode(ExpressionEvaluationMode.LimitedCompatibilityButCached)));
            var query = dbContext.TestEntities.Where(e => e.IsSomething());

            var exception = Should.Throw<InvalidOperationException>(() => query.ToQueryString());
            exception.Message.ShouldBe("Accessing parameters in a cached query context is not allowed. Explicitly call .Expressionify() on the query or use ExpressionEvaluationMode.Always.");
        }

        [Test]
        public void UseExpressionify_ShouldProduceSameOutputAsExpressionify()
        {
            using var dbContext = new TestDbContext(GetOptions());
            var queryA = dbContext.TestEntities.Where(e => e.IsSomething());
            var queryB = dbContext.TestEntities.Expressionify().Where(e => e.IsSomething());

            queryA.ToQueryString().ShouldBe(queryB.ToQueryString());
        }

        [TestCase(ExpressionEvaluationMode.FullCompatibilityButSlow)]
        [TestCase(ExpressionEvaluationMode.LimitedCompatibilityButCached)]
        public void UseExpressionify_ShouldProduceSameOutputAsExpressionify_InAllModes(ExpressionEvaluationMode mode)
        {
            // Note: when not using the result of ParameterExtractingExpressionVisitor, the Cached mode returns another query with an additional concat (which would be unintended)

            using var dbContext = new TestDbContext(GetOptions(o => o.WithEvaluationMode(mode)));
            var queryA = dbContext.TestEntities.Select(e => e.GetName("oh hi"));
            var queryB = dbContext.TestEntities.Expressionify().Select(e => e.GetName("oh hi"));

            queryA.ToQueryString().ShouldBe(queryB.ToQueryString());
        }

        [Test]
        public void UseExpressionify_EvaluationModeAlways_ShouldHandleEvaluatableExpressions()
        {
            using var dbContext = new TestDbContext(GetOptions(o => o.WithEvaluationMode(ExpressionEvaluationMode.FullCompatibilityButSlow)));
            var query = dbContext.TestEntities.Select(e => e.ToTestView(null));

            query.ToQueryString().ShouldBe($"SELECT \"t\".\"Name\", NULL AS \"Street\"{Environment.NewLine}FROM \"TestEntities\" AS \"t\"");
        }

        [Test]
        public void UseExpressionify_EvaluationModeCached_CannotHandleEvaluatableExpressions()
        {
            using var dbContext = new TestDbContext(GetOptions(o => o.WithEvaluationMode(ExpressionEvaluationMode.LimitedCompatibilityButCached)));
            var query = dbContext.TestEntities.Select(e => e.ToTestView(null));

            var exception = Should.Throw<InvalidOperationException>(() => query.ToQueryString());
            exception.Message.ShouldBe("Accessing parameters in a cached query context is not allowed. Explicitly call .Expressionify() on the query or use ExpressionEvaluationMode.Always.");
        }

        [TestCase(ExpressionEvaluationMode.FullCompatibilityButSlow)]
        [TestCase(ExpressionEvaluationMode.LimitedCompatibilityButCached)]
        public void UseExpressionify_WithEvaluationMode_SetsEvaluationMode(ExpressionEvaluationMode mode)
        {
            var options = GetOptions(o => o.WithEvaluationMode(mode));
            var extension = options.FindExtension<ExpressionifyDbContextOptionsExtension>()!;
            
            var debugInfo = new Dictionary<string, string>();
            extension.Info.PopulateDebugInfo(debugInfo);
            
            debugInfo["Expressionify:EvaluationMode"].ShouldBe(mode.ToString());
        }

        [Test]
        public void UseExpressionify_EvaluationMode_DefaultsToAlways()
        {
            var options = GetOptions();
            var extension = options.FindExtension<ExpressionifyDbContextOptionsExtension>()!;
            
            var debugInfo = new Dictionary<string, string>();
            extension.Info.PopulateDebugInfo(debugInfo);
            
            debugInfo["Expressionify:EvaluationMode"].ShouldBe(ExpressionEvaluationMode.FullCompatibilityButSlow.ToString());
        }

        private DbContextOptions GetOptions(Action<ExpressionifyDbContextOptionsBuilder>? optionsAction = null, bool useExpressionify = true)
        {
            var builder = new DbContextOptionsBuilder<TestDbContext>().UseSqlite("DataSource=:memory:");

            if (useExpressionify)
                builder.UseExpressionify(optionsAction);

            return builder.Options;
        }
    }
}
