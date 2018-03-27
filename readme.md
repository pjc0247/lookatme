lookatme
====

Studying purpose .NET obfuscator.

Goal
----
* 최대한 간단한 구현 방법으로 코드 난독화 패턴 구현
  * 실제로 복잡한 난독화 코드를 생성하기보단 각 패턴들을 짧게짧게 구현하는것이 목표
* 실제로 동작하는 exe 뽑아내기

Patterns
----
* [x] Encode/Decode string literals
* [x] Force inlining
* [x] Change field names.
* [ ] Change method names.
  * [ ] Also supports `System.Reflection`.
* [ ] Change method symbols.
* [ ] Runtime code generation.