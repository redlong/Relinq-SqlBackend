﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Data.Linq;
using System.Linq;

namespace Remotion.Data.Linq.IntegrationTests.TestDomain.Northwind
{
  internal class LinqToSqlNorthwindDataProvider : INorthwindDataProvider
  {
    private readonly Northwind _dataContext = new Northwind ("Data Source=localhost;Initial Catalog=Northwind; Integrated Security=SSPI;");

    public IQueryable<Product> Products
    {
      get { return _dataContext.Products; }
    }

    public IQueryable<Customer> Customers
    {
      get { return _dataContext.Customers; }
    }

    public IQueryable<Employee> Employees
    {
      get { return _dataContext.Employees; }
    }

    public IQueryable<Category> Categories
    {
      get { return _dataContext.Categories; }
    }

    IQueryable<Order> INorthwindDataProvider.Orders
    {
      get { return _dataContext.Orders; }
    }

    public IQueryable<OrderDetail> OrderDetails
    {
      get { return _dataContext.OrderDetails; }
    }

    public IQueryable<Contact> Contacts
    {
      get { return _dataContext.Contacts; }
    }

    public IQueryable<Invoices> Invoices
    {
      get { return _dataContext.Invoices; }
    }

    public IQueryable<QuarterlyOrder> QuarterlyOrders
    {
      get { return _dataContext.QuarterlyOrders; }
    }

    public IQueryable<Shipper> Shippers
    {
      get { return _dataContext.Shippers; }
    }

    public IQueryable<Supplier> Suppliers
    {
      get { return _dataContext.Suppliers;  }
    }

    public decimal? TotalProductUnitPriceByCategory (int categoryID)
    {
      return _dataContext.TotalProductUnitPriceByCategory(categoryID);
    }

    public decimal? MinUnitPriceByCategory (int? nullable)
    {
      return _dataContext.MinUnitPriceByCategory (nullable);
    }

    public IQueryable<ProductsUnderThisUnitPriceResult> ProductsUnderThisUnitPrice (decimal @decimal)
    {
      return _dataContext.ProductsUnderThisUnitPrice (@decimal);
    }

    public int CustomersCountByRegion (string wa)
    {
      return _dataContext.CustomersCountByRegion (wa);
    }

    public ISingleResult<CustomersByCityResult> CustomersByCity (string london)
    {
      return _dataContext.CustomersByCity (london);
    }

    public IMultipleResults WholeOrPartialCustomersSet (int p0)
    {
      return _dataContext.WholeOrPartialCustomersSet (p0);
    }

    public IMultipleResults GetCustomerAndOrders (string seves)
    {
      return _dataContext.GetCustomerAndOrders (seves);
    }

    public void CustomerTotalSales (string customerID, ref decimal? totalSales)
    {
      _dataContext.CustomerTotalSales (customerID, ref totalSales);
    }
  }
}