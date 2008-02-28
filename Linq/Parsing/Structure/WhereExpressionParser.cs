using System;
using System.Linq.Expressions;
using Rubicon.Data.Linq.Parsing.Structure;
using Rubicon.Utilities;

namespace Rubicon.Data.Linq.Parsing.Structure
{
  public class WhereExpressionParser
  {
    private readonly ParseResultCollector _resultCollector;
    private readonly bool _isTopLevel;

    public WhereExpressionParser (ParseResultCollector resultCollector, MethodCallExpression whereExpression, bool isTopLevel)
    {
      ArgumentUtility.CheckNotNull ("resultCollector", resultCollector);
      ArgumentUtility.CheckNotNull ("whereExpression", whereExpression);

      _resultCollector = resultCollector;
      _isTopLevel = isTopLevel;

      ParserUtility.CheckMethodCallExpression (whereExpression, resultCollector.ExpressionTreeRoot, "Where");
      if (whereExpression.Arguments.Count != 2)
        throw ParserUtility.CreateParserException ("Where call with two arguments", whereExpression, "Where expressions",
            resultCollector.ExpressionTreeRoot);

      SourceExpression = whereExpression;
      ParseWhere ();
    }

    public MethodCallExpression SourceExpression { get; private set; }

    private void ParseWhere ()
    {
      UnaryExpression unaryExpression = ParserUtility.GetTypedExpression<UnaryExpression> (SourceExpression.Arguments[1],
          "second argument of Where expression", _resultCollector.ExpressionTreeRoot);
      LambdaExpression ueLambda = ParserUtility.GetTypedExpression<LambdaExpression> (unaryExpression.Operand,
          "second argument of Where expression", _resultCollector.ExpressionTreeRoot);

      SourceExpressionParser sourceExpressionParser = new SourceExpressionParser (new ParseResultCollector (_resultCollector.ExpressionTreeRoot), SourceExpression.Arguments[0], false,
          ueLambda.Parameters[0], "first argument of Where expression");

      foreach (LambdaExpression projectionExpression in sourceExpressionParser.ProjectionExpressions)
        _resultCollector.AddProjectionExpression (projectionExpression);
      foreach (BodyExpressionBase bodyExpression in sourceExpressionParser.BodyExpressions)
        _resultCollector.AddBodyExpression (bodyExpression);

      _resultCollector.AddBodyExpression (new WhereExpression (ueLambda));
      
      if (_isTopLevel)
        _resultCollector.AddProjectionExpression (null);
    }
  }
}