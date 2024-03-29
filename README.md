# FastActivator

[![.NET Publish](https://github.com/JaCraig/FastActivator/actions/workflows/dotnet-publish.yml/badge.svg)](https://github.com/JaCraig/FastActivator/actions/workflows/dotnet-publish.yml) [![Coverage Status](https://coveralls.io/repos/github/JaCraig/FastActivator/badge.svg?branch=master)](https://coveralls.io/github/JaCraig/FastActivator?branch=master)

FastActivator is a C# library that provides a faster alternative to the `System.Activator` class for creating instances of classes.

## Purpose

The purpose of FastActivator is to replace the usage of `System.Activator` with a more efficient implementation, resulting in improved performance when creating instances of classes.

## Speed Comparisons

| Method                      | Mean      | Error    | StdDev   | Rank | Gen0   | Allocated |
|---------------------------- |----------:|---------:|---------:|-----:|-------:|----------:|
| ActivatorCreateInstance     | 182.80 ns | 1.272 ns | 1.127 ns |    2 | 0.0401 |     336 B |
| FastActivatorCreateInstance |  28.27 ns | 0.422 ns | 0.352 ns |    1 | 0.0115 |      96 B |

In many instances it's about 2x to 10x faster depending on the method used.

## Usage

To use FastActivator, follow these steps:

1. Add a reference to the FastActivator library in your project.
    ```ps
    dotnet add package Fast.Activator
    ```

2. Import the required namespaces:
   ```csharp
   using Fast.Activator;
   ```

3. To create an instance of a class using FastActivator, use one of the following methods:

   - `CreateInstance<TClass>(params object[] args)`: Creates an instance of the specified class `TClass` with the provided arguments, if any.
     ```csharp
     var instance = FastActivator.CreateInstance<MyClass>(arg1, arg2);
     ```

   - `CreateInstance<TClass>()`: Creates an instance of the specified class `TClass` without any arguments.
     ```csharp
     var instance = FastActivator.CreateInstance<MyClass>();
     ```

   - `CreateInstance(Type type, params object[] args)`: Creates an instance of the specified `Type` with the provided arguments, if any.
     ```csharp
     var instance = FastActivator.CreateInstance(typeof(MyClass), arg1, arg2);
     ```

   - `CreateInstance(Type type)`: Creates an instance of the specified `Type` without any arguments.
     ```csharp
     var instance = FastActivator.CreateInstance(typeof(MyClass));
     ```

   Note: If the object cannot be created, the methods will return `null`.

## Contributing

Contributions to FastActivator are welcome! If you would like to contribute, please follow these guidelines:

- Fork the repository and create a new branch for your feature or bug fix.

- Make your changes and ensure that the tests pass.

- Submit a pull request describing your changes and the problem they solve.

- Ensure that your code adheres to the project's coding conventions and style guidelines.

## License

FastActivator is released under the [Apache 2.0 License](https://github.com/JaCraig/FastActivator/blob/master/LICENSE).