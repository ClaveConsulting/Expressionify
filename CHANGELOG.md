## **6.6.2** <sub><sup>2022-12-14 ([174ff0d...753ab7a](https://github.com/ClaveConsulting/Expressionify/compare/174ff0d...753ab7a?diff=split))</sup></sub>

*no relevant changes*

## **6.6.1** <sub><sup>2022-12-12 ([6ae459e...2f5c15f](https://github.com/ClaveConsulting/Expressionify/compare/6ae459e...2f5c15f?diff=split))</sup></sub>

### Bug Fixes
*  Generated source files end in \.g\.cs ([6ae459e](https://github.com/ClaveConsulting/Expressionify/commit/6ae459e))
*  tests ([2f5c15f](https://github.com/ClaveConsulting/Expressionify/commit/2f5c15f))


### ???
*  Replacing Environment\.NewLine with actual new lines to make Git handle line ending differences between platforms\. Ref https://github\.com/dotnet/roslyn/issues/51437\#issuecomment\-784750434 ([aa1a1b5](https://github.com/ClaveConsulting/Expressionify/commit/aa1a1b5))


## **6.6.0** <sub><sup>2022-11-04 ([e0c50b9...118d2eb](https://github.com/ClaveConsulting/Expressionify/compare/e0c50b9...118d2eb?diff=split))</sup></sub>

### Features
*  Added support for the EF compiled query cachen when using \.UseExpressionify\(\) ([e0c50b9](https://github.com/ClaveConsulting/Expressionify/commit/e0c50b9))


### Bug Fixes
*  Mark Generator as development dependency ([d8650c2](https://github.com/ClaveConsulting/Expressionify/commit/d8650c2))
*  Fixed failing tests and an outdated exception message ([1a82a24](https://github.com/ClaveConsulting/Expressionify/commit/1a82a24))
*  Running tests against the pull request, not the target ([118d2eb](https://github.com/ClaveConsulting/Expressionify/commit/118d2eb))


### ???
*  Use result of ParameterExtractingExpressionVisitor ([6de12cc](https://github.com/ClaveConsulting/Expressionify/commit/6de12cc))
*  Default query caching ([c56b4d3](https://github.com/ClaveConsulting/Expressionify/commit/c56b4d3))
*  Renamed ExpressionEvaluationMode enums ([07f00a8](https://github.com/ClaveConsulting/Expressionify/commit/07f00a8))


## **6.5.0** <sub><sup>2022-04-29 ([b8b62c1...b8b62c1](https://github.com/ClaveConsulting/Expressionify/compare/b8b62c1...b8b62c1?diff=split))</sup></sub>

### Features
*  Use QueryCompiler to support Include\(\) ([b8b62c1](https://github.com/ClaveConsulting/Expressionify/commit/b8b62c1))


## **6.4.1** <sub><sup>2022-04-29 ([993a4c7...7e03b8c](https://github.com/ClaveConsulting/Expressionify/compare/993a4c7...7e03b8c?diff=split))</sup></sub>

### Bug Fixes
*  support all features that \.Expressionify\(\) does ([993a4c7](https://github.com/ClaveConsulting/Expressionify/commit/993a4c7))


### ???
*  doc: Improved readme ([9e1032a](https://github.com/ClaveConsulting/Expressionify/commit/9e1032a))


## **6.4.0** <sub><sup>2022-04-29 ([c8a8dce...01bb336](https://github.com/ClaveConsulting/Expressionify/compare/c8a8dce...01bb336?diff=split))</sup></sub>

### Features
*  Added \.UseExpressionify\(\) on DbContext\-Configuration, removing the need to always call \.Expressionify\(\) on each query ([c8a8dce](https://github.com/ClaveConsulting/Expressionify/commit/c8a8dce))


### ???
*  Merge pull request \#14 from jhartmann123/db\-context\-options ([cd6b113](https://github.com/ClaveConsulting/Expressionify/commit/cd6b113))
*  run on ubuntu ([ba62561](https://github.com/ClaveConsulting/Expressionify/commit/ba62561))
*  Updated dependencies and inlined the test\-report ([fc8a819](https://github.com/ClaveConsulting/Expressionify/commit/fc8a819))
*  Fixed linefeed difference between windows and linux ([69e8a80](https://github.com/ClaveConsulting/Expressionify/commit/69e8a80))
*  other way around ([caf6cd6](https://github.com/ClaveConsulting/Expressionify/commit/caf6cd6))
*  Fixed failed build ([01bb336](https://github.com/ClaveConsulting/Expressionify/commit/01bb336))


## **6.4.0** <sub><sup>2022-04-21 ([c8a8dce...c8a8dce](https://github.com/ClaveConsulting/Expressionify/compare/c8a8dce...c8a8dce?diff=split))</sup></sub>

### Features
*  Added \.UseExpressionify\(\) on DbContext\-Configuration, removing the need to always call \.Expressionify\(\) on each query ([c8a8dce](https://github.com/ClaveConsulting/Expressionify/commit/c8a8dce))