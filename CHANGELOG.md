## **10.0.0**&emsp;<sub><sup>2025-11-19 ([ba0427f...ba0427f](https://github.com/ClaveConsulting/Expressionify/compare/ba0427ffd6c2453538092f8ea503ba8af9499ba4...ba0427ffd6c2453538092f8ea503ba8af9499ba4?diff=split))</sup></sub>

### Features

- migrate to EF Core 10 and \.NET 10 \(\#40\) ([ba0427f](https://github.com/ClaveConsulting/Expressionify/commit/ba0427ffd6c2453538092f8ea503ba8af9499ba4))


### BREAKING CHANGES
-  Upgrade to EF Core 10 and \.NET 10 ([ba0427f](https://github.com/ClaveConsulting/Expressionify/commit/ba0427ffd6c2453538092f8ea503ba8af9499ba4))


  This is a major version upgrade that requires:
  \- \.NET 10 SDK
  \- EF Core 10 packages
  \- Updated parameter naming in generated SQL \(@\_\_p\_0 \-\> @p\)
  
  Core Changes:
  \- Upgrade Microsoft\.EntityFrameworkCore from 9\.0\.0 to 10\.0\.0
  \- Upgrade to \.NET 10 SDK and target framework \(net10\.0\)
  \- Replace ParameterExtractingExpressionVisitor with ExpressionTreeFuncletizer
  \- Remove IParameterValues interface \(replaced with Dictionary<string, object?\>\)
  \- Delete ParameterExtractingExpressionVisitor\.cs \(~700 lines of internal EF Core code\)
  \- Update parameter detection strategy to check dictionary count after funcletization
  \- Update test expectations for simplified parameter naming
  \- Remove obsolete test \(EF Core 10 optimizer handles constant folding\)
  
  All 51 tests passing\.
  
  Co\-authored\-by: Fabien Molinet <molinetf@medgate\.ch\>
<br>

## **9.1.0**&emsp;<sub><sup>2025-03-21 ([579137a...579137a](https://github.com/ClaveConsulting/Expressionify/compare/579137a7550db48071b2a568718011031d3a3244...579137a7550db48071b2a568718011031d3a3244?diff=split))</sup></sub>

### Features

- handle nullable propagation expression in arguments ([579137a](https://github.com/ClaveConsulting/Expressionify/commit/579137a7550db48071b2a568718011031d3a3244))

<br>

## **9.0.1**&emsp;<sub><sup>2025-03-21 ([a928912...a928912](https://github.com/ClaveConsulting/Expressionify/compare/a928912b9559dc3963cb1f2be47abf196b7ea991...a928912b9559dc3963cb1f2be47abf196b7ea991?diff=split))</sup></sub>

*no relevant changes*
<br>

## **9.0.0**&emsp;<sub><sup>2024-12-27 ([0a25faa...ab25144](https://github.com/ClaveConsulting/Expressionify/compare/0a25faa81d7fef929450b4548bef80c7784b7869...ab251447ae87ffcca3c4c8d9f7d0492a63dcb604?diff=split))</sup></sub>


### BREAKING CHANGES
-  supports EF9 ([ab25144](https://github.com/ClaveConsulting/Expressionify/commit/ab251447ae87ffcca3c4c8d9f7d0492a63dcb604))


  \-\-\-\-\-\-\-\-\-
  
  Co\-authored\-by: Fabien Molinet <molinetf@medgate\.ch\>
<br>

## **6.7.1**&emsp;<sub><sup>2024-10-31 ([ad5c447...ddc49b2](https://github.com/ClaveConsulting/Expressionify/compare/ad5c4470f9eb8bc91284c556f719f01b6d0dab49...ddc49b22ebb0feeb77c6c4c7b460117a4a33ef74?diff=split))</sup></sub>

*no relevant changes*
<br>

## **6.7.0**&emsp;<sub><sup>2022-12-14 ([50641f0...3fae05d](https://github.com/ClaveConsulting/Expressionify/compare/50641f0924d179f8c6cceb0ab1c1eea473ac9428...3fae05d19585f2ffa7b23d533c4ab16d98a61f10?diff=split))</sup></sub>

### Features

- Implemented generic methods ([50641f0](https://github.com/ClaveConsulting/Expressionify/commit/50641f0924d179f8c6cceb0ab1c1eea473ac9428))
- Generic classes can contain expressionify methods ([3fae05d](https://github.com/ClaveConsulting/Expressionify/commit/3fae05d19585f2ffa7b23d533c4ab16d98a61f10))

<br>

## **6.6.4**&emsp;<sub><sup>2024-11-11 ([2658a0f...2658a0f](https://github.com/ClaveConsulting/Expressionify/compare/2658a0f86c3062e60e2391e43e25fcd690bbfe4f...2658a0f86c3062e60e2391e43e25fcd690bbfe4f?diff=split))</sup></sub>

*no relevant changes*
<br>

## **6.6.3**&emsp;<sub><sup>2023-07-16 ([4c34cb9...4c34cb9](https://github.com/ClaveConsulting/Expressionify/compare/4c34cb964e517ec5609cc820d969011c7359c447...4c34cb964e517ec5609cc820d969011c7359c447?diff=split))</sup></sub>

*no relevant changes*
<br>

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