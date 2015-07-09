# Introduction #

The following is not supposed to be LINQ, that would be far more complicated and LINQ is essentially its own interface, which is not the point. The point is, if ICriteria was written today in .net 3.5, what could it look like? How could it change? This library is essentially a set of extensions methods that enable the use of Lambda expressions to add Criteria to the ICriteria interface.

## Dependencies ##
This library is **independent** of the main System.BusinessObject.Framework, so it can be included in any NHibernate 2.0 project by simply referencing the assembly System.BusinessObjects.Expressions.

# Examples #

Adding a simple EQ expression:
```
ICriteria c = session.CreateExpression<Person>()  
      .Add(p => p.FirstName == "John")  
      .Criteria;  
```

Adding multiple types of restrictions using the .Add method
```
ICriteria c = session.CreateExpression<person>()  
       .Add(p => p.FirstName == "John") //Restriction.Eq()  
       .Add(p => p.LastName != null)    //Restriction.IsNotNull()   
       .Add(p => p.ID > 0 && p.ID < 1000) //Restriction.Between()  
       .Criteria;  
```

Joining with an Alias
```
ICriteria c = session.CreateExpression<Person>()  
      .Alias<Address>(p => p.Addresses, "addr")  
            .Add(a => a.Postcode != null)  
            .AddAndReturn(a => a.Address2 == null)  
      .Criteria; 
```