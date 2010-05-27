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
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.UnitTests.Linq.Core;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlGeneration.IntegrationTests
{
  [TestFixture]
  public class SelectProjectionSqlBackendIntegrationTest : SqlBackendIntegrationTestBase
  {
    [Test]
    public void Entity ()
    {
      CheckQuery (
          from s in Cooks select s,
          "SELECT [t0].[ID],[t0].[FirstName],[t0].[Name],[t0].[IsStarredCook],[t0].[IsFullTimeCook],[t0].[SubstitutedID],[t0].[KitchenID] "
          + "FROM [CookTable] AS [t0]");
    }

    [Test]
    public void Constant ()
    {
      CheckQuery (
          from k in Kitchens select "hugo",
          "SELECT @1 AS [value] FROM [KitchenTable] AS [t0]",
          new CommandParameter ("@1", "hugo"));
    }

    [Test]
    public void Null ()
    {
      CheckQuery (
          Kitchens.Select<Kitchen, object> (k => null),
          "SELECT NULL AS [value] FROM [KitchenTable] AS [t0]");
    }

    [Test]
    public void True ()
    {
      CheckQuery (
          from k in Kitchens select true,
          "SELECT @1 AS [value] FROM [KitchenTable] AS [t0]",
          new CommandParameter ("@1", 1));
    }

    [Test]
    public void False ()
    {
      CheckQuery (
          from k in Kitchens select false,
          "SELECT @1 AS [value] FROM [KitchenTable] AS [t0]",
          new CommandParameter ("@1", 0));
    }

    [Test]
    public void BooleanConditions ()
    {
      CheckQuery (
          from k in Kitchens select k.Name == "SpecialKitchen",
          "SELECT CASE WHEN ([t0].[Name] = @1) THEN 1 ELSE 0 END AS [value] FROM [KitchenTable] AS [t0]",
          new CommandParameter ("@1", "SpecialKitchen"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = 
        "The member 'Cook.Assistants' describes a collection and can only be used in places where collections are allowed.")]
    public void Collection_ThrowsNotSupportedException ()
    {
      CheckQuery (
          from c in Cooks select c.Assistants,
          "");
    }

    [Test]
    public void NestedSelectProjection ()
    {
      CheckQuery (
          from c in (from sc in Cooks select new { A = sc.Name, B = sc.ID }).Distinct() where c.B != 0 select c.A,
            "SELECT [q0].[get_A] AS [value] FROM ("
            + "SELECT DISTINCT [t1].[Name] AS [get_A],[t1].[ID] AS [get_B] FROM [CookTable] AS [t1]) AS [q0] "
            + "WHERE ([q0].[get_B] <> @1)",
            new CommandParameter("@1", 0)
          );
    }

    [Test]
    public void NestedSelectProjection_AccessingEntity ()
    {
      CheckQuery (
          from c in (from sc in Cooks select new { A = sc, B = sc.ID }).Distinct () where c.B != 0 select c.A,
            "SELECT [q0].[get_A_ID],[q0].[get_A_FirstName],[q0].[get_A_Name],[q0].[get_A_IsStarredCook],[q0].[get_A_IsFullTimeCook],"
            + "[q0].[get_A_SubstitutedID],[q0].[get_A_KitchenID] FROM ("
            + "SELECT DISTINCT [t1].[ID] AS [get_A_ID],[t1].[FirstName] AS [get_A_FirstName],[t1].[Name] AS [get_A_Name],"
            + "[t1].[IsStarredCook] AS [get_A_IsStarredCook],[t1].[IsFullTimeCook] AS [get_A_IsFullTimeCook],"
            + "[t1].[SubstitutedID] AS [get_A_SubstitutedID],[t1].[KitchenID] AS [get_A_KitchenID],[t1].[ID] AS [get_B] "
            + "FROM [CookTable] AS [t1]) AS [q0] WHERE ([q0].[get_B] <> @1)",
            new CommandParameter ("@1", 0)
          );
    }

    [Test]
    public void NestedNestedSelectProjection ()
    {
      CheckQuery (
          from x in Kitchens
          from c in
            ( // SubStatementTableInfo (SqlStatement (
              from sc in Cooks
              select // SelectProjection = NamedExpression (null, 
                new
                { // NewExpression (
                  A = 10, // NamedExpression ("get_A", ...),  => SqlValueReference ("get_A")
                  B = sc.Name, // NamedExpression ("get_B", ...),  => SqlValueReference ("get_B")
                  C = new
                  { // NewExpression ( => SqlCompoundReference
                    D = sc.Name
                  }
                }) // NamedExpression ("get_C_get_D", ...))))) => SqlValueReference ("get_C_get_D")
          where c.C.D != null // MemberExpression (MemberExpression (SqlTableReferenceExpression))
          select c.B,
          "SELECT [q0].[get_B] AS [value] FROM [KitchenTable] AS [t1] CROSS APPLY "+
          "(SELECT @1 AS [get_A],[t2].[Name] AS [get_B],[t2].[Name] AS [get_C_get_D] FROM [CookTable] AS [t2]) AS [q0] "+
          "WHERE ([q0].[get_C_get_D] IS NOT NULL)",
          new CommandParameter("@1", 10));
    }

    [Test]
    [Ignore ("TODO Review 2788 - correct SQL should look very similar to the SQL below")]
    public void NestedSelectProjection_WithJoinOnCompoundReferenceMember ()
    {
      CheckQuery (
          from x in (from c in Cooks select new { A = c, B = c.ID }).Distinct () select x.A.Substitution.FirstName,
          "SELECT [t2].[FirstName] "
          + "FROM (SELECT DISTINCT [t0].[ID] AS [get_A_ID],... FROM [CookTable] AS [t0]) AS [q1] "
          + "LEFT OUTER JOIN [CookTable] AS [t2] ON [q1].[get_A_ID] = [t2].[SubstitutedID]");
    }
   
  }
}