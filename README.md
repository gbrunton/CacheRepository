# CacheRepository

A repository that caches data in memory and allows you to use indexes to query data quickly. 

By default, [Dapper-Extensions](https://github.com/tmsmith/Dapper-Extensions) is used internally for relational database support. This means that many of the same configuration features and [conventions](https://github.com/tmsmith/Dapper-Extensions/blob/master/readme.md#naming-conventions) available in Dapper-Extensions applies to CacheRepository as well. 

CacheRepository also supports text files as data sources. It can read from both delimited and fixed width files and write to them as well with little configuration.

I mostly use CacheRepository library within [ETL](http://en.wikipedia.org/wiki/Extract,_transform,_load) programs that I write when it is necessary to be fast and when I will very likely be moving large amounts of data around.

See the following for more details

[CacheRepository Introduction (Why I Made It)](http://gbrunton.blogspot.com/2013/01/cacherepository-introduction-why-i-made.html)

[CacheRepository Use Case - Reading From (Somewhat) Unstructured Text File](http://gbrunton.blogspot.com/2013/01/cacherepository-use-case-reading-from.html)

[Writing To And Reading From Text File](https://github.com/gbrunton/CacheRepository/blob/master/Tests/IntegrationTests/Repositories/FileRepositoryTests.cs)

## Features

* Repository can query both relational databases and file based data sources using the same (or very similar) api

* Insert into and read from both delimited and fixed width text files

* Create in memory indexes 

* Built in Sql Server bulk insert implementation

* Customize sql select queries by entity type

* Use pure POCO entities 

* Optional and configurable entity Id property usage

* Load data from remote data source and persist it locally to avoid having to query the remote data source in future runs.

## Installation

https://nuget.org/packages/CacheRepository

```
PM> Install-Package CacheRepository
```

## Usage

Assume the following type.

```c#
public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string CreatedDate { get; set; }
}
```

## Insert

Insert into sql database

```c#
IDbConnection connection = new SqlConnection(@"data source=....");
var config = new SqlRepositoryConfigBuilder(connection).Build();
using (var repository = config.BuildRepository())
{
	var blog = new Blog
		{
			Title = "Future Time Saver Log",
			Author = "Gary Brunton",
			CreatedDate = DateTime.Now
		};
	repository.Insert(blog);
	repository.Commit();
}
```

Insert into comma delimited text file

```c#
var config = new FileRepositoryConfigBuilder(@"c:\folderContainingFiles")
	.WithFileDelimitor(",")
	.WithFieldQualifier("\"")
	.Build();
using (var repository = config.BuildRepository())
{
	var blog = new Blog
	{
		Title = "Future Time Saver Log",
		Author = "Gary Brunton",
		CreatedDate = DateTime.Now
	};
	repository.Insert(blog);
}
```

## Bulk Insert (SqlRepository only)

```c#
using (var repository = config.BuildRepository())
{
	var stopwatch = new Stopwatch();
	stopwatch.Start();
	for (var i = 0; i < 5000000; i++)
	{
		repository.BulkInsert
			(
				new Blog
					{
						Title = "Future Time Saver Log",
						Author = "Gary Brunton",
						CreatedDate = DateTime.Now
					}
			);
	}
	repository.BulkInsertFlush();
	repository.Commit();
	stopwatch.Stop();
	var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
}
```

On my laptop, this completed in 1.7 minutes.

## Sql Queries (SqlRepository only)

```c#
using (var repository = config.BuildRepository())
{
	var blogs = repository.Query<Blog>("Select Top 100 * From Blog Where Author = @Author"
		, new {Author = "Gary Brunton"});
}
```

This is basically the same API that [Dapper-Extensions](https://github.com/tmsmith/Dapper-Extensions) uses but I'm doing it on the repository and not direction on the connection.

## Update (SqlRepository only)

```c#
using (var repository = config.BuildRepository())
{
	var blog = repository.GetAll<Blog>().Single();
	blog.Author = "Starlin Castro";
	repository.Update(blog);
	repository.Commit();
}
```

## GetAll

Use GetAll to retrieve and cache all entities of the given type from the repository. Once the entities are loaded, if you call it again a round trip to the database will not be performed. Instead the data will be retrieved from cache (memory).

```c#
using (var repository = config.BuildRepository())
{
	var blogs = repository
					.GetAll<Blog>()
					.Where(x => x.Author == "Gary Brunton");
}
```

## Customized GetAll (SqlRepository only)

We can customize the dynamic "GetAll" sql but adding CustomEntitySql for a given entity type. 

The following will ensure that the GetAll method only loads 100 entities into memory.

```c#
var config = new SqlRepositoryConfigBuilder(connection)
	.AddCustomEntitySql<Blog>("Select Top 100 * From Blog")
	.Build();
using (var repository = config.BuildRepository())
{
	var blogs = repository.GetAll<Blog>();
}
```

## Change the default GetAllQueryStrategy

There are times when retrieving data from a data source throws because the data is not formatted properly. For example, I’ve had problems when querying an old db using an odbc driver and the driver throws when accessing a specific DateTime property that contains data that is corrupt. The default strategy will throw and the program will stop. You can use the ‘GetAllQuerySettingDefaultValueOnError’ strategy instead that will not throw but will set the value to the data type’s default value. Unfortunately this strategy is very slow compared to the default one.

```c#
var config = new SqlRepositoryConfigBuilder(connection)
	.WithGetAllQueryStrategy<Blog>(new GetAllQuerySettingDefaultValueOnError())
	.Build();
using (var repository = config.BuildRepository())
{
	var blogs = repository.GetAll<Blog>();
}
```

## In Memory Indexes

Performing Linq on in memory objects can be relatively fast but when you are dealing with several million entities and you want to filter by multiple properties the milliseconds start adding up to several minutes. This is when you should consider Indexes. CacheRepository supports both a UniqueIndex type which will only contain one entity or a NonUniqueIndex type which will contain a collection.

```c#
public class BlogByIdIndex : UniqueIndex<Blog, Guid>
{
	protected override Guid GetKey(Blog entity)
	{
		return entity.Id;
	}
}

public class BlogByAuthorIndex : NonUniqueIndex<Blog, string>
{
	protected override string GetKey(Blog entity)
	{
		return entity.Author;
	}
}

using (var repository = config.BuildRepository())
{
	var blog = repository.GetIndex<BlogByIdIndex>().Get(id);
	
	IEnumerable<Blog> blogs = repository.GetIndex<BlogByAuthorIndex>().Get("Gary Brunton");
}
```

You can even create an index spanning multiple properties.

```c#
public class BlogByTitleAndAuthorIndex : NonUniqueIndex<Blog, Tuple<string,string>>
{
	protected override Tuple<string,string> GetKey(Blog entity)
	{
		return new Tuple<string, string>(entity.Title, entity.Author);
	}
}

using (var repository = config.BuildRepository())
{
	IEnumerable<Blog> enumerable = repository
		.GetIndex<BlogByTitleAndAuthorIndex>()
		.Get(new Tuple<string, string>("Future Time Saver Log", "Gary Brunton"));
}
```

## Text File Support

These in memory Linq statements and in memory indexes work on the FileRepository in the exact same way as they work on the SqlRepository.

I've included some basic support to build up and query text files with the same API. By default if you don't specify a delimitor the [ConstructorContainsLine](https://github.com/gbrunton/CacheRepository/blob/master/CacheRepository/FileEntityFactoryStrategies/ConstructorContainsLine.cs) type is used to build entities by defining an action from within a constructor.

```c#
public class Blog
{
	public Blog(string line)
	{
		Id = Guid.Parse(line.Substring(0, 10));
		Title = line.Substring(10, 20);
		Author = line.Substring(30, 20);
		CreatedDate = DateTime.Parse(line.Substring(50, 8));
	}

	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Author { get; set; }
	public DateTime CreatedDate { get; set; }
}
```

See the [ConstructByEvaluatingDelimitedFile](https://github.com/gbrunton/CacheRepository/blob/master/CacheRepository/FileEntityFactoryStrategies/ConstructByEvaluatingDelimitedFile.cs) type for building up entities from delimited text files.

## File Output Conventions

Implementations of [IOutputConvension](https://github.com/gbrunton/CacheRepository/blob/master/CacheRepository/OutputConventions/IOutputConvention.cs) can be created to apply formatting of property values based on conventions.

For example, the [DateTimeToyyyyMMdd](https://github.com/gbrunton/CacheRepository/blob/master/CacheRepository/OutputConventions/DateTimeToyyyyMMdd.cs) convention will automatically format all DateTime and DateTime? properties to the MMddyyyy format when saved to a text file. See the [DateTimeToyyyyMMddTestFixture](https://github.com/gbrunton/CacheRepository/blob/master/Tests/UnitTests/OutputConventions/DateTimeToyyyyMMddTestFixture.cs) test for details.

## File String Formatter Attributes

You can create your own [IStringFormatterAttribute](https://github.com/gbrunton/CacheRepository/blob/master/CacheRepository/StringFormatterAttributes/IStringFormatterAttribute.cs) implementations and apply them directly to properties for explicit formatting.

For example, the [LengthAttribute](https://github.com/gbrunton/CacheRepository/blob/master/CacheRepository/StringFormatterAttributes/LengthAttribute.cs) attribute allows you to set a field's length and justification.

## Persist Data Locally

Setting the WithPersistedDataPath value of the ConfigBuilder will persist the data locally in the given path. 

```c#
var config = new SqlRepositoryConfigBuilder(connection)
	.WithPersistedDataPath("c:\temp")
	.Build();
using (var repository = config.BuildRepository())
{
	var blogs = repository.GetAll<Blog>();
}
```

I've used this when retrieving data from the original source takes a long time, like ODBC access to slow server. 

Both entities and indexes are persisted so when a call is executed against the repository, it will first look in the given persisted path to see if any data already exists. If not it will query the external data source and ultimately persist it locally. On subsequent program executions, instead of loading from the external data source the data will be retrieved from the local persistent store.

## Still To Come

Text file bulk insert implementation? (Note, I need to look into this. I'm not auto flushing the file stream so my current implementation of Insert might actually be closer to the BulkInsert)

Text file update implementation?
