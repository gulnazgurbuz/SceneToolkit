# Factory Pattern Implementation in C#

This repository contains an implementation of the **Factory Design Pattern** in C#. The pattern provides an interface for creating objects in a superclass while allowing subclasses to alter the type of objects that will be created.

## Features
- Abstract factory base class (`BaseFactory`)
- Factory interface (`IFactory`) for consistency
- Product interface (`IProduct`) ensuring common functionality
- Mock implementations for testing purposes (`MockFactory`, `MockProduct`)
- Unit tests using **NUnit** to verify functionality

## Getting Started

### Prerequisites
- .NET SDK (latest recommended version)
- NUnit for running unit tests

### Installation
Clone the repository:
```sh
git clone https://github.com/gulnazgurbuz/factory-pattern.git
```

## Structure
```
📂 GG.DesignPatterns.FactoryPattern
 │── Runtime/
 │   ├── BaseFactory.cs           # Abstract factory implementation
 │   ├── IFactory.cs              # Factory interface
 │   ├── IProduct.cs              # Product interface
 │── Tests/
 │   ├── MockFactory.cs           # Concrete factory for testing
 │   ├── MockProduct.cs           # Concrete product implementation
 │   ├── FactoryPatternTests.cs   # NUnit test cases
```

## Usage

### Implementing a Custom Factory
To create a new factory, inherit from `BaseFactory`:
```csharp
public class CustomFactory : BaseFactory {
    public override IProduct CreateProduct() {
        return new CustomProduct();
    }
}
```

### Creating a Custom Product
Implement `IProduct` for custom behavior:
```csharp
public class CustomProduct : IProduct {
    public void Initialize() {
        Console.WriteLine("Custom Product Initialized!");
    }
}
```


## Unit Tests
The repository includes test cases to validate the implementation:
- **Factory creates a product** (`MockFactory` ensures a product is instantiated)
- **Product initializes correctly** (`MockProduct` verifies initialization logic)

