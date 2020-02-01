:: 运行网关

set spath=%cd%
echo %spath%

rem Gateway 7001
cd Xms.Api.Gateway\bin\Debug\netcoreapp3.1
start "Gateway" dotnet xms.api.gateway.dll
cd %spath%

rem Identity 7003
cd Xms.Api.Identity\bin\Debug\netcoreapp3.1
start "Identity" dotnet xms.api.identity.dll
cd %spath%

rem Schema  7005
cd Xms.Schema.Api\bin\Debug\netcoreapp3.1
start "Schema" dotnet xms.schema.api.dll
cd %spath%

rem Org  7006
cd xms.api.Org\bin\Debug\netcoreapp3.1
start "Org" dotnet xms.api.Org.dll
cd %spath%

rem EntityData  7007
cd Xms.EntityData.Api\bin\Debug\netcoreapp3.1
start "EntityData" dotnet Xms.EntityData.Api.dll
cd %spath%


rem Flow  7008
cd Xms.Flow.Api\bin\Debug\netcoreapp3.1
start "Flow" dotnet Xms.Flow.Api.dll
cd %spath%

rem Form  7009
cd Xms.Form.Api\bin\Debug\netcoreapp3.1
start "Form" dotnet Xms.Form.Api.dll
cd %spath%