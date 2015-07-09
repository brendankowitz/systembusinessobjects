# Basic example of usage #

Querying a list of contacts:
```
IList<Person> list = Person.Search(QrySearchContactByName.Query(name));
```

Loading a single contact:
```
Person item = Person.Load(ID);
```

Saving a contact:
```
Person item = Person.Load(ID);
item.Save();
```

Updating a contact:
```
Person item = Person.Load(ID);
item.Name = "new name";
item.Save();
```

Deleting a contact:
```
Person item = Person.Load(ID);
item.Delete();
item.Save();
```