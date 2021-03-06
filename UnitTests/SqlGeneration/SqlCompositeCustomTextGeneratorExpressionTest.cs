// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.Development.UnitTesting.Parsing;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Linq.SqlBackend.UnitTests.SqlGeneration
{
  [TestFixture]
  public class SqlCompositeCustomTextGeneratorExpressionTest
  {
    private SqlCompositeCustomTextGeneratorExpression _sqlCompositeCustomTextGeneratorExpression;
    private ConstantExpression _expression1;
    private ConstantExpression _expression2;

    [SetUp]
    public void SetUp ()
    {
      _expression1 = Expression.Constant ("5");
      _expression2 = Expression.Constant ("1");
      _sqlCompositeCustomTextGeneratorExpression = new SqlCompositeCustomTextGeneratorExpression (typeof (Cook), _expression1, _expression2);
    }

    [Test]
    public void Generate ()
    {
      var visitor = MockRepository.GeneratePartialMock<TestableExpressionVisitor>();
      var commandBuilder = new SqlCommandBuilder();

      visitor
          .Expect (mock => mock.Visit (_expression1))
          .Return (_expression1);
      visitor
          .Expect (mock => mock.Visit (_expression2))
          .Return (_expression2);
      visitor.Replay();

      _sqlCompositeCustomTextGeneratorExpression.Generate (commandBuilder, visitor, MockRepository.GenerateStub<ISqlGenerationStage>());

      visitor.VerifyAllExpectations();
    }

    [Test]
    public void VisitChildren_ExpressionsNotChanged ()
    {
      var visitorMock = MockRepository.GenerateMock<ExpressionVisitor> ();
      var expressions = _sqlCompositeCustomTextGeneratorExpression.Expressions;
      visitorMock.Expect (mock => mock.Visit (expressions[0])).Return (expressions[0]);
      visitorMock.Expect (mock => mock.Visit (expressions[1])).Return (expressions[1]);
      visitorMock.Replay ();

      var result = ExtensionExpressionTestHelper.CallVisitChildren (_sqlCompositeCustomTextGeneratorExpression, visitorMock);

      visitorMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_sqlCompositeCustomTextGeneratorExpression));
    }

    [Test]
    public void VisitChildren_ChangeExpression ()
    {
      var visitorMock = MockRepository.GenerateMock<ExpressionVisitor> ();
      var expressions = new ReadOnlyCollection<Expression> (new List<Expression> { Expression.Constant (1), Expression.Constant (2) });
      visitorMock.Expect (mock => mock.Visit (_sqlCompositeCustomTextGeneratorExpression.Expressions[0])).Return (expressions[0]);
      visitorMock.Expect (mock => mock.Visit (_sqlCompositeCustomTextGeneratorExpression.Expressions[1])).Return (expressions[1]);
      visitorMock.Replay ();

      var result = ExtensionExpressionTestHelper.CallVisitChildren (_sqlCompositeCustomTextGeneratorExpression, visitorMock);

      visitorMock.VerifyAllExpectations ();
      Assert.That (result, Is.Not.SameAs (_sqlCompositeCustomTextGeneratorExpression));
    }

    [Test]
    public void To_String ()
    {
      var result = _sqlCompositeCustomTextGeneratorExpression.ToString();

      Assert.That (result, Is.EqualTo ("\"5\" \"1\""));
    }
  }
}