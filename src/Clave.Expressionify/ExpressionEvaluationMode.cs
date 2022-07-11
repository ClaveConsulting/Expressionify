namespace Clave.Expressionify;

public enum ExpressionEvaluationMode
{
    /// <summary> Always check for <code>[Expressionify]</code> extension methods when executing a query. </summary>
    FullCompatibilityButSlow = 0,

    /// <summary>
    /// Use the EF compiled query cache and only check for <code>[Expressionify]</code> extension methods when a query gets cached.<br/>
    /// Not all queries work with this mode enabled. For those queries who don't, you get an <code>InvalidOperationException</code> and you need
    /// to call <code>query.Expressionify()</code> explicitly.<br/>
    /// This is the case for <code>[Expressionify]</code>-methods that introduce new query-parameters either directly, or indirectly via an EF optimization.
    /// </summary>
    LimitedCompatibilityButCached = 1
}