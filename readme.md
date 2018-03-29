lookatme
====

.NET obfuscator for Studying purpose.

Goal
----
* Implements well-known obfuscation patterns with as simple as possible codes.
  * 실제로 복잡한 난독화 코드를 생성하기보단 각 패턴들을 짧게짧게 구현하는것이 목표
* Generates `exe` which runs exactly same as original.

Patterns
----
* [x] Encode/Decode string literals
* [x] Force inlining
* [x] Change field names.
* [ ] Change method names.
  * [ ] Also supports `System.Reflection`.
* [ ] Change method symbols.
* [ ] Runtime code generation.

