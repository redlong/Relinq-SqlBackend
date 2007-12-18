using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using Rubicon.Collections;
using Rubicon.Data.Linq.Clauses;
using Rubicon.Data.Linq.Parsing;
using Rubicon.Data.Linq.DataObjectModel;
using Rubicon.Utilities;

namespace Rubicon.Data.Linq.SqlGeneration
{
  public class SqlGeneratorVisitor : IQueryVisitor
  {
    private readonly IDatabaseInfo _databaseInfo;
    
    public SqlGeneratorVisitor (IDatabaseInfo databaseInfo)
    {
      _databaseInfo = databaseInfo;
      Tables = new List<Table>();
      Columns = new List<Column>();
    }

    public List<Table> Tables { get; private set; }
    public List<Column> Columns { get; private set; }
    public ICriterion Criterion{ get; private set; }

    public void VisitQueryExpression (QueryExpression queryExpression)
    {
      queryExpression.FromClause.Accept (this);
      queryExpression.QueryBody.Accept (this);
    }

    public void VisitMainFromClause (MainFromClause fromClause)
    {
      Table tableEntry = DatabaseInfoUtility.GetTableForFromClause(_databaseInfo, fromClause);
      Tables.Add (tableEntry);
    }

    public void VisitAdditionalFromClause (AdditionalFromClause fromClause)
    {
      Table tableEntry = DatabaseInfoUtility.GetTableForFromClause (_databaseInfo, fromClause);
      Tables.Add (tableEntry);
    }

    public void VisitJoinClause (JoinClause joinClause)
    {
    }

    public void VisitLetClause (LetClause letClause)
    {
    }

    public void VisitWhereClause (WhereClause whereClause)
    {
      WhereConditionParser conditionParser = new WhereConditionParser (whereClause, _databaseInfo);
      Criterion = conditionParser.GetCriterion();
    }

    public void VisitOrderByClause (OrderByClause orderByClause)
    {
    }

    public void VisitOrderingClause (OrderingClause orderingClause)
    {
    }

    public void VisitSelectClause (SelectClause selectClause)
    {
      SelectProjectionParser projectionParser = new SelectProjectionParser (selectClause, _databaseInfo);
      IEnumerable<Tuple<FromClauseBase, MemberInfo>> selectedFields = projectionParser.SelectedFields;
      
      IEnumerable<Column> columns =
          from field in selectedFields
          let table = DatabaseInfoUtility.GetTableForFromClause (_databaseInfo, field.A)
          select DatabaseInfoUtility.GetColumn (_databaseInfo, table, field.B);

      Columns.AddRange (columns);
    }

    public void VisitGroupClause (GroupClause groupClause)
    {
    }

    public void VisitQueryBody (QueryBody queryBody)
    {
      foreach (IFromLetWhereClause fromLetWhereClause in queryBody.FromLetWhereClauses)
        fromLetWhereClause.Accept (this);
      if (queryBody.OrderByClause != null)
        queryBody.OrderByClause.Accept (this);
      queryBody.SelectOrGroupClause.Accept (this);
    }
  }
}