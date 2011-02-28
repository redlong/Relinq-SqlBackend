// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.SqlBackend.SqlPreparation.MethodCallTransformers
{
  /// <summary>
  /// <see cref="ReplaceMethodCallTransformer"/> implements <see cref="IMethodCallTransformer"/> for the <see cref="string.Replace(string,string)"/> 
  /// method.
  /// </summary>
  public class ReplaceMethodCallTransformer : IMethodCallTransformer
  {
    public static readonly MethodInfo[] SupportedMethods =
        new[]
        {
           MethodCallTransformerUtility.GetInstanceMethod (typeof (string), "Replace", typeof (string), typeof (string))
         };

    public Expression Transform (MethodCallExpression methodCallExpression)
    {
      ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);

      MethodCallTransformerUtility.CheckArgumentCount (methodCallExpression, 2);
      MethodCallTransformerUtility.CheckInstanceMethod (methodCallExpression);
      
      return new SqlFunctionExpression (methodCallExpression.Type, "REPLACE", methodCallExpression.Object, methodCallExpression.Arguments[0],methodCallExpression.Arguments[1]);
    }
  }
}