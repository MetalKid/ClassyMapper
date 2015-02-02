# ClassyMapper
--------------------------------
C# Property Mapper that utilizes attributes in order to perform the mapping.

While I personally love auto mappers, I don't like being forced to configure one type to another.  In the end, we are just mapping one set of properties to another set of properties.  All I care about is that the names match (or we can tell it what name to look for) and the types are assignable.
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

Let's say you want to go from that Dto back to the Entity. No problem!  You don't even have to put any MapProperty attributes on the entity:

```csharp
var entity = ClassyMapper.New().Map<SomeEntity>(dto);
```

Full Class Mapping
--------------------------------
Sometimes, you may want to map all the properties.  If you have 100 properties to map, adding [MapProperty] could get tedious.  Thus, you can attribute the class with [MapClass] and ClassyMapper will try to map all the properties based upon their given PropertyName.  You can still add [MapProperty] if you need to change a few names and they will be included.

```csharp
[MapClass]
public class SomeDto
{
    public string A { get; set; }

    public string B { get; set; }
}
```

The call is the same as in the basic version.

Custom Mapping
--------------------------------
You may want to do the bulk of your mapping with ClassyMapper, but there are scenarios that require complicated logic to do that mapping.  You can pass in a method for this scenario.  For example:

```csharp
public class SomeDto
{
    [MapProperty]
    public string A { get; set; }

    [MapProperty]
    public string B { get; set; }

    public string SomeRandomThing { get; set; }
}
```

If you want to set this value to "Blarb" if A is "1" then:

```csharp
var dto = ClassyMapper.New().RegisterCustomMap<SomeEntity, SomeDto>((from, to) =>
{
    if (from.A == "1")
    {
        to.SomeRandomThing = "Blarb";
    }
})
.Map<SomeDto>(entity);
```

Obviously, you can pass in a specific method here if you need to reuse this more than once.  If you go backwards from Dto to Entity, then you will need a different Map <SomeDto, SomeEntity> or it won't get called. The To object will have been fully mapped by now, too.

Multiple From Objects
--------------------------------
You may want to flatten a hierarchy or map some entities to a single domain object.  Like CustomMapping, you will need to register a method callback to return the objects to map with.  For example:

```csharp
public class SomeEntity
{
    public string A { get; set; }
    public string B { get; set; }
    public SomeChildEntity Child { get; set; }
}

public class SomeChildEntity
{
    public string C { get; set; }
}

[MapClass]
public class SomeDto
{
    public string A { get; set; }
    public string B { get; set; }
    public string C { get; set; }
}
```
If you want to flatten the hierarchy, do the following:
```csharp
var dto = ClassyMapper.New()
                  .RegisterFromObjects<SomeEntity>(from => new object[] { from, from.Child })
                  .Map<SomeDto>(entity);
```
Be careful here because you need to include the from object as part of the return array.  If you don't, I won't map from it because there may be some scenarios where if some property on From is some value, you want to return an entirely different class instead.  Also, if two property names are the same, the object you return last wins.

Timestamp
--------------------------------
A timestamp can be automatically mapped by using the [MapPropertyTimestamp] attribute on your Timestamp property.  The entity side will be byte[] and the "Dto" side will be string.  ClassyMapper will automatically base64 encode/decode where appropriate.  Here is an example:

```csharp
public class SomeEntity
{
    public byte[] Timestamp { get; set; }
}

public class SomeDto
{
    [MapPropertyTimestamp]
    public string Timestamp { get; set; }
}
```
After that, you can map normally and it will do the conversion for you!  This is useful for when you are returning a timestamp during a web call.

Namespace Conflicts
--------------------------------
When you flatten out a hierarchy, there may end up being two properties with the same name that you'd like to map to your target object.  You can specify the specific namespace+class in the MapPropertyAttribute like this:

```csharp

        public class SomeEntity
        {
            public string A { get; set; }
        }

        public class  SomeEntity2
        {
            public string A { get; set; }
        }

        private class NamespaceDto
        {
            [MapProperty("A", "Somenamespace.SomeEntity")]
            public string A1 { get; set; }

            [MapProperty("A", "Somenamespace.SomeEntity2")]
            public string A2 { get; set; }
        }

```
You can then map with FromObjects like before to merge these 2 entities into a single Dto.  In order to get back both entities later, you will have to call .Map<> once for each Entity type.

