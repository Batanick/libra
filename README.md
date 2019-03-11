# Libra - The Balancing Framework 
Libra is a framework designed to ease creating and managing big sets of configurations. The ultimate goal is to provide tools that will allow others to design, build and configure complex systems without writing code. 

## Main ideas 
- extensibility is a king. Creating new field or adding resource type should be easy. We don't care where changes are coming from and where they go. Where and how configuration are stored should be irrelevant
- declarative style. Creating and working with resources should be done in declarative manner
- cross platform. Even though we targeting C# and Unity stack, we should not use platform dependent frameworks in core libraries. 

## Definitions
**Resource type** - template for creating **resources**. Can contain different variety of fields, but always contain an *id* as one of elements 
**Resource** - an consistent set of data describing instance of **Resource type**. Contains unique **id** value.
**Resource system** - a consistent set of *resources*
**Resource id** - unique *resource* identifier. Framework provides runtime guarantees that *id* is unique inside resource system instance. 

### Supported field types
**int** - signed 4 byte integer number
**float** - signed 4 byte floating number
**boolean** - true / false
**Reference** - a reference pointer from one resource to another one
**Resource part** - contains a set of field values that. Does not have resource id and cannot be referenced from other resources.
**Array** - contains multiple values of other types. For non-reference types might contain empty values 

## Modules 
TODO: add dependency diagram 
# Shared 
A set of classes provided by the client 
# Core 
# Factory 
A main module for loadting 

# Validation 

# Open questions
- id type. GUID or Ints? 
- how editor will looks like? 
- editor UI framework 