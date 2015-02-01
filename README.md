# ClassyMapper
C# Property Mapper that utilizes attributes in order to perform the mapping.

While I personally love auto mappers, I don't like being forced to configure 
one type to another.  In the end, I am just mapping one set of properties to
another set of properties.  All I care about is that the names match
(or I can tell it what name to look for) and the types are assignable.
ClassyMapper will do just that!

Basic Usage
--------------------------------
```csharp
public class SomeEntity
{
    public string A { get; set; }
    public string B { get; set; }
}

public class SomeDto
{
	[MapProperty]
    public string A { get; set; }
	[MapProperty]
	public string B { get; set; }
}
```

Once these properties are set, the only code you need to run is:

```csharp
var entity = new SomeEntity { A = "1", B = "2" };
var dto = ClassyMapper.New().Map<SomeDto>(entity);
```

The dto now stores all the data the entity did!

Let's say you want to go from that Dto back to the Entity. No problem!  
You don't even have to put any MapProperty attributes on the entity:

```csharp
var entity = ClassyMapper.New().Map<SomeEntity>(dto);
```