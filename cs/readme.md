# fork

change : C/C++ to C#  
add : 6 card omaha evaluator

# enable library

## 1. install .net 5.0 sdk

## 2. create binary file  
no_flush_omaha_6.bin file is dummy  

execute HashTableGenerator
```
PokerHandEvaluator\cs\HashTableGenerator> dotnet run
```
created no_flush_omaha_6.bin  
move PokerHandEvaluator\cs\src\Resources\no_flush_omaha_6.bin

## 3. test
```
PokerHandEvaluator\cs\test\PokerHandEvaluatorTest> dotnet test
```