Nullable vs Non-Nullable
--------------------------------
ClassMapper will automatically map a non-nullable field to a nullable field if they are of the same base type.  When mapping from a nullable type to a non-nullable type, ClassyMapper will map the nullable type so long as it isn't null.  If it is null, it won't map it and your To object will have the default value of that base type.

Map Lists
--------------------------------
If you want to map two lists of objects, you can do that, too!  However, an IList will be returned (instance of List<T>) because I have to call the Add method.  Here is an example:

```csharp
List<SomeEntity> input = new List<SomeEntity>
{
    new SomeEntity {A = "1", B = "2"},
    new SomeEntity {A = "4", B = "5"},
};

IList<SomeDto> result = ClassyMapper.New().MapToList<SomeDto, SomeEntity>(input);
```

Complex Types
--------------------------------
If you have a hierarchy in place that you want to keep on the other side, ClassyMapper will bring it over for you.  Take this for example:

```csharp
public class SomeEntity
{
    public string A { get; set; }
    public AnotherEntity Child { get; set; }
}
public class AnotherEntity
{
   public long Id { get; set; }
   public string Name { get; set; }
}
```
If you want to map this to a Dto and keep the hierarchy intact, you would setup the Dtos like this (You can also give the property a different name and still map from the other object by specifying the property name you want in the MapProperty attribute):
```csharp
public class SomeDto
{
    [MapProperty("A")]
    public string A { get; set; }

    [MapProperty("Child")]
    public AnotherDto SomeOtherName { get; set; }
}
[MapClass]
public class AnotherDto
{
    public long Id { get; set; }
    public string Name { get; set; }
}
```
You would simply call the Map method like the Basic example shows and everything will be mapped over automatically.  However, your To objects must all have a parameterless constructor or... bad things happen!

Enum Mapping
--------------------------------
Assuming the Enum and base type on the other side inherit from the same base type (i.e. "int" normally), Enums will automatically be mapped over.  Here is how you can set that up:

```csharp
public class SomeEntity
{
    public int TheValue { get; set; }
}
public class SomeDto
{
    public TheValueEnum TheValue { get; set; }
}
public enum TheValue  // int is default
{
    Unknown = 0,
    SomeVal = 1,
    SomeOtherVal = 2
}
```
Then you can map the same way you normally do (like Basic example) and it'll carry it over for you.  If you go from the Dto to the entity, it will convert it back to the int for you, too!  However, if you try to assign a value to the enum that is not defined, the property will not be set so the value will be whatever the defualt Enum value is.

If the enum is coming from a string, it will automatically try to map that as well, in both directions.

SubList Mapping
--------------------------------
ClassyMapper will map sub lists for you as well, assuming they implement IEnumerable.  However, the property must be compatible with IList<T> or it won't be able to map it.  ClassyMapper needs to be able to call Add when it loops through and maps each individual object.  Here is an example:

```csharp
public class SomeEntity
{
    public string A { get; set; }
    public ICollection<AnotherEntity> Children { get; set; }
}
public class AnotherEntity
{
   public long Id { get; set; }
   public string Name { get; set; }
}

[MapClass]
public class SomeDto
{
    public string A { get; set; }
    public IList<AnotherDto> Children { get; set; }
}
[MapClass]
public class AnotherDto
{
    public long Id { get; set; }
    public string Name { get; set; }
}
```
You call map just like the Basic example showed and the Lists will be mapped automatically.

Recursive Properties
--------------------------------
With Entity Framework, you often have a parent referencing children and children referencing the parent.  If for some reason you need/want to keep this relationship on the other side, ClassyMapper will realize it already mapped the parent and use that reference to avoid an infinite loop of mapping.  Take this for example:

