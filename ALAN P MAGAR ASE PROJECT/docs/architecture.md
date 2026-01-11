# Architecture

## Projects
- **BOOSE**: Core interpreter/parser library (DLL) used by UI and CLI.
- **booseapp**: WinForms application for running BOOSE programs and drawing.
- **boosecli**: Command-line runner (optional PNG export).
- **booseapp.tests**: MSTest unit tests.

## Execution flow
1. User enters a BOOSE program (UI or CLI).
2. Program is parsed into statements and expressions (AST).
3. Statements execute against an `ICanvas` implementation.
4. Commands update canvas state and draw shapes.

## Interfaces used
- `ICommand`: common interface for executable drawing commands.
- `IExpression`: expression tree nodes produced by the parser.
- `ICanvas`: abstraction for drawing surface used by commands.

## Design patterns
- **Factory**: `CommandFactory` maps keywords to `ICommand` objects.
- **Singleton**: `CommandFactorySingleton` provides one shared factory instance.
- **Command pattern**: each drawing operation is an `ICommand` executed by statements.
