# FatCat.Fakes


## **Fake object data populate that are perfect for testing or when you need some complete fake data with an easy to use interface**

https://www.nuget.org/packages/FatCat.Fakes/

```
Install-Package FatCat.Fakes -Version 1.0.0
```

###  **Features include**

* All basic C# primitives
* Will create a random interface/abstract implementation from the assembly of the type asked to be created.
* Create List, Arrays, and Dictionaries
* Callback to change object after creation before Faker returns

### *Future Features*

* Ignore properties from faking
* Custom Type Fake Generators

# Examples

#### *Create a basic primitive*

```C#
    var someNumber = Faker.Create<int>();
```

#### *Create a fake 1 level class*

```C#
    public class MyClass
    {
        public int NumberOfTimes { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
    }
    
    var myClass = Faker.Create<MyClass>();

    // json of my class
    {
        "CreatedDate":"2052-08-29T13:29:26.7833974-04:00",
        "Name":"fIcc4ShcN6TC5GZohk7gsO4",
        "NumberOfTimes":623309770
    }
```

#### *Create a list of numbers between 3 and 9 items*

```C#
    var numbers = Faker.Create<List<int>>();

    // json of numbers
    [
        660525537,
        1763346267,
        145042704,
        1123996617,
        2114388821
    ]
```

#### *Create a list with the given length*

```C#
    var numbers = Faker.Create<List<int>>(length: 21);
```

#### *Create a dictionary*

```C#
    var dictionary = Faker.Create<Dictionary<string, MyClass>>(length: 3);

    // json of dictionary
    {
        "6Pab9r323z":{
            "CreatedDate":"2051-01-27T21:13:05.5176146-05:00",
            "Name":"gsscLi27PPhcK96pkjKpyo2vlmHtSYZ9QVM",
            "NumberOfTimes":1211802506
        },
        "gIdxE7sNE6m9fniZ2zr0wqZa9OrpHWp":{
            "CreatedDate":"2030-02-22T23:24:57.5179499-05:00",
            "Name":"qpjDYqgCFigZ",
            "NumberOfTimes":1319840685
        },
        "jHsaFapOJJPQaPIx83CpKOr33D":{
            "CreatedDate":"2041-04-15T10:26:38.517957-04:00",
            "Name":"n25PZHCzRcL9T8aX9e",
            "NumberOfTimes":729603723
        }
    }
```

#### *Create an object with sub objects*

```C#
    public class MultiLevelObject 
    {
        public MyClass ObjectOne { get; set; }

        public Guid Id { get; set; }
    }

    // All properties on top level and sub class are populated
    var levelObject = Faker.Create<MultiLevelObject>();

    // json of levelObject
    {
        "ObjectOne":{
            "CreatedDate":"2042-08-08T09:13:29.1457916-04:00",
            "Name":"u5XHZBhLmv2pVvszrKvexP1s5i59VbC",
            "NumberOfTimes":494396233
        },
        "Id":"e79256c9-06c6-44b5-a9d7-de26d61d892d"
    }
```

#### *Create an interface or an abstract class*

```C#
    public interface IDoSomeWork 
    {
        void DoWork();

        int Result { get; set; }

        string Name { get; }
    }

    public class FirstWorker : IDoSomeWork
    {
        public void DoWork() 
        {
            // Do first work
        }

        public int Result { get; set; }

        public string Name => "FirstWorker";
    }

    public class SecondWorker : IDoSomeWork
    {
        public void DoWork() 
        {
            // Do first work
        }

        public int Result { get; set; }

        public string Name => "SecondWorker";
    }

    // Can create the interface will pick a random implementation between FirstWorker and SecondWorker
    var worker = Faker.Create<IDoSomeWork>();

    // json of worker
    {
        "Result":1086374187,
        "Name":"SecondWorker"
    }
```

#### *Change the object after it is created*

```C#
    var myObject = Faker.Create<MyClass>(i => { i.Name = "I like fakes"; });

    // json of myObject
    {
        "CreatedDate":"2014-08-13T03:41:25.8911822-04:00",
        "Name":"I like fakes",
        "NumberOfTimes":958091195
    }
```

Please explore the unit tests for examples currently supported.  *Happy faking.*