```csharp
private class ParentEntity
{
    public string Test { get; set; }
    public ICollection<ChildEntity> Children { get; set; }
}

private class ChildEntity
{
    public string ChildA { get; set; }
    public ParentEntity Parent { get; set; }
}

[MapClass]
private class ParentDto
{
    public string Test { get; set; }
    public IList<ChildDto> Children { get; set; }
}

[MapClass]
private class ChildDto
{
    public string ChildA { get; set; }
    public ParentDto Parent { get; set; }
}

```
Then you call just like you do the basic call:

```csharp
ParentEntity input = new ParentEntity { Test = "1", Children = new List<ChildEntity>() };
input.Children.Add(new ChildEntity { ChildA = "A", Parent = input });
input.Children.Add(new ChildEntity { ChildA = "B", Parent = input });

var dto = ClassyMapper.New().Map<ParentDto>(input);
```

ClassyMapperConfig
--------------------------------
A few configuration properties can be set when calling ClassyMapper.New():

CreateToObjectFromNullFromObject - This tells ClassyMapper to create an instance of the To object even if the From object was null. You might do this in a RDLC (local SSRS) file so you can ignore a ton of IsNot Nothing checks. - Default: False

MaxNullDepth - If CreateToObjectFromNullFromObject is True, then we need a way to prevent infinite loops from happening (i.e. child was null but child also had reference to parent).  Thus, this will limit how "deep" this fake creation goes.  - Default: 10.

MapEmptyListFromNullList - If the From has a list that ends up being null, then setting this to true will create an empty list on the To object. - Default: True

ThrowExceptionIfNoMatchingPropertyFound - If you always expect your To Object properties to be fully mapped (i.e. you can match every MapProperty on the To Object to a corresponding one on the From object(s)), then setting this to True will throw an exception is this is not the case. - Default: False

IgnoreEnumCase - When an enum is mapped from a string property, this will cause the Enum.Parse call to ignore case if set to true. - Default: True

IIsNullable
--------------------------------
One interface, IIsNullable, is included with ClassyMapper.  Sometimes, when you map from an entity to a dto, the entity is null.  Normally, you'd expect the resutling Dto to be null as well.  But sometimes, you are using this Dto in a local SSRS (RDLC) file and don't want to do 10 IsNot Nothing checks when assigning the value...  Thus, you would need a way to determine if this Dto was mapped from a null object or that object just had no data.  By defining this interface, the "IsNull" property is added and will be assigned True if this scenario happens.  However, you must turn on the Configuration property 'CreateToObjectFromNullFromObject' in order to enable this functionality.  One of the side effects of this, of course, is that you could have an infinite depth issue.  If this null object has a reference to its parent and vice-versa, you'd end up in an infinite loop.  Since the source object was null, there is no way to know where to end.  By default, ClassyMapper will go 10 levels deep and then stop.  You can configure this depth to your liking.  Thus, at the end of that 10th level, all the sub entities would finally be null.

# Helper Methods

CopyValues
--------------------------------
If you want to copy all matching properties from one class to another specifically and without using the MapProperty/MapClass attributes, you can use the CopyValues static method on ClassyMapper.  However, it will only copy the values if the names match and the types are assignable to each other.  Otherwise, nothing gets mapped.  Also, the reference will be directly copied, including lists.  Just a heads up. [Let me know if you use this method and would like the choice as to whether the reference is used or we do a "Map".]  The classes do not have to be of the same type.

```csharp
public class SomeDto
{
    public string A { get; set; }
    public string B { get; set; }
}
var from = new SomeDto { A = "1", B = "2" };
public to = new SomeDto();

ClassyMapper.CopyValues(from, to);
```

DefaultStringValues
--------------------------------
If you want to initialize all the string values of a class to a particular value, like string.Empty, you can call this method and pass in your class.

```csharp
public class SomeDto
{
    public string A { get; set; }
    public string B { get; set; }
}
var dto = new SomeDto();
ClassyMapper.DefaultStringValues(dto);
```

DefaultStringValuesIfNull
--------------------------------
If you have a partially assigned class and want to default the rest of the null strings to some value, you can use this method.

```csharp
public class SomeDto
{
    public string A { get; set; }
    public string B { get; set; }
}
var dto = new SomeDto { A = "1" };
ClassyMapper.DefaultStringValuesIfNull(dto, "SomeValue");
```

If there are any more features you'd like to see, let me know! :)
